using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using Orleans.Serialization;
using Shouldly;

namespace Orleans.DiagnosticSource.Tests
{
    public class TestGrain : Grain, ITestGrain
    {
        public async Task TestGrainToGrain()
        {
            var activity = Activity.Current;

            var context = RequestContext.Export(ServiceProviderServiceExtensions.GetService<SerializationManager>(this.ServiceProvider));
            activity.ShouldNotBeNull();



            await this.GrainFactory.GetGrain<ITestGrain>(5).TestGrainToGrain();
        }

        public async Task TestClientToGrain(string activityId)
        {
            var activity = Activity.Current;

            activity.ShouldNotBeNull();
            activity.ParentId.ShouldBe(activityId);
        }
    }
}