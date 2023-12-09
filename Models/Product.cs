using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogService.Models;

public class Product
{
  public int Id { get; set; }

  [StringLength(60, MinimumLength = 3)]
  [Required]
  public string Name { get; set; } = string.Empty;
  
  [Range(0, int.MaxValue)]
  [DataType(DataType.Currency)]
  [Column(TypeName = "decimal(18,2)")]
  [Required]
  public decimal Price { get; set; } = 0M;

  [Range(0, int.MaxValue)]
  [Required]
  public int Inventory { get; set; } = 0;
}