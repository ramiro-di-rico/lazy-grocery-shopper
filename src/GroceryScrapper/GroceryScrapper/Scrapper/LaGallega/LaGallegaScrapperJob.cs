namespace GroceryScrapper.Scrapper.LaGallega;

public class LaGallegaScrapperJob : IInvocable
{
    private readonly ILogger<LaGallegaScrapperJob> _logger;
    private readonly IEnumerable<ICategoryDataCollector> _dataCollectors;

    public LaGallegaScrapperJob(ILogger<LaGallegaScrapperJob> logger, IEnumerable<ICategoryDataCollector> dataCollectors)
    {
        _logger = logger;
        _dataCollectors = dataCollectors;
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
        
        var groceryStore = new GroceryStore
        {
            Id = 1,
            Name = "La Gallega",
            Url = "https://www.lagallega.com.ar"
        };
        
        _logger.LogInformation("ChromeDriver initialized");

        var jobCollectors = _dataCollectors
            .Where(dataCollector => dataCollector.JobOwner == nameof(LaGallegaScrapperJob))
            .ToList();

        foreach (var categoryDataCollector in jobCollectors)
        {
            await categoryDataCollector.InitializeAsync(driver, groceryStore);
            await categoryDataCollector.CollectDataAsync();
        }

        _logger.LogInformation("Closing ChromeDriver");
        
        driver.Quit();
    }
}