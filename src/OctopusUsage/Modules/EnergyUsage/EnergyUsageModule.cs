namespace OctopusUsage.Modules.EnergyUsage;

public class EnergyUsageModule : ModuleBase
{
    public override IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection(nameof(OctopusConfiguration)).Get<OctopusConfiguration>());
        services.AddHttpClient<IOctopusEnergyClient, OctopusEnergyClient>();

        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();
            options.UseInMemoryStore();

            options.AddJobAndTriggerWithCronSchedule<SyncOctopusElectricityUsage>(
                configuration.GetValue<string>($"CronSchedules:{nameof(SyncOctopusElectricityUsage)}"));

            options.AddJobAndTriggerWithCronSchedule<SyncOctopusGasUsage>(
                configuration.GetValue<string>($"CronSchedules:{nameof(SyncOctopusGasUsage)}"));
        });

        services.AddQuartzServer(options => options.WaitForJobsToComplete = true);

        return services;
    }

    public override void ExecuteModulePreRunActions(WebApplication app) => app.MapHub<UsageHub>("/usage");
}