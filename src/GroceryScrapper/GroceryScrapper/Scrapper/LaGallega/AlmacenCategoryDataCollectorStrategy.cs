using System.Globalization;
using OpenQA.Selenium;

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
        await Navigate();

        var pages = GetPages();
        _logger.LogInformation("Found {PagesCount} pages", pages.Count);
        while (_currentPage <= pages.Max())
        {
            var results = ScrapeProductData();
            _logger.LogInformation("Found {ResultsCount} products on page {CurrentPage}", results.Count, _currentPage);
            
            await GoToNextPage();
        }
    }
    
    public List<Product> ScrapeProductData()
    {
        var products = new List<Product>();

        var productElements = _driver.FindElements(By.CssSelector(".cuadProd")); // Adjust the selector based on the actual HTML structure

        foreach (var element in productElements)
        {
            var name = element.FindElement(By.CssSelector(".desc")).Text;
            var priceText = element.FindElement(By.CssSelector(".precio .izq")).Text;
            var price = decimal.Parse(priceText, NumberStyles.Currency, new CultureInfo("es-AR"));

            products.Add(new Product
            {
                Name = name,
                Price = price,
                ImageUrl = "https://www.lagallega.com.ar/" + element.FindElement(By.CssSelector("img")).GetAttribute("src")
            });
        }

        return products;
    }
    
    public List<int> GetPages()
    {
        var pages = new List<int>();

        var pageElements = _driver.FindElements(By.CssSelector(".TxtPagina a.linkPag"));

        foreach (var element in pageElements)
        {
            var pageUrl = element.GetAttribute("onclick");
            var pageNumber = int.Parse(pageUrl.Split('=')[1].Split('&')[0]);
            pages.Add(pageNumber);
        }

        return pages;
    }
}