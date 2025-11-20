using FluentValidation;

namespace ExamenTec.Application.Features.Categories.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la categoría es requerido")
            .MaximumLength(200).WithMessage("El nombre de la categoría no debe exceder 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción de la categoría no debe exceder 500 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

