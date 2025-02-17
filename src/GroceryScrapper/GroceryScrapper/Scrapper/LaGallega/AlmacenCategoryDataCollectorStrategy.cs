namespace GroceryScrapper.Scrapper.LaGallega;

public class AlmacenCategoryDataCollectorStrategy
{
    private readonly ILogger _logger;
    private readonly ChromeDriver _driver;
    private int _currentPage = 1;

    public AlmacenCategoryDataCollectorStrategy(ILogger logger, ChromeDriver driver)
    {
        _logger = logger;
        _driver = driver;
    }

    public async Task Navigate()
    {
        await _driver.Navigate().GoToUrlAsync("https://www.lagallega.com.ar/productosnl.asp?nl=03010000&TM=cx");
        await Task.Delay(1500);
    }
    
    public async Task GoToNextPage()
    {
        _currentPage++;
        await Task.Delay(1500);
        await _driver.Navigate().GoToUrlAsync($"https://www.lagallega.com.ar/productosnl.asp?nl=03010000&TM=cx&pg={_currentPage}");
    }
    
    public async Task CollectData()
    {
        _logger.LogInformation("Collecting data from Almacen category");
    }
}