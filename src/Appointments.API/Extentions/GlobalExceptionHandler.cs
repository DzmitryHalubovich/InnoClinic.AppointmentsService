using Appointments.Domain.Errors;
using Microsoft.AspNetCore.Diagnostics;

namespace Appointments.API.Extentions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, 
        Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";

        var excDetails = exception switch
        {
            NotFoundException => (Detail: exception.Message, StatusCode: StatusCodes.Status404NotFound),
            _ => (Detail: exception.Message, StatusCode: StatusCodes.Status500InternalServerError)
        };

        httpContext.Response.StatusCode = excDetails.StatusCode;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Title = "An error occured",
                Detail = excDetails.Detail,
                Type = exception.GetType().Name,
                Status = excDetails.StatusCode
            },
            Exception = exception
        });
    }
}
