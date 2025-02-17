namespace GroceryScrapper.Scrapper;

public interface ICategoryDataCollector
{
    GroceryStore GroceryStore { get; }
    string JobOwner { get; }
    
    Task InitializeAsync(ChromeDriver driver, GroceryStore groceryStore);

    Task CollectDataAsync();
}