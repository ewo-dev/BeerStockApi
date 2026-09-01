using Xunit;
using Moq;
using BeerStockApi.Contracts.Requests;
using BeerStockApi.Domain;
using BeerStockApi.Exceptions;
using BeerStockApi.Repositories;
using BeerStockApi.Services;
using Microsoft.Extensions.Logging;

namespace BeerStockApi.Tests.Services;

public class QuoteServiceTests
{
    private readonly Mock<IBeerRepository> _beerRepository;
    private readonly Mock<IWholesalerBeerRepository> _wholesalerBeerRepository;
    private readonly Mock<ILogger<QuoteService>> _logger;
    private readonly QuoteService _service;

    public QuoteServiceTests()
    {
        _beerRepository = new Mock<IBeerRepository>();
        _wholesalerBeerRepository = new Mock<IWholesalerBeerRepository>();
        _logger = new Mock<ILogger<QuoteService>>();
        _service = new QuoteService(_beerRepository.Object, _wholesalerBeerRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task GenerateQuoteAsync_ReturnsQuote_WithDiscountsAndVat()
    {
        // Arrange
        var request = new CreateQuoteRequest
        {
            WholesalerId = 1,
            Lines =
            [
                new QuoteLineRequest { BeerId = 1, Quantity = 9 },
                new QuoteLineRequest { BeerId = 2, Quantity = 10 },
                new QuoteLineRequest { BeerId = 3, Quantity = 20 }
            ]
        };
        var beers = new[]
        {
            CreateBeer(1, "IPA", 2.00m),
            CreateBeer(2, "Lager", 3.00m),
            CreateBeer(3, "Stout", 5.00m)
        };

        SetupBeers(beers);
        _wholesalerBeerRepository.Setup(r => r.GetByWholesalerIdAsync(request.WholesalerId))
            .ReturnsAsync(
            [
                CreateStock(request.WholesalerId, beers[0], 50),
                CreateStock(request.WholesalerId, beers[1], 50),
                CreateStock(request.WholesalerId, beers[2], 50)
            ]);

        // Act
        var result = await _service.GenerateQuoteAsync(request);

        // Assert
        Assert.Equal(3, result.Lines.Count);
        Assert.Equal(0m, result.Lines[0].DiscountPercent);
        Assert.Equal(10m, result.Lines[1].DiscountPercent);
        Assert.Equal(20m, result.Lines[2].DiscountPercent);
        Assert.Equal(18.00m, result.Lines[0].LineTotal);
        Assert.Equal(27.00m, result.Lines[1].LineTotal);
        Assert.Equal(80.00m, result.Lines[2].LineTotal);
        Assert.Equal(125.00m, result.SubTotal);
        Assert.Equal(26.25m, result.VAT);
        Assert.Equal(151.25m, result.FinalTotal);
    }

    [Fact]
    public async Task GenerateQuoteAsync_ThrowsValidationException_WhenOrderIsEmpty()
    {
        // Arrange
        var request = new CreateQuoteRequest
        {
            WholesalerId = 1,
            Lines = []
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.GenerateQuoteAsync(request));
    }

    [Fact]
    public async Task GenerateQuoteAsync_ThrowsValidationException_WhenBeerIsDuplicated()
    {
        // Arrange
        var request = new CreateQuoteRequest
        {
            WholesalerId = 1,
            Lines =
            [
                new QuoteLineRequest { BeerId = 1, Quantity = 1 },
                new QuoteLineRequest { BeerId = 1, Quantity = 2 }
            ]
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.GenerateQuoteAsync(request));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GenerateQuoteAsync_ThrowsValidationException_WhenQuantityIsNotPositive(int quantity)
    {
        // Arrange
        var request = new CreateQuoteRequest
        {
            WholesalerId = 1,
            Lines = [new QuoteLineRequest { BeerId = 1, Quantity = quantity }]
        };
        var beer = CreateBeer(1, "IPA", 2.00m);

        SetupBeers(beer);
        _wholesalerBeerRepository.Setup(r => r.GetByWholesalerIdAsync(request.WholesalerId))
            .ReturnsAsync([CreateStock(request.WholesalerId, beer, 10)]);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.GenerateQuoteAsync(request));
    }

    [Fact]
    public async Task GenerateQuoteAsync_ThrowsBusinessException_WhenBeerDoesNotExist()
    {
        // Arrange
        var request = new CreateQuoteRequest
        {
            WholesalerId = 1,
            Lines = [new QuoteLineRequest { BeerId = 99, Quantity = 1 }]
        };

        _beerRepository.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Beer?)null);
        _wholesalerBeerRepository.Setup(r => r.GetByWholesalerIdAsync(request.WholesalerId))
            .ReturnsAsync([]);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _service.GenerateQuoteAsync(request));
    }

    [Fact]
    public async Task GenerateQuoteAsync_ThrowsBusinessException_WhenWholesalerDoesNotSellBeer()
    {
        // Arrange
        var request = new CreateQuoteRequest
        {
            WholesalerId = 1,
            Lines = [new QuoteLineRequest { BeerId = 1, Quantity = 1 }]
        };
        var beer = CreateBeer(1, "IPA", 2.00m);

        SetupBeers(beer);
        _wholesalerBeerRepository.Setup(r => r.GetByWholesalerIdAsync(request.WholesalerId))
            .ReturnsAsync([]);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _service.GenerateQuoteAsync(request));
    }

    [Fact]
    public async Task GenerateQuoteAsync_ThrowsBusinessException_WhenStockIsInsufficient()
    {
        // Arrange
        var request = new CreateQuoteRequest
        {
            WholesalerId = 1,
            Lines = [new QuoteLineRequest { BeerId = 1, Quantity = 11 }]
        };
        var beer = CreateBeer(1, "IPA", 2.00m);

        SetupBeers(beer);
        _wholesalerBeerRepository.Setup(r => r.GetByWholesalerIdAsync(request.WholesalerId))
            .ReturnsAsync([CreateStock(request.WholesalerId, beer, 10)]);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _service.GenerateQuoteAsync(request));
    }

    private void SetupBeers(params Beer[] beers)
    {
        foreach (var beer in beers)
        {
            _beerRepository.Setup(r => r.GetByIdAsync(beer.Id))
                .ReturnsAsync(beer);
        }
    }

    private static Beer CreateBeer(int id, string name, decimal unitPrice)
    {
        return new Beer
        {
            Id = id,
            Name = name,
            AlcoholByVolume = 5.0m,
            UnitPriceExcludingVat = unitPrice,
            BrewerId = 1,
            Brewer = new Brewer { Id = 1, Name = "Test Brewery" }
        };
    }

    private static WholesalerBeer CreateStock(int wholesalerId, Beer beer, int quantity)
    {
        return new WholesalerBeer
        {
            WholesalerId = wholesalerId,
            Wholesaler = new Wholesaler { Id = wholesalerId, Name = "Test Wholesaler" },
            BeerId = beer.Id,
            Beer = beer,
            Quantity = quantity
        };
    }
}
