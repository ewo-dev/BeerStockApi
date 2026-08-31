using BeerStockApi.Contracts;
using BeerStockApi.Contracts.Requests;
using BeerStockApi.Exceptions;
using BeerStockApi.Repositories;

namespace BeerStockApi.Services;

public class QuoteService(
    IBeerRepository beerRepository,
    IWholesalerBeerRepository wholesalerBeerRepository,
    ILogger<QuoteService> logger) : IQuoteService
{
    private const decimal VAT_RATE = 0.21m; // 21% de TVA

    private readonly IBeerRepository _beerRepository = beerRepository;
    private readonly IWholesalerBeerRepository _wholesalerBeerRepository = wholesalerBeerRepository;
    private readonly ILogger<QuoteService> _logger = logger;

    public async Task<QuoteResponse> GenerateQuoteAsync(CreateQuoteRequest request)
    {
        if (request.Lines == null || request.Lines.Count == 0)
        {
            throw new ValidationException("La commande ne peut pas être vide.");
        }

        var beerIds = request.Lines.Select(l => l.BeerId).ToList();
        if (beerIds.Count != beerIds.Distinct().Count())
        {
            throw new ValidationException("Il ne peut pas y avoir de doublon de bière dans la commande.");
        }

        var stocks = await _wholesalerBeerRepository.GetByWholesalerIdAsync(request.WholesalerId);

        var quoteLines = new List<QuoteLineResponse>();
        decimal subTotal = 0;

        foreach (var line in request.Lines)
        {
            if (line.Quantity <= 0)
            {
                throw new ValidationException($"La quantité pour la bière {line.BeerId} doit être supérieure à 0.");
            }

            var beer = await _beerRepository.GetByIdAsync(line.BeerId) ?? throw new BusinessException($"La bière avec l'ID {line.BeerId} n'existe pas.");

            var stock = stocks.FirstOrDefault(s => s.BeerId == line.BeerId) ?? throw new BusinessException($"Le grossiste n'a pas la bière '{beer.Name}' (ID {line.BeerId}) en catalogne.");

            if (stock.Quantity < line.Quantity)
            {
                throw new BusinessException($"Stock insuffisant pour '{beer.Name}'. Demandé: {line.Quantity}, Disponible: {stock.Quantity}");
            }

            decimal unitPrice = beer.UnitPriceExcludingVat;
            decimal discountPercent = CalculateDiscount(line.Quantity);
            decimal priceAfterDiscount = unitPrice * (1 - discountPercent / 100);
            decimal lineTotal = priceAfterDiscount * line.Quantity;

            quoteLines.Add(new QuoteLineResponse
            {
                BeerId = beer.Id,
                BeerName = beer.Name,
                Quantity = line.Quantity,
                UnitPrice = unitPrice,
                DiscountPercent = discountPercent,
                LineTotal = lineTotal
            });

            subTotal += lineTotal;
        }

        decimal vat = subTotal * VAT_RATE;
        decimal finalTotal = subTotal + vat;

        _logger.LogInformation("Devis généré avec succès pour le grossiste {WholesalerId}. Total: {FinalTotal}",
            request.WholesalerId, finalTotal);

        return new QuoteResponse
        {
            Lines = quoteLines,
            SubTotal = subTotal,
            VAT = vat,
            FinalTotal = finalTotal
        };
    }

    private static decimal CalculateDiscount(int quantity)
    {
        if (quantity >= 20)
            return 20m;

        if (quantity >= 10)
            return 10m;

        return 0m;
    }
}
