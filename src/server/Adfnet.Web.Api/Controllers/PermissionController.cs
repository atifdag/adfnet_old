using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;

namespace Adfnet.Web.Api.Controllers
{

    public class PermissionController : BaseCrudApiController<PermissionModel>
    {
        public PermissionController(IPermissionService servicePermission) : base(servicePermission)
        {
        }
    }
}
