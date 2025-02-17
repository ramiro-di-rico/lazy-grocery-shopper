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
        _logger.LogInformation("LaGallegaScrapperJob is running");
        
        var options = new ChromeOptions();
        //options.AddArgument("--headless");
        using var driver = new ChromeDriver(options);
        
        _logger.LogInformation("ChromeDriver initialized");
        
        _logger.LogInformation("Navigating to La Gallega");

        await driver.Navigate().GoToUrlAsync("https://www.lagallega.com.ar/login.asp");

        
        _logger.LogInformation("La Gallega page loaded");
        
        _logger.LogInformation("Closing ChromeDriver");
        
        driver.Quit();
    }
}