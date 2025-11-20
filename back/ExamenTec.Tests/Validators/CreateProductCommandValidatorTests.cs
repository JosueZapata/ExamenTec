using ExamenTec.Application.Features.Products.CreateProduct;
using FluentValidation.TestHelper;

namespace ExamenTec.Tests.Validators;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Deberia_Tener_Error_Cuando_Nombre_Esta_Vacio()
    {
        var command = new CreateProductCommand("", "Descripción", 100, 10, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Deberia_Tener_Error_Cuando_Precio_Negativo()
    {
        var command = new CreateProductCommand("Producto", "Descripción", -10, 10, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Deberia_Tener_Error_Cuando_Stock_Negativo()
    {
        var command = new CreateProductCommand("Producto", "Descripción", 100, -5, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Stock);
    }

    [Fact]
    public void Deberia_Tener_Error_Cuando_CategoryId_Vacio()
    {
        var command = new CreateProductCommand("Producto", "Descripción", 100, 10, Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void No_Deberia_Tener_Error_Cuando_Todos_Los_Campos_Son_Validos()
    {
        var command = new CreateProductCommand("Producto", "Descripción", 100, 10, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
        result.ShouldNotHaveValidationErrorFor(x => x.Stock);
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }
}

