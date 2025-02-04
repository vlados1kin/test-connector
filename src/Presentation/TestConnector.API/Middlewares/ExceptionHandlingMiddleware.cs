using Microsoft.AspNetCore.Mvc;

namespace TestConnector.API.Middlewares;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception occured: {Message}", exception.Message);

            var problemDetails = new ProblemDetails()
            {
                Status = exception switch
                {
                    _ => StatusCodes.Status500InternalServerError
                },
                Title = exception.Message
            };

            context.Response.StatusCode = problemDetails.Status.Value;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}