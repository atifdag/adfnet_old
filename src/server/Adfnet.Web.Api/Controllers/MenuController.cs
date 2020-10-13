using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;

namespace Adfnet.Web.Api.Controllers
{

    public class MenuController : BaseCrudApiController<MenuModel>
    {
        public MenuController(IMenuService serviceMenu) : base(serviceMenu)
        {
        }
    }
}
