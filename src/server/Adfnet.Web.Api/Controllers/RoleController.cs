using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;

namespace Adfnet.Web.Api.Controllers
{

    public class RoleController : BaseCrudApiController<RoleModel>
    {
        public RoleController(IRoleService serviceRole) : base(serviceRole)
        {
        }
    }
}
