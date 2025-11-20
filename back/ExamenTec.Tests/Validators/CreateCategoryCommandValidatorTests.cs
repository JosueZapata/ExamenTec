using ExamenTec.Application.Features.Categories.CreateCategory;
using FluentValidation.TestHelper;

namespace ExamenTec.Tests.Validators;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryCommandValidatorTests()
    {
        _validator = new CreateCategoryCommandValidator();
    }

    [Fact]
    public void Deberia_Tener_Error_Cuando_Nombre_Esta_Vacio()
    {
        var command = new CreateCategoryCommand("", "Descripción");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Deberia_Tener_Error_Cuando_Nombre_Excede_Longitud()
    {
        var longName = new string('A', 201);
        var command = new CreateCategoryCommand(longName, "Descripción");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void No_Deberia_Tener_Error_Cuando_Nombre_Es_Valido()
    {
        var command = new CreateCategoryCommand("Nombre Válido", "Descripción");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void No_Deberia_Tener_Error_Cuando_Descripcion_Es_Nula()
    {
        var command = new CreateCategoryCommand("Nombre Válido", null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}

