using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Khet360.Application.Interfaces;

namespace Khet360.Api.Attributes;

/// <summary>
/// Attribute to enforce a specific permission for an API action.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(string permissionCode) : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { permissionCode };
    }

    private class PermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permissionCode;
        private readonly IAuthorizationService _authService;

        public PermissionFilter(string permissionCode, IAuthorizationService authService)
        {
            _permissionCode = permissionCode;
            _authService = authService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!_authService.HasPermission(_permissionCode))
            {
                context.Result = new ForbidResult();
            }

            await Task.CompletedTask;
        }
    }
}
