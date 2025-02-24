namespace RestorationBot.Extensions;

public static class ServicesConfigurationExtensions
{
    public static IServiceCollection AddSingletonWithMultipleAbstractions<TImplementation, TFirstAbstraction, TSecondAbstraction>(this IServiceCollection services)
        where TImplementation : class, TFirstAbstraction, TSecondAbstraction
        where TFirstAbstraction : class
        where TSecondAbstraction : class
    {
        services.AddSingleton<TImplementation>();
        
        services.AddSingleton<TFirstAbstraction>(serviceProvider => serviceProvider.GetRequiredService<TImplementation>());
        services.AddSingleton<TSecondAbstraction>(serviceProvider => serviceProvider.GetRequiredService<TImplementation>());

        return services;
    }
}