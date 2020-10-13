using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;

namespace Adfnet.Web.Api.Controllers
{

    public class ParameterController : BaseCrudApiController<ParameterModel>
    {
        public ParameterController(IParameterService serviceParameter) : base(serviceParameter)
        {
        }
    }
}
