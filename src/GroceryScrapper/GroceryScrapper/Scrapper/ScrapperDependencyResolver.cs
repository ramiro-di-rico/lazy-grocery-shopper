namespace GroceryScrapper.Scrapper;

public class ScrapperDependencyResolver
{
    public static IServiceCollection RegisterDependencies(IServiceCollection services)
    {
        services.AddSingleton<ICategoryDataCollector, AlmacenCategoryDataCollectorStrategy>();
        services.AddSingleton<LaGallegaScrapperJob>();
        return services;
    }
}