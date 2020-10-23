using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;

namespace Adfnet.Web.Api.Controllers
{

    public class LanguageController : BaseCrudApiController<LanguageModel>
    {
        public LanguageController(ILanguageService serviceLanguage, IMainService serviceMain) : base(serviceLanguage, serviceMain)
        {
        }
    }
}
