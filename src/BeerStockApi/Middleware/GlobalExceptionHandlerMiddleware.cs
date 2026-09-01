using BeerStockApi.Exceptions;
using System.Net;
using System.Text.Json;

namespace BeerStockApi.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Une exception non gérée s'est produite: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case BusinessException businessException:
                // Check if it's a "not found" error
                bool isNotFound = businessException.Message.Contains("introuvable") 
                    || businessException.Message.Contains("not found")
                    || businessException.Message.Contains("n'existe pas");

                if (isNotFound)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Ressource Non Trouvée";
                    problemDetails.Status = StatusCodes.Status404NotFound;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Erreur Métier";
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                }
                problemDetails.Detail = businessException.Message;
                break;

            case ValidationException validationException:
                response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                problemDetails.Title = "Erreur de Validation";
                problemDetails.Detail = validationException.Message;
                problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                break;

            default:
                response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Erreur Interne du Serveur";
                problemDetails.Detail = "Une erreur inattendue s'est produite. Veuillez réessayer plus tard.";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                break;
        }

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return response.WriteAsync(json);
    }
}

public class ProblemDetails
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
}
