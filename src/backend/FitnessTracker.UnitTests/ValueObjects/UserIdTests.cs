using FitnessTracker.Domain.ValueObjects;

namespace FitnessTracker.UnitTests.ValueObjects;

public class UserIdTests
{
    [Fact]
    public void Create_WrapsGuid()
    {
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        Assert.Equal(guid, userId.Value);
    }
}
