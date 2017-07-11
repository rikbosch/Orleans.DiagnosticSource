using System;
using Microsoft.Extensions.DependencyInjection;

namespace Orleans.DiagnosticSource.Tests
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDiagnostics();

            return services.BuildServiceProvider();
        }
    }
}