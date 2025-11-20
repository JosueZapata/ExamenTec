namespace ExamenTec.Application.DTOs.Product;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? StoreName { get; set; }
    public DateTime CreatedDate { get; set; }
}

