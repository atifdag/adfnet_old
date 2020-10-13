using System.Collections.Generic;
using Adfnet.Core;
using Adfnet.Core.ValueObjects;
using Adfnet.Service.Models;

namespace Adfnet.Service
{
    public interface IParameterGroupService : ICrudService<ParameterGroupModel>
    {
        List<IdCodeName> List();
    }
}
