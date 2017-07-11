using System.Threading.Tasks;

namespace Orleans.DiagnosticSource.Tests
{
    public interface ITestGrain : IGrainWithIntegerKey
    {
        Task TestGrainToGrain();

        Task TestClientToGrain(string activityId);
    }
}