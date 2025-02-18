namespace GroceryScrapper.Scrapper;

public class ScrapperDependencyResolver
{
    public static IServiceCollection RegisterDependencies(IServiceCollection services)
    {
        services.AddTransient<ICategoryDataCollector, LaGallegaDataCollector>();
        services.AddTransient<LaGallegaScrapperJob>();
        return services;
    }
}