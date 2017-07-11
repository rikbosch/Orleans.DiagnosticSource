using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Orleans.DiagnosticSource.Tests
{
    public class IntegrationTests : IClassFixture<TestClusterFixture>
    {
        private IClusterClient _client;


        public IntegrationTests(TestClusterFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task Activity_should_flow_from_client_to_grain()
        {

            var activity = new Activity("Activity_should_flow_from_client_to_grain")
                .AddBaggage("bag1","val1")
                .Start();

            try
            {
                await _client.GetGrain<ITestGrain>(0).TestClientToGrain(activity.Id);
            }
            finally
            {
                activity.Stop();
            }
        }

        [Fact]
        public async Task Activity_should_flow_from_grain_to_grain()
        {
            var activity = new Activity("Activity_should_flow_from_client_to_grain")
                .AddBaggage("bag2", "val2")
                .Start();

            try
            {
                await _client.GetGrain<ITestGrain>(0).TestGrainToGrain();
            }
            finally
            {
                activity.Stop();
            }
        }
    }
}