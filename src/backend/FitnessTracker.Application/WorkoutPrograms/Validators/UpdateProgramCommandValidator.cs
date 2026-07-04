using FluentValidation;
using FitnessTracker.Application.WorkoutPrograms.Commands;

namespace FitnessTracker.Application.WorkoutPrograms.Validators;

public class UpdateProgramCommandValidator : AbstractValidator<UpdateProgramCommand>
{
    public UpdateProgramCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Program name cannot be empty.")
            .MaximumLength(100).WithMessage("Program name cannot exceed 100 characters.");

        RuleFor(x => x.ProgramDays)
            .NotEmpty().WithMessage("At least one program day is required.");

        RuleForEach(x => x.ProgramDays).ChildRules(day =>
        {
            day.RuleFor(d => d.Name).NotEmpty();

            day.RuleFor(d => d.Exercises)
                .NotEmpty().WithMessage("Each program day must have at least one exercise.");

            day.When(d => d.Exercises.Count != 0, () =>
            {
                day.RuleForEach(d => d.Exercises).ChildRules(ex =>
                {
                    ex.RuleFor(e => e.TargetSets).GreaterThan(0);
                    ex.RuleFor(e => e.TargetReps).GreaterThan(0);
                    ex.RuleFor(e => e.Order).GreaterThanOrEqualTo(0);
                });
            });
        });
    }
}
