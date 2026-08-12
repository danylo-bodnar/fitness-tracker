using FluentValidation;
using FitnessTracker.Application.WorkoutSessions.Commands;

namespace FitnessTracker.Application.WorkoutSessions.Validators;

public class LogWorkoutSessionCommandValidator : AbstractValidator<LogWorkoutSessionCommand>
{
    public LogWorkoutSessionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id cannot be empty.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Workout date is required.");

        RuleFor(x => x.Exercises)
            .NotEmpty().WithMessage("A workout must contain at least one exercise.");

        RuleForEach(x => x.Exercises).ChildRules(exercise =>
        {
            exercise.RuleFor(e => e.ExerciseId)
                .NotEmpty().WithMessage("Exercise id cannot be empty.");

            exercise.RuleFor(e => e.ExerciseName)
                .NotEmpty().WithMessage("Exercise name cannot be empty.")
                .MaximumLength(100).WithMessage("Exercise name cannot exceed 100 characters.");

            exercise.RuleFor(e => e.Sets)
                .NotEmpty().WithMessage("An exercise must contain at least one set.");

            exercise.RuleForEach(e => e.Sets).ChildRules(set =>
            {
                set.RuleFor(s => s.WeightKg)
                    .GreaterThan(0).WithMessage("Weight must be greater than 0.");

                set.RuleFor(s => s.Reps)
                    .GreaterThan(0).WithMessage("Reps must be greater than 0.");
            });
        });
    }
}