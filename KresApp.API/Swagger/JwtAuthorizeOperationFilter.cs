using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KresApp.API.Swagger;

public sealed class JwtAuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!RequiresAuth(context))
            return;

        var schemeId = JwtBearerDefaults.AuthenticationScheme;
        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(schemeId, context.Document)] = []
        };

        operation.Security ??= [];
        operation.Security.Add(requirement);
    }

    private static bool RequiresAuth(OperationFilterContext context)
    {
        if (AllowsAnonymousExplicitly(context.ApiDescription.ActionDescriptor.EndpointMetadata))
            return false;

        if (HasAuthorizeInMetadata(context.ApiDescription.ActionDescriptor.EndpointMetadata))
            return true;

        return context.ApiDescription.ActionDescriptor switch
        {
            ControllerActionDescriptor cad => HasAuthorizeControllerOrAction(cad),
            _ => false
        };
    }

    private static bool AllowsAnonymousExplicitly(IList<object>? metadata)
    {
        return metadata?.OfType<IAllowAnonymous>().Any() ?? false;
    }

    private static bool HasAuthorizeInMetadata(IList<object>? metadata)
    {
        return metadata?.OfType<IAuthorizeData>().Any() ?? false;
    }

    private static bool HasAuthorizeControllerOrAction(ControllerActionDescriptor cad)
    {
        return cad.ControllerTypeInfo.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Length > 0
               || cad.MethodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Length > 0;
    }
}
