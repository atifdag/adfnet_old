using System;
using System.Collections.Generic;
using Adfnet.Core.Exceptions;
using Adfnet.Core.Globalization;
using Adfnet.Core.ValueObjects;
using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace Adfnet.Web.Api.Controllers
{

    public class RoleController : BaseCrudApiController<RoleModel>
    {
        private readonly IRoleService _serviceRole;
        public RoleController(IRoleService serviceRole, IMainService serviceMain) : base(serviceRole, serviceMain)
        {
            _serviceRole = serviceRole;
        }

        [Route("IdNameList")]
        [HttpGet]
        public ActionResult<List<IdName>> IdNameList()
        {
            try
            {
                return Ok(_serviceRole.IdNameList());
            }

            catch (NotFoundException)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
