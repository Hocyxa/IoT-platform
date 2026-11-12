public sealed class TestTimeProvider : TimeProvider
{
    private DateTimeOffset _currentTime;

    public TestTimeProvider(DateTimeOffset? initialTime = null)
    {
        _currentTime = initialTime
            ?? new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
    }

    public override DateTimeOffset GetUtcNow()
    {
        var result = _currentTime;
        _currentTime = _currentTime.AddSeconds(1);

        return result;
    }
}
