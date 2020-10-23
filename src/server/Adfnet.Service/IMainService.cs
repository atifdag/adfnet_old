using System;
using System.Collections.Generic;
using Adfnet.Core.ValueObjects;
using Adfnet.Data.DataEntities;

namespace Adfnet.Service
{
    public interface IMainService
    {
        ApplicationSettings ApplicationSettings { get; }
        List<Guid> GetActionRoles(string controller, string action);
        User IdentityUser { get; }
        int IdentityUserMinRoleLevel { get; }
        Language DefaultLanguage { get; }
    }
}
