using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Events;
using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.UnitTests.Aggregates;

public class WorkoutSessionTests
{
    private static readonly UserId UserId = new(Guid.NewGuid());
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public void Create_ReturnsSessionWithTodayDate()
    {
        var session = WorkoutSession.Create(UserId, Today);

        Assert.Equal(Today, session.Date);
        Assert.Equal(UserId, session.UserId);
        Assert.False(session.Id.Value == Guid.Empty);
    }

    [Fact]
    public void Create_FiresWorkoutLoggedEvent()
    {
        var session = WorkoutSession.Create(UserId, Today);

        var events = session.PopEvents();

        Assert.Single(events);
        Assert.IsType<WorkoutLogged>(events.First());
    }

    [Fact]
    public void AddExercise_WithUniqueName_AddsExercise()
    {
        var session = WorkoutSession.Create(UserId, Today);

        var exercise = session.AddExercise(Guid.NewGuid(), new ExerciseName("squat"), Today);

        Assert.Single(session.Exercises);
        Assert.Equal(exercise, session.Exercises[0]);
    }

    [Fact]
    public void AddExercise_FiresExerciseAddedEvent()
    {
        var session = WorkoutSession.Create(UserId, Today);
        session.PopEvents(); // clear creation event

        session.AddExercise(Guid.NewGuid(), new ExerciseName("squat"), Today);

        var events = session.PopEvents();
        Assert.Single(events);
        Assert.IsType<ExerciseAdded>(events.First());
    }

    [Fact]
    public void AddExercise_WithDuplicateName_ThrowsDuplicateExerciseException()
    {
        var session = WorkoutSession.Create(UserId, Today);

        session.AddExercise(Guid.NewGuid(), new ExerciseName("squat"), Today);

        Assert.Throws<DuplicateExerciseException>(() =>
            session.AddExercise(Guid.NewGuid(), new ExerciseName("squat"), Today));
    }

    [Fact]
    public void AddExercise_WithPastDate_ThrowsPastSessionModificationException()
    {
        var session = WorkoutSession.Create(UserId, Today);
        var yesterday = Today.AddDays(-1);

        Assert.Throws<PastSessionModificationException>(() =>
            session.AddExercise(Guid.NewGuid(), new ExerciseName("squat"), yesterday));
    }

    [Fact]
    public void PopEvents_ReturnsEventsAndClears()
    {
        var session = WorkoutSession.Create(UserId, Today);

        var first = session.PopEvents();
        Assert.NotEmpty(first);

        var second = session.PopEvents();
        Assert.Empty(second);
    }

    [Fact]
    public void Exercises_IsReadOnly()
    {
        var session = WorkoutSession.Create(UserId, Today);

        Assert.IsAssignableFrom<IReadOnlyList<ExerciseLog>>(session.Exercises);
    }
}
