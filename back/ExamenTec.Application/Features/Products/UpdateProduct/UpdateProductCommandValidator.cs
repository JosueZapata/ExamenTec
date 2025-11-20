using FluentValidation;

namespace ExamenTec.Application.Features.Products.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del producto es requerido")
            .MaximumLength(200).WithMessage("El nombre del producto no debe exceder 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción del producto no debe exceder 500 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("El precio del producto no puede ser negativo");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock del producto no puede ser negativo");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("El ID de la categoría es requerido");
    }
}

