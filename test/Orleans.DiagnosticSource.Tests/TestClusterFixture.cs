using System;
using Orleans.DiagnosticSource.Client;
using Orleans.Runtime.Configuration;
using Orleans.TestingHost;

namespace Orleans.DiagnosticSource.Tests
{

    public class TestClusterFixture : IDisposable
    {
        private TestCluster _cluster;


        public TestClusterFixture()
        {
            var options = new TestClusterOptions(2);
            options.ClusterConfiguration.UseStartupType<Startup>();

            _cluster = new TestCluster(options);
            _cluster.Deploy();

            var builder = new ClientBuilder()
                .UseConfiguration(options.ClientConfiguration)
                .UseDiagnostics();

            Client = builder.Build();
            Client.Connect().Wait();
        }

        public IClusterClient Client { get; private set; }

        public void Dispose()
        {
            Client?.Dispose();
            _cluster?.StopAllSilos();
        }
    }
}
