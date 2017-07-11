using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Orleans.CodeGeneration;
using Orleans.Runtime;

namespace Orleans.DiagnosticSource.Client
{
    public static class ConfigurationExtensions
    {
        public static IClientBuilder UseDiagnostics(this IClientBuilder builder)
        {
            return builder
                .AddClientInvokeCallback(ClientDiagnosticsCallback.Intercept)
                .ConfigureServices(s => s.AddDiagnostics());
        }

        public static IServiceCollection AddDiagnostics(this IServiceCollection services)
        {
            return services;
        }
    }

    static class ClientDiagnosticsCallback
    {

        public static void Intercept(InvokeMethodRequest request, IGrain grain)
        {
            // flow activity.current
            Activity currentActivity = Activity.Current;

            if (currentActivity != null)
            {
                RequestContext.Set(DiagnosticsLoggingStrings.RequestIdHeaderName, currentActivity.Id);
                // we expect baggage to be empty or contain a few items
                var baggage = currentActivity.Baggage.ToArray();
                if (baggage.Length > 0)
                {
                    RequestContext.Set(DiagnosticsLoggingStrings.CorrelationContextHeaderName, baggage);
                }
            }

        }
    }
}