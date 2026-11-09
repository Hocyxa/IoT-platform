using IoT.ControlPlane.Application.Modules.Sites;
using IoT.ControlPlane.Application.Modules.Sites.Abstractions;
using IoT.ControlPlane.Application.Modules.Sites.Contracts;
using IoT.ControlPlane.Domain.Modules.Sites;

using NSubstitute;

namespace IoT.ControlPlane.Application.Tests.Modules.Sites;

public sealed class SiteServiceTests
{
    private readonly ISiteRepository _siteRepository =
        Substitute.For<ISiteRepository>();

    private readonly IOrganizationExistenceChecker _organizationChecker =
        Substitute.For<IOrganizationExistenceChecker>();

    private readonly TimeProvider _timeProvider =
        Substitute.For<TimeProvider>();

    private SiteService CreateService()
    {
        return new SiteService(
            _siteRepository,
            _organizationChecker,
            _timeProvider);
    }
    private static Site CreateSite()
    {
        return Site.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test site",
            "Test address",
            DateTimeOffset.UtcNow);
    }

    private static Site CreateSite(string name, string address)
    {
        return Site.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            name,
            address,
            DateTimeOffset.UtcNow);
    }
    private static Site CreateSite(
        Guid organizationId,
        string name,
        string address)
    {
        return Site.Create(
            Guid.NewGuid(),
            organizationId,
            name,
            address,
            DateTimeOffset.UtcNow);
    }
    [Fact]
    public async Task CreateAsync_WhenOrganizationExists_CreatesSite()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        _organizationChecker
            .ExistsAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(true);

        _timeProvider.GetUtcNow().Returns(now);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(
            organizationId,
            "Test site",
            "Test address",
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(organizationId, result.OrganizationId);
        Assert.Equal("Test site", result.Name);
        Assert.Equal("Test address", result.Address);
        Assert.Equal(now, result.CreatedAtUtc);
        Assert.Equal(now, result.UpdatedAtUtc);

        await _organizationChecker.Received(1)
            .ExistsAsync(organizationId, Arg.Any<CancellationToken>());

        await _siteRepository.Received(1)
            .AddAsync(
                Arg.Is<Site>(site =>
                    site.Id == result.Id &&
                    site.OrganizationId == organizationId &&
                    site.Name == "Test site" &&
                    site.Address == "Test address"),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WhenOrganizationDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var organizationId = Guid.NewGuid();

        _organizationChecker
            .ExistsAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(false);

        var service = CreateService();

        // Act
        var act = () => service.CreateAsync(
            organizationId,
            "Test site",
            "Test address",
            CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(act);

        await _organizationChecker.Received(1)
            .ExistsAsync(organizationId, Arg.Any<CancellationToken>());

        await _siteRepository.DidNotReceive()
            .AddAsync(
                Arg.Any<Site>(),
                Arg.Any<CancellationToken>());

        _timeProvider.DidNotReceive().GetUtcNow();
    }

    [Fact]
    public async Task GetByIdAsync_WhenSiteExists_ReturnsSiteDto()
    {
        // Arrange
        var site = CreateSite();

        _siteRepository
            .GetByIdAsync(site.Id, Arg.Any<CancellationToken>())
            .Returns(site);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(
            site.Id,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(site.Id, result.Id);
        Assert.Equal(site.OrganizationId, result.OrganizationId);
        Assert.Equal(site.Name, result.Name);
        Assert.Equal(site.Address, result.Address);
        Assert.Equal(site.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(site.UpdatedAtUtc, result.UpdatedAtUtc);

        await _siteRepository.Received(1)
            .GetByIdAsync(site.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_WhenSiteDoesNotExist_ReturnsNull()
    {
        // Arrange
        var siteId = Guid.NewGuid();

        _siteRepository
            .GetByIdAsync(siteId, Arg.Any<CancellationToken>())
            .Returns((Site?)null);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(
            siteId,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        await _siteRepository.Received(1)
            .GetByIdAsync(siteId, Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task GetAllAsync_WhenSitesExist_ReturnsSiteDtos()
    {
        // Arrange
        var sites = new List<Site>
        {
            CreateSite("Site 1", "Address 1"),
            CreateSite("Site 2", "Address 2"),
        };

        _siteRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sites);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(
            CancellationToken.None);

        // Assert
        Assert.Equal(sites.Count, result.Count);

        for (var i = 0; i < sites.Count; i++)
        {
            Assert.Equal(sites[i].Id, result[i].Id);
            Assert.Equal(sites[i].OrganizationId, result[i].OrganizationId);
            Assert.Equal(sites[i].Name, result[i].Name);
            Assert.Equal(sites[i].Address, result[i].Address);
            Assert.Equal(sites[i].CreatedAtUtc, result[i].CreatedAtUtc);
            Assert.Equal(sites[i].UpdatedAtUtc, result[i].UpdatedAtUtc);
        }

        await _siteRepository.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_WhenNoSitesExist_ReturnsEmptyList()
    {
        // Arrange
        _siteRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Site>());

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(
            CancellationToken.None);

        // Assert
        Assert.Empty(result);

        await _siteRepository.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task GetByOrganizationIdAsync_WhenSitesExist_ReturnsSiteDtos()
    {
        // Arrange
        var organizationId = Guid.NewGuid();

        var sites = new List<Site>
        {
            CreateSite(organizationId, "Site 1", "Address 1"),
            CreateSite(organizationId, "Site 2", "Address 2"),
        };

        _siteRepository
            .GetByOrganizationIdAsync(
                organizationId,
                Arg.Any<CancellationToken>())
            .Returns(sites);

        var service = CreateService();

        // Act
        var result = await service.GetByOrganizationIdAsync(
            organizationId,
            CancellationToken.None);

        // Assert
        Assert.Equal(sites.Count, result.Count);

        for (var i = 0; i < sites.Count; i++)
        {
            Assert.Equal(sites[i].Id, result[i].Id);
            Assert.Equal(sites[i].OrganizationId, result[i].OrganizationId);
            Assert.Equal(sites[i].Name, result[i].Name);
            Assert.Equal(sites[i].Address, result[i].Address);
            Assert.Equal(sites[i].CreatedAtUtc, result[i].CreatedAtUtc);
            Assert.Equal(sites[i].UpdatedAtUtc, result[i].UpdatedAtUtc);
        }

        Assert.All(result, site =>
            Assert.Equal(organizationId, site.OrganizationId));

        await _siteRepository.Received(1)
            .GetByOrganizationIdAsync(
                organizationId,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByOrganizationIdAsync_WhenNoSitesExist_ReturnsEmptyList()
    {
        // Arrange
        var organizationId = Guid.NewGuid();

        _siteRepository
            .GetByOrganizationIdAsync(
                organizationId,
                Arg.Any<CancellationToken>())
            .Returns(new List<Site>());

        var service = CreateService();

        // Act
        var result = await service.GetByOrganizationIdAsync(
            organizationId,
            CancellationToken.None);

        // Assert
        Assert.Empty(result);

        await _siteRepository.Received(1)
            .GetByOrganizationIdAsync(
                organizationId,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WhenSiteExists_UpdatesSite()
    {
        // Arrange
        var site = CreateSite();
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(5);

        _siteRepository
            .GetByIdAsync(site.Id, Arg.Any<CancellationToken>())
            .Returns(site);

        _timeProvider
            .GetUtcNow()
            .Returns(updatedAt);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(
            site.Id,
            "Updated site",
            "Updated address",
            CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal("Updated site", site.Name);
        Assert.Equal("Updated address", site.Address);
        Assert.Equal(updatedAt, site.UpdatedAtUtc);

        await _siteRepository.Received(1)
            .GetByIdAsync(site.Id, Arg.Any<CancellationToken>());

        await _siteRepository.Received(1)
            .UpdateAsync(
                Arg.Is<Site>(updatedSite =>
                    updatedSite.Id == site.Id &&
                    updatedSite.Name == "Updated site" &&
                    updatedSite.Address == "Updated address" &&
                    updatedSite.UpdatedAtUtc == updatedAt),
                Arg.Any<CancellationToken>());

        _timeProvider.Received(1).GetUtcNow();
    }

    [Fact]
    public async Task UpdateAsync_WhenSiteDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var siteId = Guid.NewGuid();

        _siteRepository
            .GetByIdAsync(siteId, Arg.Any<CancellationToken>())
            .Returns((Site?)null);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(
            siteId,
            "Updated site",
            "Updated address",
            CancellationToken.None);

        // Assert
        Assert.False(result);

        await _siteRepository.Received(1)
            .GetByIdAsync(siteId, Arg.Any<CancellationToken>());

        await _siteRepository.DidNotReceive()
            .UpdateAsync(
                Arg.Any<Site>(),
                Arg.Any<CancellationToken>());

        _timeProvider.DidNotReceive().GetUtcNow();
    }
    [Fact]
    public async Task DeleteAsync_WhenSiteExists_DeletesSite()
    {
        // Arrange
        var site = CreateSite();

        _siteRepository
            .GetByIdAsync(site.Id, Arg.Any<CancellationToken>())
            .Returns(site);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(
            site.Id,
            CancellationToken.None);

        // Assert
        Assert.True(result);

        await _siteRepository.Received(1)
            .GetByIdAsync(site.Id, Arg.Any<CancellationToken>());

        await _siteRepository.Received(1)
            .DeleteAsync(
                Arg.Is<Site>(deletedSite =>
                    deletedSite.Id == site.Id),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenSiteDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var siteId = Guid.NewGuid();

        _siteRepository
            .GetByIdAsync(siteId, Arg.Any<CancellationToken>())
            .Returns((Site?)null);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(
            siteId,
            CancellationToken.None);

        // Assert
        Assert.False(result);

        await _siteRepository.Received(1)
            .GetByIdAsync(siteId, Arg.Any<CancellationToken>());

        await _siteRepository.DidNotReceive()
            .DeleteAsync(
                Arg.Any<Site>(),
                Arg.Any<CancellationToken>());
    }
}
