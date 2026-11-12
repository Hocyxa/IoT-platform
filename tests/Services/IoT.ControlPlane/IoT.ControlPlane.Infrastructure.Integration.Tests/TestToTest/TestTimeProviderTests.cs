namespace IoT.ControlPlane.Infrastructure.Integration.Tests.TestToTest;

public sealed class TestTimeProviderTests
{
    [Fact]
    public void GetUtcNow_ShouldReturnIncreasingTimestamps()
    {
        // Arrange
        var initialTime = new DateTimeOffset(
            2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var timeProvider = new TestTimeProvider(initialTime);

        // Act
        var first = timeProvider.GetUtcNow();
        var second = timeProvider.GetUtcNow();
        var third = timeProvider.GetUtcNow();

        // Assert
        Assert.Equal(initialTime, first);
        Assert.Equal(initialTime.AddSeconds(1), second);
        Assert.Equal(initialTime.AddSeconds(2), third);
    }
}
