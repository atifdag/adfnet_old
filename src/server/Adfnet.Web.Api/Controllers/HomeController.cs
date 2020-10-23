using System.Collections.Generic;
using Adfnet.Core.Globalization;
using Adfnet.Core.ValueObjects;
using Adfnet.Service;
using Adfnet.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace Adfnet.Web.Api.Controllers
{

    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class HomeController : CommonController
    {
        private IMainService _serviceMain;

        public HomeController(IMainService serviceMain) : base(serviceMain)
        {
            _serviceMain = serviceMain;
        }
        [Route("Index")]
        [HttpGet]
        public ActionResult<string> Index()
        {
            return Ok(Dictionary.Success);
        }


        [Route("GlobalizationKeys")]
        [HttpGet]
        public new ActionResult<List<KeyValue>> GlobalizationKeys()
        {
            return  Ok(base.GlobalizationKeys());
        }

    }
}
