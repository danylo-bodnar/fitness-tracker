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
            .Must(days => days.Count <= 4)
            .WithMessage("A program cannot contain more than 4 days.");

        RuleForEach(x => x.ProgramDays).ChildRules(day =>
        {
            day.RuleFor(d => d.Name).NotEmpty();
            day.RuleFor(d => d.Order).GreaterThanOrEqualTo(0);

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
