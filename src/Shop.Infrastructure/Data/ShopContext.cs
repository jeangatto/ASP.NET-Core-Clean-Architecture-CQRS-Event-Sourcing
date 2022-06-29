using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data;

public class ShopContext : DbContext
{
    /// <summary>
    /// Collation: define o conjunto de regras que o servidor irá utilizar para ordenação e comparação entre textos.
    /// Latin1_General_CI_AI: Configurado para ignorar o "Case Insensitive (CI)" e os acentos "Accent Insensitive (AI)".
    /// </summary>
    private const string SQLServerCollation = "Latin1_General_CI_AI";

    public ShopContext(DbContextOptions<ShopContext> options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .UseCollation(SQLServerCollation)
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
            .RemoveCascadeDeleteConvention();
}