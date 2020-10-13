using System.Collections.Generic;
using Adfnet.Core;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Core.ValueObjects;
using Adfnet.Service.Models;

namespace Adfnet.Service
{
    public interface IMenuService : ICrudService<MenuModel>
    {
        List<IdCodeName> List();
        ListModel<MenuModel> List(FilterModelWithParent filterModel);
    }
}
