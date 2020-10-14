using Adfnet.Service;
using Adfnet.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace Adfnet.Web.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class GlobalizationMessagesController : CommonController
    {
        public GlobalizationMessagesController(IMainService serviceMain) : base(serviceMain)
        {
        }


        [Route("Get")]
        [HttpGet]
        public ActionResult<string> Get(string key)
        {
            return Ok(GetMessage(key));
        }

        [Route("GetByParameter")]
        [HttpGet]
        public ActionResult<string> GetByParameter(string key, string parameter)
        {
            return Ok(GetMessageByParameter(key, parameter));
        }

        [Route("GetByParameter2")]
        [HttpGet]
        public ActionResult<string> GetByParameter2(string key, string parameter1, string parameter2)
        {
            return Ok(GetMessageByTwoParameter(key, parameter1, parameter2));
        }


    }
}