using CatalogService.Data;
using CatalogService.Controllers;
using CatalogService.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace CatalogService.Tests;

public class CatalogControllerTests
{
    private static readonly Product product1 = new() { Id = 1, Name = "Product 1", Price = 9.99M, Inventory = 5 };
    private static readonly Product product2 = new() { Id = 2, Name = "Product 2", Price = 15.99M, Inventory = 10 };

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

    [Fact]
    public async Task CreateProduct_CreatesAndAdds()
    {
        // Arrange
        using var connection = Helper.GetSqliteConnection();
        var options = Helper.GetSqliteContextOptions(connection);

        using var context = new CatalogContext(options);
        var controller = new CatalogController(context);
        string name = product1.Name;
        decimal price = product1.Price;
        int inventory = product1.Inventory;
        var newProduct = CreateProduct(name, price, inventory);
        
        // Act
        await controller.CreateProduct(newProduct);
        var product = (await controller.GetProducts()).Value?.SingleOrDefault();

        // Assert
        Assert.IsType<Product>(product);
        Assert.Equal(name, product.Name);
        Assert.Equal(price, product.Price);
        Assert.Equal(inventory, product.Inventory);
    }

    [Fact]
    public async Task UpdateProduct_UpdatesValues_WhenProductExists()
    {
        // Arrange
        using var connection = Helper.GetSqliteConnection();
        var options = Helper.GetSqliteContextOptions(connection);
        AddTestData(options);

        using var context = new CatalogContext(options);
        var controller = new CatalogController(context);
        string newName = "new name";
        decimal newPrice = 99.99M;
        int newInventory = 100;
        var edittedProduct = CreateProduct(newName, newPrice, newInventory);

        // Act
        await controller.UpdateProduct(1, edittedProduct);
        var product = (await controller.GetProductById(1)).Value;

        // Assert
        Assert.Equal(newName, product?.Name);
        Assert.Equal(newPrice, product?.Price);
        Assert.Equal(newInventory, product?.Inventory);
    }

    [Fact]
    public async Task UpdateInventory_IncrementsAndDecrements()
    {
        // Arrange
        using var connection = Helper.GetSqliteConnection();
        var options = Helper.GetSqliteContextOptions(connection);
        AddTestData(options);

        using var context = new CatalogContext(options);
        var controller = new CatalogController(context);
        int changeValue = 4;

        // Act
        await controller.UpdateInventory(1, changeValue);
        await controller.UpdateInventory(2, changeValue * -1);
        var products = (await controller.GetProducts()).Value;

        // Assert
        Assert.Equal(product1.Inventory + changeValue, products?.First().Inventory);
        Assert.Equal(product2.Inventory - changeValue, products?.Last().Inventory);
    }

    [Fact]
    public async Task UpdateInventory_DoesNotDecrementBelowZero()
    {
        // Arrange
        using var connection = Helper.GetSqliteConnection();
        var options = Helper.GetSqliteContextOptions(connection);
        AddTestData(options);

        using var context = new CatalogContext(options);
        var controller = new CatalogController(context);
        int changeValue = -100;

        // Act
        await controller.UpdateInventory(1, changeValue);
        var product = (await controller.GetProductById(1)).Value;

        // Assert
        Assert.Equal(product1.Inventory, product?.Inventory);
    }

    [Fact]
    public async Task DeleteProduct_RemovesProduct()
    {
        // Arrange
        using var connection = Helper.GetSqliteConnection();
        var options = Helper.GetSqliteContextOptions(connection);
        AddTestData(options);

        using var context = new CatalogContext(options);
        var controller = new CatalogController(context);

        // Act
        await controller.DeleteProduct(1);
        var product = (await controller.GetProductById(1)).Value;

        // Assert
        Assert.Null(product);
    }

    #region Helpers

    private static void AddTestData(DbContextOptions<CatalogContext> options)
    {
        using var context = new CatalogContext(options);
        context.Products.Add(product1);
        context.Products.Add(product2);
        context.SaveChanges();
    }

    private static Product CreateProduct(string name, decimal price, int inventory)
    {
        return new Product { Name = name, Price = price, Inventory = inventory };
    }

    #endregion
}
