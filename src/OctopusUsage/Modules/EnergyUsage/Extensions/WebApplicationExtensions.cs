namespace OctopusUsage.Modules.EnergyUsage.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ExecuteRegisteredJobsAsync(this WebApplication app)
    {
        var factory = app.Services.GetRequiredService<ISchedulerFactory>();
        var jobScheduler = await factory.GetScheduler().ConfigureAwait(false);

        await jobScheduler.TriggerJob(new(nameof(SyncOctopusElectricityUsage))).ConfigureAwait(false);
        await jobScheduler.TriggerJob(new(nameof(SyncOctopusGasUsage))).ConfigureAwait(false);
    }
}