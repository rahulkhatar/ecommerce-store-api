using ECommerce.Domain.Exceptions;

namespace ECommerce.API.Middleware;

public class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, body) = exception switch
        {
            NotFoundException nf => (StatusCodes.Status404NotFound, (object)new { message = nf.Message }),
            ValidationException ve => (StatusCodes.Status400BadRequest, (object)new { message = ve.Message, errors = ve.Errors }),
            BusinessException be => (StatusCodes.Status400BadRequest, (object)new { message = be.Message }),
            _ => (StatusCodes.Status500InternalServerError, (object)new { message = "Internal server error" }),
        };

        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(body);
    }
}
