namespace GroceryScrapper.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<GroceryStore> GroceryStores { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasKey(p => p.Id);
        
        modelBuilder.Entity<Product>()
            .HasOne(p => p.GroceryStore)
            .WithMany(g => g.Products)
            .HasForeignKey(p => p.GroceryStoreId);
        
        modelBuilder.Entity<GroceryStore>()
            .HasKey(g => g.Id);

        modelBuilder.Entity<GroceryStore>()
            .Property(x => x.Name)
            .HasMaxLength(450);

        modelBuilder.Entity<GroceryStore>()
            .Property(x => x.Url)
            .HasMaxLength(1000);

        modelBuilder.Entity<Product>()
            .Property(x => x.Name)
            .HasMaxLength(450);
    }
}