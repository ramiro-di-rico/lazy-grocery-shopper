namespace GroceryScrapper.Scrapper.LaGallega;

public class LaGallegaScrapperJob : IInvocable
{
    private readonly ILogger<LaGallegaScrapperJob> _logger;

    public LaGallegaScrapperJob(ILogger<LaGallegaScrapperJob> logger)
    {
        _logger = logger;
    }
    
    public async Task Invoke()
    {
        var value = true;

        if (value)
        {
            return;
        }
        _logger.LogInformation("LaGallegaScrapperJob is running");
        
        var options = new ChromeOptions();
        //options.AddArgument("--headless");
        using var driver = new ChromeDriver(options);
        
        _logger.LogInformation("ChromeDriver initialized");
        
        _logger.LogInformation("Navigating to La Gallega");

        var strategy = new AlmacenCategoryDataCollectorStrategy(_logger, driver);
        await strategy.Navigate();
        await strategy.CollectData();

        
        _logger.LogInformation("La Gallega page loaded");
        
        _logger.LogInformation("Closing ChromeDriver");
        
        driver.Quit();
    }
}