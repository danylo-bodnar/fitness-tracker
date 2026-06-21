using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Events;
using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.UnitTests.Aggregates;

public class WorkoutSessionTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public void Create_ReturnsSessionWithTodayDate()
    {
        var session = WorkoutSession.Create(UserId, Today);

        Assert.Equal(Today, session.Date);
        Assert.Equal(UserId, session.UserId);
        Assert.False(session.Id == Guid.Empty);
    }

    [Fact]
    public void Create_FiresNoEvents()
    {
        var session = WorkoutSession.Create(UserId, Today);

        var events = session.PopEvents();

        Assert.Empty(events);
    }

    [Fact]
    public void AddExercise_WithUniqueName_AddsExercise()
    {
        var session = WorkoutSession.Create(UserId, Today);

        var exercise = session.AddExercise(Guid.NewGuid(), new ExerciseName("squat"));

        Assert.Single(session.Exercises);
        Assert.Equal(exercise, session.Exercises[0]);
    }

    [Fact]
    public void CompleteExercise_FiresExercisePerformedEvent()
    {
        var session = WorkoutSession.Create(UserId, Today);
        var exerciseLog = session.AddExercise(Guid.NewGuid(), new ExerciseName("squat"));
        exerciseLog.LogSet(new Weight(100), new Repetitions(5));
        exerciseLog.LogSet(new Weight(100), new Repetitions(5));

        session.CompleteExercise(exerciseLog);

        var events = session.PopEvents();
        var performed = Assert.Single(events);
        var ev = Assert.IsType<ExercisePerformed>(performed);
        Assert.Equal(session.Id, ev.SessionId);
        Assert.Equal(UserId, ev.UserId);
        Assert.Equal(exerciseLog.ExerciseId, ev.ExerciseId);
        Assert.Equal(2, ev.Sets.Count);
    }

    [Fact]
    public void AddExercise_WithDuplicateName_ThrowsDuplicateExerciseException()
    {
        var session = WorkoutSession.Create(UserId, Today);

        session.AddExercise(Guid.NewGuid(), new ExerciseName("squat"));

        Assert.Throws<DuplicateExerciseException>(() =>
            session.AddExercise(Guid.NewGuid(), new ExerciseName("squat")));
    }

    [Fact]
    public void PopEvents_ReturnsEventsAndClears()
    {
        var session = WorkoutSession.Create(UserId, Today);
        var exerciseLog = session.AddExercise(Guid.NewGuid(), new ExerciseName("squat"));
        exerciseLog.LogSet(new Weight(100), new Repetitions(5));
        session.CompleteExercise(exerciseLog);

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
