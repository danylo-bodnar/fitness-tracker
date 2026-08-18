using FluentValidation;
using FitnessTracker.Application.WorkoutSessions.Queries;

namespace FitnessTracker.Application.WorkoutSessions.Validators;

public class GetWorkoutHistoryQueryValidator : AbstractValidator<GetWorkoutHistoryQuery>
{
    public GetWorkoutHistoryQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("PageSize must be between 1 and 50.");
    }
}