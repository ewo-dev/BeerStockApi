using Xunit;
using Moq;
using BeerStockApi.Services;
using BeerStockApi.Repositories;
using BeerStockApi.Domain;
using BeerStockApi.Exceptions;
using Microsoft.Extensions.Logging;

namespace BeerStockApi.Tests.Services;

public class StockServiceTests
{
    private readonly Mock<IWholesalerBeerRepository> _mockRepository;
    private readonly Mock<ILogger<StockService>> _mockLogger;
    private readonly StockService _service;

    public StockServiceTests()
    {
        _mockRepository = new Mock<IWholesalerBeerRepository>();
        _mockLogger = new Mock<ILogger<StockService>>();
        _service = new StockService(_mockRepository.Object, _mockLogger.Object);
    }

    #region GetStockAsync Tests

    [Fact]
    public async Task GetStockAsync_ReturnsStock_WhenStockExists()
    {
        // Arrange
        var wholesalerId = 1;
        var beerId = 1;
        var stock = new WholesalerBeer
        {
            WholesalerId = wholesalerId,
            BeerId = beerId,
            Quantity = 50,
                        Beer = new Beer { Id = beerId, Name = "IPA", AlcoholByVolume = 6.5m, UnitPriceExcludingVat = 2.50m, BrewerId = 1 },
            Wholesaler = new Wholesaler { Id = wholesalerId, Name = "Test Wholesaler" }
        };

        _mockRepository.Setup(r => r.GetStockAsync(wholesalerId, beerId))
            .ReturnsAsync(stock);

        // Act
        var result = await _service.GetStockAsync(wholesalerId, beerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(wholesalerId, result.Id);
        Assert.Equal(50, result.Quantity);
    }

    [Fact]
    public async Task GetStockAsync_ThrowsException_WhenStockNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetStockAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((WholesalerBeer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _service.GetStockAsync(999, 999));
    }

    #endregion

    #region GetStocksByWholesalerAsync Tests

    [Fact]
    public async Task GetStocksByWholesalerAsync_ReturnsEmptyList_WhenNoStockExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByWholesalerIdAsync(It.IsAny<int>()))
            .ReturnsAsync([]);

        // Act
        var result = await _service.GetStocksByWholesalerAsync(1);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStocksByWholesalerAsync_ReturnsStocks_WhenStocksExist()
    {
        // Arrange
        var wholesalerId = 1;
        var stocks = new List<WholesalerBeer>
        {
            new() { WholesalerId = wholesalerId, BeerId = 1, Quantity = 50, Beer = new Beer { Id = 1, Name = "IPA", AlcoholByVolume = 6.5m, UnitPriceExcludingVat = 2.50m, BrewerId = 1 }, Wholesaler = new Wholesaler { Id = wholesalerId, Name = "Test Wholesaler" } },
            new() { WholesalerId = wholesalerId, BeerId = 2, Quantity = 30, Beer = new Beer { Id = 2, Name = "Lager", AlcoholByVolume = 4.5m, UnitPriceExcludingVat = 2.00m, BrewerId = 1 }, Wholesaler = new Wholesaler { Id = wholesalerId, Name = "Test Wholesaler" } }
        };

        _mockRepository.Setup(r => r.GetByWholesalerIdAsync(wholesalerId))
            .ReturnsAsync(stocks);

        // Act
        var result = await _service.GetStocksByWholesalerAsync(wholesalerId);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region GetWholesalersForBeerAsync Tests

    [Fact]
    public async Task GetWholesalersForBeerAsync_ReturnsWholesalers_WhenBeerIsAvailable()
    {
        // Arrange
        var beerId = 1;
        var stocks = new List<WholesalerBeer>
        {
            new() { WholesalerId = 1, BeerId = beerId, Quantity = 50, Beer = new Beer { Id = beerId, Name = "IPA", AlcoholByVolume = 6.5m, UnitPriceExcludingVat = 2.50m, BrewerId = 1 }, Wholesaler = new Wholesaler { Id = 1, Name = "Wholesaler A" } },
            new() { WholesalerId = 2, BeerId = beerId, Quantity = 30, Beer = new Beer { Id = beerId, Name = "IPA", AlcoholByVolume = 6.5m, UnitPriceExcludingVat = 2.50m, BrewerId = 1 }, Wholesaler = new Wholesaler { Id = 2, Name = "Wholesaler B" } }
        };

        _mockRepository.Setup(r => r.GetByBeerIdAsync(beerId))
            .ReturnsAsync(stocks);

        // Act
        var result = await _service.GetWholesalersForBeerAsync(beerId);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region UpdateStockAsync Tests

    [Fact]
    public async Task UpdateStockAsync_UpdatesStock_WithValidQuantity()
    {
        // Arrange
        var wholesalerId = 1;
        var beerId = 1;
        var newQuantity = 100;
        var existingStock = new WholesalerBeer
        {
                        Beer = new Beer { Id = beerId, Name = "IPA", AlcoholByVolume = 6.5m, UnitPriceExcludingVat = 2.50m, BrewerId = 1 },
            WholesalerId = wholesalerId,
            BeerId = beerId,
            Quantity = 50,
            Wholesaler = new Wholesaler { Id = wholesalerId, Name = "Test Wholesaler" }
        };

        _mockRepository.Setup(r => r.GetStockAsync(wholesalerId, beerId))
            .ReturnsAsync(existingStock);
        _mockRepository.Setup(r => r.UpdateStockAsync(It.IsAny<WholesalerBeer>()))
            .ReturnsAsync(new WholesalerBeer 
            { 
                WholesalerId = wholesalerId, 
                BeerId = beerId, 
                Quantity = newQuantity,
                Beer = new Beer { Id = beerId, Name = "IPA", AlcoholByVolume = 6.5m, UnitPriceExcludingVat = 2.50m, BrewerId = 1 },
                Wholesaler = new Wholesaler { Id = wholesalerId, Name = "Test Wholesaler" }
            });

        // Act
        var result = await _service.UpdateStockAsync(wholesalerId, beerId, newQuantity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newQuantity, result.Quantity);
        _mockRepository.Verify(r => r.UpdateStockAsync(It.IsAny<WholesalerBeer>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStockAsync_ThrowsException_WhenQuantityIsNegative()
    {
        // Arrange
        var wholesalerId = 1;
        var beerId = 1;
        var negativeQuantity = -5;

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _service.UpdateStockAsync(wholesalerId, beerId, negativeQuantity));
    }

    [Fact]
    public async Task UpdateStockAsync_ThrowsException_WhenStockNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetStockAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((WholesalerBeer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => 
            _service.UpdateStockAsync(999, 999, 100));
    }

    #endregion
}
