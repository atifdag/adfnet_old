using Adfnet.Core;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Service.Models;

namespace Adfnet.Service
{
    public interface IMenuService : ICrudService<MenuModel>
    {
        ListModel<MenuModel> List(FilterModelWithParent filterModel);
    }
}
