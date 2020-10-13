using System.Collections.Generic;
using Adfnet.Core;
using Adfnet.Core.ValueObjects;
using Adfnet.Service.Models;

namespace Adfnet.Service
{
    public interface IRoleService : ICrudService<RoleModel>
    {
        List<IdCodeName> List();
    }
}
