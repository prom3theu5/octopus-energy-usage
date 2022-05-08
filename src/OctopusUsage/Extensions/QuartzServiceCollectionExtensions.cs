namespace OctopusUsage.Extensions;

public static class QuartzServiceCollectionExtensions
{
    public static void AddJobAndTriggerWithCronSchedule<T>(this IServiceCollectionQuartzConfigurator quartz, string cronSchedule)
        where T : IJob
    {
        Guard.Against.NullOrEmpty(cronSchedule, nameof(cronSchedule));

        var jobName = typeof(T).Name;

        var jobKey = new JobKey(jobName);
        quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithCronSchedule(cronSchedule));
    }

    public static void AddJobAndTriggerWithTimespanSchedule<T>(this IServiceCollectionQuartzConfigurator quartz, TimeSpan repeatInterval, int timesToRepeat = 0)
        where T : IJob
    {
        Guard.Against.Zero(repeatInterval, nameof(repeatInterval));

        var jobName = typeof(T).Name;

        var jobKey = new JobKey(jobName);

        quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithSimpleSchedule(trigger => WithInterval(trigger, repeatInterval, timesToRepeat)));
    }

    private static SimpleScheduleBuilder WithInterval(SimpleScheduleBuilder trigger, TimeSpan repeatInterval, int timesToRepeat = 0)
    {
        var scheduleBuilder = trigger.WithInterval(repeatInterval);

        return timesToRepeat != 0 ?
            scheduleBuilder.WithRepeatCount(timesToRepeat) :
            scheduleBuilder.RepeatForever();
    }
}