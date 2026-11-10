
using IoT.ControlPlane.Domain.Modules.MonitoringRules;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Enums;
using IoT.ControlPlane.Domain.Modules.MonitoringRules.Events;

namespace IoT.ControlPlane.Domain.Tests.Modules.MonitoringRules;

public sealed class MonitoringRuleUpdateTests
{
    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 11, 10, 10, 0, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset UpdatedAtUtc =
        new(2026, 11, 10, 11, 0, 0, TimeSpan.Zero);

    private static MonitoringRule CreateRule()
    {
        return MonitoringRule.Create(
            Guid.NewGuid(),
            "Original name",
            "Original description",
            RuleSeverity.Information,
            CreatedAtUtc);
    }

    [Fact]
    public void UpdateDetails_WithValidValues_UpdatesAllProperties()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        rule.UpdateDetails(
            "Updated name",
            "Updated description",
            RuleSeverity.Critical,
            UpdatedAtUtc);

        // Assert
        Assert.Equal("Updated name", rule.Name);
        Assert.Equal("Updated description", rule.Description);
        Assert.Equal(RuleSeverity.Critical, rule.Severity);
        Assert.Equal(UpdatedAtUtc, rule.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateDetails_WithWhitespace_TrimsValues()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        rule.UpdateDetails(
            "  Updated name  ",
            "  Updated description  ",
            RuleSeverity.Warning,
            UpdatedAtUtc);

        // Assert
        Assert.Equal("Updated name", rule.Name);
        Assert.Equal("Updated description", rule.Description);
    }

    [Fact]
    public void UpdateDetails_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var rule = CreateRule();

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => rule.UpdateDetails(
                "   ",
                null,
                RuleSeverity.Warning,
                UpdatedAtUtc));

        // Assert
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithNameLongerThan200Characters_ThrowsArgumentException()
    {
        // Arrange
        var rule = CreateRule();
        var name = new string('a', 201);

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => rule.UpdateDetails(
                name,
                null,
                RuleSeverity.Warning,
                UpdatedAtUtc));

        // Assert
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithDescriptionLongerThan1000Characters_ThrowsArgumentException()
    {
        // Arrange
        var rule = CreateRule();
        var description = new string('a', 1001);

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => rule.UpdateDetails(
                "Updated name",
                description,
                RuleSeverity.Warning,
                UpdatedAtUtc));

        // Assert
        Assert.Equal("description", exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithInvalidSeverity_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var rule = CreateRule();
        var invalidSeverity = (RuleSeverity)999;

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => rule.UpdateDetails(
                "Updated name",
                null,
                invalidSeverity,
                UpdatedAtUtc));

        // Assert
        Assert.Equal("severity", exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithSameValues_DoesNotChangeUpdatedAtOrAddDomainEvent()
    {
        // Arrange
        var rule = CreateRule();
        rule.ClearDomainEvents();

        var originalUpdatedAt = rule.UpdatedAtUtc;
        var originalEventCount = rule.DomainEvents.Count;

        // Act
        rule.UpdateDetails(
            "Original name",
            "Original description",
            RuleSeverity.Information,
            UpdatedAtUtc);

        // Assert
        Assert.Equal(originalUpdatedAt, rule.UpdatedAtUtc);
        Assert.Equal(originalEventCount, rule.DomainEvents.Count);
    }

    [Fact]
    public void UpdateDetails_WhenSuccessful_AddsUpdatedDomainEvent()
    {
        // Arrange
        var rule = CreateRule();
        rule.ClearDomainEvents();

        // Act
        rule.UpdateDetails(
            "Updated name",
            "Updated description",
            RuleSeverity.Warning,
            UpdatedAtUtc);

        // Assert
        var domainEvent = Assert.Single(rule.DomainEvents);

        Assert.IsType<MonitoringRuleUpdatedDomainEvent>(domainEvent);

        var updatedEvent =
            (MonitoringRuleUpdatedDomainEvent)domainEvent;

        Assert.Equal(rule.Id, updatedEvent.MonitoringRuleId);
    }
}
