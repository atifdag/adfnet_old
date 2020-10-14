using Adfnet.Service;
using Adfnet.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace Adfnet.Web.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class GlobalizationDictionaryController : CommonController
    {
        public GlobalizationDictionaryController(IMainService serviceMain) : base(serviceMain)
        {
        }

        [Route("Get")]
        [HttpGet]
        public ActionResult<string> Get(string key)
        {
            return Ok(GetDictionary(key));
        }
    }
}