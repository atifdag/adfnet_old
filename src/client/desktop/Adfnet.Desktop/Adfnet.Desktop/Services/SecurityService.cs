using System;
using System.Linq;
using Adfnet.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Adfnet.Desktop.Services
{
    internal class SecurityService
    {
        private readonly IServiceProvider _provider;

        public SecurityService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public bool IsAuthorized(string pathValue)
        {
           
            var identityService = _provider.GetService<IIdentityService>();
            var identity = identityService.Get();
            if (identity == null)
            {
                return false;
            }

            var pathValueArray = pathValue.Split('/');
            if (pathValueArray.Length <= 1) return false;
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

            var mainService = _provider.GetService<IMainService>();
            var actionRoles = mainService.GetActionRoles(controller, action);

            if (actionRoles == null || actionRoles.Count <= 0) return false;
            var identityIsAuthorized = identity.Roles.Any(r => actionRoles.Contains(r));
            return identityIsAuthorized;
        }
    }

}
