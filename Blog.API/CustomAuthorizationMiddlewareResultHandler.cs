using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Blog.API;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate requestDelegate,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizationResult)
    {
        if (authorizationResult.Forbidden)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://httpstatuses.io/403",
                Status = (int)HttpStatusCode.Forbidden,
                Title = nameof(HttpStatusCode.Forbidden),
                Detail = "You are not authorized to access this resource."
            };

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(problemDetails);
            return;
        }

        if (authorizationResult.Challenged)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://httpstatuses.io/401",
                Status = (int)HttpStatusCode.Unauthorized,
                Title = nameof(HttpStatusCode.Unauthorized),
                Detail = "You are not authenticated to access this resource."
            };

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(problemDetails);
            return;
        }

        await _defaultHandler.HandleAsync(requestDelegate, context, policy, authorizationResult);
    }
}