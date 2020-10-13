using Adfnet.Core;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Service.Models;

namespace Adfnet.Service
{
    public interface IParameterService : ICrudService<ParameterModel>
    {
        ListModel<ParameterModel> List(FilterModelWithParent filterModel);

    }
}
