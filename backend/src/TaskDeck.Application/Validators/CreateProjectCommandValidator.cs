using FluentValidation;
using TaskDeck.Application.Commands.Projects;

namespace TaskDeck.Application.Validators;

/// <summary>
/// Validator for CreateProjectCommand
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(100).WithMessage("Project name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Color)
            .MaximumLength(20).WithMessage("Color must not exceed 20 characters");

        RuleFor(x => x.Icon)
            .MaximumLength(50).WithMessage("Icon must not exceed 50 characters");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Owner ID is required");
    }
}
