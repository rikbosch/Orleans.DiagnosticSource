namespace Orleans.DiagnosticSource
{
    public static class DiagnosticsLoggingStrings
    {
        public const string RuntimeDiagnosticListenerName = "Orleans.Runtime";
        public const string GrainRequestIn = "Orleans.Runtime.GrainRequestIn";
        public const string GrainRequestInStartName = "Orleans.Runtime.GrainRequestIn.Start";
        public const string DiagnosticsUnhandledExceptionName = "Orleans.Runtime.UnhandledException";


        public const string RequestIdHeaderName = "Request-Id";
        public const string CorrelationContextHeaderName = "Correlation-Context";

        public const string RuntimeClientDiagnosticListenerName = "Orleans.Client";
    }
}