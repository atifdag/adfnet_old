using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;

namespace Adfnet.Web.Api.Controllers
{

    public class ParameterGroupController : BaseCrudApiController<ParameterGroupModel>
    {
        public ParameterGroupController(IParameterGroupService serviceParameterGroup, IMainService serviceMain) : base(serviceParameterGroup, serviceMain)
        {
        }
    }
}
