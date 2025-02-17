namespace GroceryScrapper.Data;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int GroceryStoreId { get; set; }
    public GroceryStore? GroceryStore { get; set; }
}