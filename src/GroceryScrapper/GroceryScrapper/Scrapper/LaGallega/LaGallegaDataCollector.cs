namespace GroceryScrapper.Scrapper.LaGallega;

public class LaGallegaDataCollector : ICategoryDataCollector
{
    private readonly ILogger<LaGallegaDataCollector> _logger;
    private readonly ApplicationDbContext _dbContext;
    private ChromeDriver _driver;
    private int _currentPage = 0;
    private List<string> _subCategories = new()
    {
        // Store
        "nl=03010000&TM=cx",
        "nl=03020000&TM=cx",
        "nl=03080000&TM=cx",
        "nl=03100000&TM=cx",
        "nl=03170000&TM=cx",
        "nl=03090000&TM=cx",
        "nl=03110000&TM=cx",
        "nl=03040000&TM=cx",
        "nl=03120000&TM=cx",
        "nl=03070000&TM=cx",
        "nl=03030000&TM=cx",
        "nl=03190000&TM=cx",
        "nl=03140000&TM=cx",
        "nl=03210000&TM=cx",
        "nl=03060000&TM=cx",
        //"nl=03150000&TM=cx", Not needed
        
        // Beberages
        "nl=07030000&TM=cx",
        "nl=07040000&TM=cx",
        "nl=07010000&TM=cx",
        "nl=07090000&TM=cx",
        "nl=07050000&TM=cx",
        "nl=07070000&TM=cx",
        "nl=07020000&TM=cx",
        
        //Breakfast
        "nl=06060000&TM=cx",
        "nl=06030000&TM=cx",
        "nl=06010000&TM=cx",
        "nl=06050000&TM=cx",
        "nl=06020000&TM=cx",
        "nl=06070000&TM=cx",
        
    };
    private string _currentSubCategory = "";

    public LaGallegaDataCollector(ILogger<LaGallegaDataCollector> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public GroceryStore GroceryStore { get; private set; }
    public string JobOwner => nameof(LaGallegaScrapperJob);

    public async Task InitializeAsync(ChromeDriver driver, GroceryStore groceryStore)
    {
        _driver = driver;
        GroceryStore = groceryStore;

        await _dbContext.Products
            .Where(product => product.GroceryStoreId == GroceryStore.Id)
            .ExecuteDeleteAsync();

        _logger.LogInformation("ChromeDriver initialized");
    }
    private async Task GoToNextPageAsync()
    {
        _currentPage++;
        await Task.Delay(1500);
        var nextSubCategoryIndex = _subCategories.IndexOf(_currentSubCategory);
        var nextSubCategory = nextSubCategoryIndex == _subCategories.Count - 1 ? _subCategories[0] : _subCategories[nextSubCategoryIndex + 1];
        await _driver.Navigate().GoToUrlAsync($"{GroceryStore.Url}/productosnl.asp?{nextSubCategory}&pg={_currentPage}");
    }
    
    public async Task CollectDataAsync()
    {
        _logger.LogInformation("Collecting data from Almacen category");
        await GoToNextPageAsync();

        var pages = GetPages();
        _logger.LogInformation("Found {PagesCount} pages", pages.Count);
        List<Product> products = [];
        while (_currentSubCategory != _subCategories.Last())
        {
            while (_currentPage <= pages.Max())
            {
                var results = ScrapeProductData();
                _logger.LogInformation("Found {ResultsCount} products on page {CurrentPage}", results.Count,
                    _currentPage);

                await GoToNextPageAsync();
                products.AddRange(results);
            }

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();
            
            _currentSubCategory = _subCategories[_subCategories.IndexOf(_currentSubCategory) + 1];
            _currentPage = 1;
            products.Clear();
            
            await GoToNextPageAsync();
        }
    }
    
    private List<Product> ScrapeProductData()
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
                GroceryStoreId = GroceryStore.Id,
                Name = name,
                Price = price,
                ImageUrl = element.FindElement(By.CssSelector("img")).GetAttribute("src")
            });
        }

        return products;
    }
    
    private List<int> GetPages()
    {
        var pages = new List<int>();

        var pageElements = _driver.FindElements(By.CssSelector(".TxtPagina a.linkPag"));

        foreach (var element in pageElements)
        {
            var pageUrl = element.GetAttribute("onclick");
            var pgQueryParamStartIndex = pageUrl.IndexOf("pg=", StringComparison.Ordinal);
            var pgQueryParamEndIndex = pageUrl.IndexOf('&', pgQueryParamStartIndex);
            var pageNumber = int.Parse(pageUrl.Substring(pgQueryParamStartIndex + 3, pgQueryParamEndIndex - pgQueryParamStartIndex - 3));
            pages.Add(pageNumber);
        }

        return pages;
    }
}