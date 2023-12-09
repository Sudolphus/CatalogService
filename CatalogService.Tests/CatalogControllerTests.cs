using CatalogService.Data;
using CatalogService.Controllers;
using CatalogService.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace CatalogService.Tests;

public class CatalogControllerTests
{
    [Fact]
    public async Task GetProducts_ReturnsProducts()
    {
        // Arrange
        using var connection = Helper.GetSqliteConnection();
        var options = Helper.GetSqliteContextOptions(connection);
        AddTestData(options);

        using var context = new CatalogContext(options);
        var controller = new CatalogController(context);

        // Act
        var result = await controller.GetProducts();

        // Assert
        Assert.NotNull(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Value);
        Assert.Equal(2, products.Count());
        Assert.Collection(products,
            item => Assert.Equal(1, item.Id),
            item => Assert.Equal(2, item.Id)
        );
    }

    [Fact]
    public async Task GetProductById_ReturnsProduct_WhenExists()
    {
        // Arrange
        using var connection = Helper.GetSqliteConnection();
        var options = Helper.GetSqliteContextOptions(connection);
        AddTestData(options);

        using var context = new CatalogContext(options);
        var controller = new CatalogController(context);

        // Act
        var product = (await controller.GetProductById(1)).Value;

        // Assert
        Assert.IsType<Product>(product);
        Assert.Equal("Product 1", product.Name);
    }

    [Fact]
    public async Task GetProductById_ReturnsNull_WhenProductDoesntExist()
    {
        // Arrange
        using var connection = Helper.GetSqliteConnection();
        var options = Helper.GetSqliteContextOptions(connection);
        AddTestData(options);

        using var context = new CatalogContext(options);
        var controller = new CatalogController(context);

        // Act
        var product = (await controller.GetProductById(5)).Value;

        // Assert
        Assert.Null(product);
    }


    #region Helpers

    private static void AddTestData(DbContextOptions<CatalogContext> options)
    {
        using var context = new CatalogContext(options);
        context.Products.Add(new Product { Id = 1, Name = "Product 1", Price = 9.99M, Inventory = 5 });
        context.Products.Add(new Product { Id = 2, Name = "Product 2", Price = 15.99M, Inventory = 10 });
        context.SaveChanges();
    }

    #endregion
}