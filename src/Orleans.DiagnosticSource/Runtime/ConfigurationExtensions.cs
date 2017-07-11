using System.Diagnostics;
using Orleans;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Serialization;


namespace Orleans.DiagnosticSource
{

    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddDiagnostics(this IServiceCollection services)
        {
            return services.AddSingleton<IGrainCallFilter, RuntimeDiagnosticsInterceptor>();
        }
    }
}
