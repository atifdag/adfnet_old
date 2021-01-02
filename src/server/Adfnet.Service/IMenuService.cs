using Adfnet.Core;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Core.ValueObjects;
using Adfnet.Service.Models;
using System.Collections.Generic;

namespace Adfnet.Service
{
    public interface IMenuService : ICrudService<MenuModel>
    {
        ListModel<MenuModel> List(FilterModelWithParent filterModel);

        List<IdCodeName> IdNameList();
    }
}
