using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Orleans.Runtime;
using Orleans.Serialization;

namespace Orleans.DiagnosticSource
{
    public class RuntimeDiagnosticsInterceptor : IGrainCallFilter
    {
        static readonly DiagnosticListener _listener = new DiagnosticListener(DiagnosticsLoggingStrings.RuntimeDiagnosticListenerName);

        /// <summary>
        /// Creates a diagnostic interceptor
        /// </summary>
        /// <param name="inner">The decorated existing InvokeInterceptor, can be null</param>
        public RuntimeDiagnosticsInterceptor(SerializationManager serializationManager)
        {

            _serializationManager = serializationManager;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task Invoke(IGrainCallContext context)
        {
            Activity activity = null;

            // export requestcontext once
            IDictionary<string, object> requestContext = RequestContext.Export(_serializationManager); ;

            if (requestContext!= null && requestContext.ContainsKey(DiagnosticsLoggingStrings.RequestIdHeaderName))
            {
                // flow activity from request headers
                activity = StartActivity(context, requestContext);
            }

            try
            {
                await context.Invoke().ConfigureAwait(false);

                if (activity != null)
                {
                    // is set when diagnosticslistener is enabled
                    // requestcontext dictionary is also not null
                    StopActivity(context, activity, requestContext);
                }
            }
            catch (Exception ex)
            {
                //capture failed method...
                if (_listener.IsEnabled(DiagnosticsLoggingStrings.DiagnosticsUnhandledExceptionName))
                {
                    var timestamp = Stopwatch.GetTimestamp();
                    // Diagnostics is enabled for UnhandledException, but it may not be for BeginRequest
                    // so call GetTimestamp if currentTimestamp is zero (from above)
                    RecordUnhandledExceptionDiagnostics(context, timestamp, ex);
                }
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RecordUnhandledExceptionDiagnostics(IGrainCallContext context, long timestamp, Exception exception)
        {
            //isenabled check is done in parent
            _listener.Write(
                DiagnosticsLoggingStrings.DiagnosticsUnhandledExceptionName,
                new
                {
                    RequestContext = RequestContext.Export(_serializationManager),
                    CallContext = context,
                    Exception = exception,
                    Timestamp = timestamp
                });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Activity StartActivity(IGrainCallContext callContext, IDictionary<string, object> requestContext)
        {
            var activity = new Activity(DiagnosticsLoggingStrings.GrainRequestIn);

            if (requestContext.TryGetValue(DiagnosticsLoggingStrings.RequestIdHeaderName, out object requestId))
            {
                activity.SetParentId((string)requestId);

                // We expect baggage to be empty by default
                // Only very advanced users will be using it in near future, we encourage them to keep baggage small (few items)

                if (requestContext.TryGetValue(DiagnosticsLoggingStrings.CorrelationContextHeaderName, out object baggage))
                {
                    KeyValuePair<string, string>[] values = (KeyValuePair<string, string>[])baggage;

                    foreach (var item in values)
                    {
                        activity.AddBaggage(item.Key, item.Value);
                    }
                }
            }

            if (_listener.IsEnabled(DiagnosticsLoggingStrings.GrainRequestInStartName))
            {
                _listener.StartActivity(activity, new { RequestContext = requestContext, CallContext = callContext });
            }
            else
            {
                // always start activity
                activity.Start();
            }

            return activity;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void StopActivity(IGrainCallContext callContext, Activity activity, IDictionary<string, object> requestContext)
        {
            _listener.StopActivity(activity, new { RequestContext = requestContext, CallContext = callContext });
        }

        private readonly SerializationManager _serializationManager;

    }
}