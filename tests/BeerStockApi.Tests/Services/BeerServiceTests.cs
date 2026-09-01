using Xunit;
using Moq;
using BeerStockApi.Services;
using BeerStockApi.Repositories;
using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;
using BeerStockApi.Domain;
using Microsoft.Extensions.Logging;

namespace BeerStockApi.Tests.Services;

public class BeerServiceTests
{
    private readonly Mock<IBeerRepository> _mockRepository;
    private readonly Mock<IBrewerRepository> _mockBrewerRepository;
    private readonly Mock<ILogger<BeerService>> _mockLogger;
    private readonly BeerService _service;

    public BeerServiceTests()
    {
        _mockRepository = new Mock<IBeerRepository>();
        _mockBrewerRepository = new Mock<IBrewerRepository>();
        _mockLogger = new Mock<ILogger<BeerService>>();
        _service = new BeerService(_mockRepository.Object, _mockBrewerRepository.Object, _mockLogger.Object);
    }

    #region GetAllBeersAsync Tests

    [Fact]
    public async Task GetAllBeersAsync_ReturnsEmptyList_WhenNoBeerExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Beer>());

        // Act
        var result = await _service.GetAllBeersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllBeersAsync_ReturnsBeersList_WhenBeersExist()
    {
        // Arrange
        var beers = new List<Beer>
        {
            new() { Id = 1, Name = "Test Beer", AlcoholByVolume = 5.0m, UnitPriceExcludingVat = 2.5m, BrewerId = 1, Brewer = new Brewer { Id = 1, Name = "Test Brewery" } }
        };
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(beers);

        // Act
        var result = await _service.GetAllBeersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Beer", result.First().Name);
    }

    #endregion

    #region GetBeerByIdAsync Tests

    [Fact]
    public async Task GetBeerByIdAsync_ReturnsBeer_WhenBeerExists()
    {
        // Arrange
        var beerId = 1;
        var beer = new Beer 
        { 
            Id = beerId, 
            Name = "Test Beer", 
            AlcoholByVolume = 5.0m, 
            UnitPriceExcludingVat = 2.5m, 
            BrewerId = 1,
            Brewer = new Brewer { Id = 1, Name = "Test Brewery" }
        };
        _mockRepository.Setup(r => r.GetByIdAsync(beerId))
            .ReturnsAsync(beer);

        // Act
        var result = await _service.GetBeerByIdAsync(beerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(beerId, result.Id);
        Assert.Equal("Test Beer", result.Name);
    }

    #endregion

    #region CreateBeerAsync Tests

    [Fact]
    public async Task CreateBeerAsync_CreatesBeer_WithValidRequest()
    {
        // Arrange
        var request = new CreateBeerRequest
        {
            Name = "New Beer",
            AlcoholByVolume = 6.5m,
            UnitPriceExcludingVat = 3.0m,
            BrewerId = 1
        };
        var createdBeer = new Beer
        {
            Id = 11,
            Name = request.Name,
            AlcoholByVolume = request.AlcoholByVolume,
            UnitPriceExcludingVat = request.UnitPriceExcludingVat,
            BrewerId = request.BrewerId,
            Brewer = new Brewer { Id = 1, Name = "Test Brewery" }
        };
        
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Beer>()))
            .ReturnsAsync(createdBeer);
        _mockBrewerRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Brewer { Id = 1, Name = "Test Brewery" });

        // Act
        var result = await _service.CreateBeerAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Beer", result.Name);
        Assert.Equal(6.5m, result.AlcoholByVolume);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Beer>()), Times.Once);
    }

    #endregion

    #region UpdateBeerAsync Tests

    [Fact]
    public async Task UpdateBeerAsync_UpdatesBeer_WhenBeerExists()
    {
        // Arrange
        var beerId = 1;
        var request = new UpdateBeerRequest
        {
            Name = "Updated Beer",
            AlcoholByVolume = 7.0m,
            UnitPriceExcludingVat = null,
            BrewerId = null
        };
        var existingBeer = new Beer
        {
            Id = beerId,
            Name = "Old Beer",
            AlcoholByVolume = 5.0m,
            UnitPriceExcludingVat = 2.5m,
            BrewerId = 1,
            Brewer = new Brewer { Id = 1, Name = "Test Brewery" }
        };
        var updatedBeer = new Beer
        {
            Id = beerId,
            Name = "Updated Beer",
            AlcoholByVolume = 7.0m,
            UnitPriceExcludingVat = 2.5m,
            BrewerId = 1,
            Brewer = new Brewer { Id = 1, Name = "Test Brewery" }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(beerId))
            .ReturnsAsync(existingBeer);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Beer>()))
            .ReturnsAsync(updatedBeer);

        // Act
        var result = await _service.UpdateBeerAsync(beerId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Beer", result.Name);
        Assert.Equal(7.0m, result.AlcoholByVolume);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Beer>()), Times.Once);
    }

    #endregion

    #region DeleteBeerAsync Tests

    [Fact]
    public async Task DeleteBeerAsync_DeletesBeer_WhenBeerExists()
    {
        // Arrange
        var beerId = 1;
        var beer = new Beer 
        { 
            Id = beerId, 
            Name = "Test Beer", 
            AlcoholByVolume = 5.0m, 
            UnitPriceExcludingVat = 2.5m, 
            BrewerId = 1 
        };
        
        _mockRepository.Setup(r => r.GetByIdAsync(beerId))
            .ReturnsAsync(beer);
        _mockRepository.Setup(r => r.DeleteAsync(beerId))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteBeerAsync(beerId);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(beerId), Times.Once);
    }

    #endregion
}
