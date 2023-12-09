using CatalogService.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Tests;

public static class Helper
{
  public static SqliteConnection GetSqliteConnection()
  {
    var connection = new SqliteConnection("DataSource=:memory:");
    connection.Open();

    return connection;
  }

  public static DbContextOptions<CatalogContext> GetSqliteContextOptions(SqliteConnection connection)
  {
    var options = new DbContextOptionsBuilder<CatalogContext>()
      .UseSqlite(connection)
      .Options;

    using (var context = new CatalogContext(options))
    {
      context.Database.EnsureCreated();
    }

    return options;
  }
}