using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Adfnet.Core.Globalization;
using Adfnet.Core.Security;
using Adfnet.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Adfnet.Web.Common
{

    // Her request öncesi çalışır.
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var httpContext = context.Request.HttpContext;
            var securityService = httpContext.RequestServices.GetService<IIdentityService>();

            try
            {
                var identity = securityService.Get();
                if (identity == null)
                {
                    identity = new CustomIdentity();
                    identity.AddClaims(new[]
                    {
                        new Claim("UserId",Guid.Empty.ToString()),
                        new Claim("Username",string.Empty),
                        new Claim("Password",string.Empty),
                        new Claim("FirstName",string.Empty),
                        new Claim("LastName",string.Empty),
                        new Claim("DisplayName",string.Empty),
                        new Claim("Email",string.Empty),
                        new Claim("LanguageId",Guid.Empty.ToString()),
                        new Claim("IsAuthenticated",false.ToString()),
                    });

                }
                var principal = new CustomPrincipal(identity);
                Thread.CurrentPrincipal = principal;
                httpContext.User = principal;
                var pathValue = httpContext.Request.Path.Value;
                if (pathValue != "/")
                {
                    pathValue = pathValue.Replace("api/", "");

                    var pathValueArray = pathValue.Split('/');

                    if (pathValueArray.Length > 1)
                    {
                        string controller;
                        string action;

                        if (pathValueArray.Length == 2)
                        {
                            controller = pathValueArray[1];
                            action = "Index";
                        }
                        else
                        {
                            controller = pathValueArray[1];
                            action = pathValueArray[2];
                        }
                        var mainService = httpContext.RequestServices.GetService<IMainService>();
                        var actionRoles = mainService.GetActionRoles(controller, action);

                        // Sayfa vtde kayıtlı değilse devam et
                        if (actionRoles != null && actionRoles.Count > 0)
                        {
                            var identityIsAuthorized = identity.Roles.Any(r => actionRoles.Contains(r));
                            if (!identityIsAuthorized)
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsync(Messages.DangerNoPermission);
                                return;
                            }
                        }

                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync(Messages.DangerNoPermission);
                        return;
                    }



                }
            }
            catch (Exception e)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(e.Message);
                return;
            }





            await _next(context);
        }
    }
}
