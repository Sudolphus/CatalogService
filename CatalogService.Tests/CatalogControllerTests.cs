using CatalogService.Data;
using CatalogService.Controllers;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace CatalogService.Tests;

public class CatalogControllerTests
{
    [Fact]
    public async Task GetProducts_ReturnsProducts()
    {
        using var connection = Helper.GetSqliteConnection();
        var options = Helper.GetSqliteContextOptions(connection);
        AddTestData(options);

        using var context = new CatalogContext(options);
        var controller = new CatalogController(context);
        var result = await controller.GetProducts();
        var products = result.Value;
        Assert.Equal(2, products.Count());
        Assert.Collection(products,
            item => Assert.Equal(1, item.Id),
            item => Assert.Equal(2, item.Id)
        );
    }

    private static void AddTestData(DbContextOptions<CatalogContext> options)
    {
        using var context = new CatalogContext(options);
        context.Products.Add(new Product { Id = 1, Name = "Product 1", Price = 9.99M, Inventory = 5 });
        context.Products.Add(new Product { Id = 2, Name = "Product 2", Price = 15.99M, Inventory = 10 });
        context.SaveChanges();
    }
}