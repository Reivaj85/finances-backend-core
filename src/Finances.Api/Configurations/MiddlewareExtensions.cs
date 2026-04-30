using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Finances.Api.Configurations;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            catch (ValidationException exception)
            {
                var errors = exception.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray());

                await Results.ValidationProblem(errors).ExecuteAsync(context);
            }
        });

        return app;
    }
}
