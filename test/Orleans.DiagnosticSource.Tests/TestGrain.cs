using System.Diagnostics;
using System.Linq;
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
            activity.ShouldNotBeNull();
            activity.Baggage.Any(x => x.Value == "val2" && x.Key == "bag2").ShouldBeTrue();

            if (this.GetPrimaryKeyLong() == 5)
            {
                // prevent overflow...
                return;
            }

            await this.GrainFactory.GetGrain<ITestGrain>(5).TestGrainToGrain();
        }

        public async Task TestClientToGrain(string activityId)
        {
            var activity = Activity.Current;

            activity.Baggage.Any(x=>x.Value== "val1" && x.Key=="bag1").ShouldBeTrue();
            activity.ShouldNotBeNull();
            activity.ParentId.ShouldBe(activityId);
        }
    }
}