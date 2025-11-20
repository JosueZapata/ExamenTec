using ExamenTec.Application.DTOs.Category;
using ExamenTec.Application.DTOs.Product;
using ExamenTec.Domain.Entities;
using Mapster;

namespace ExamenTec.Application.Mappings;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Category, CategoryResponseDto>.NewConfig();

        TypeAdapterConfig<Product, ProductResponseDto>.NewConfig()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null);
    }
}

