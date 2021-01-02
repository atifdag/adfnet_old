using Adfnet.Core.Exceptions;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Core.Globalization;
using Adfnet.Core.ValueObjects;
using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Adfnet.Web.Api.Controllers
{

    public class MenuController : BaseCrudApiController<MenuModel>
    {

        private readonly IMenuService _menuService;
        public MenuController(IMenuService serviceMenu, IMainService serviceMain) : base(serviceMenu, serviceMain)
        {
            _menuService = serviceMenu;
        }




        [Route("FilterWithMultiParent")]
        [HttpPost]
        public ActionResult<ListModel<MenuModel>> FilterWithMultiParent(FilterModelWithMultiParent filterModel)
        {
            try
            {
                return Ok(_menuService.List(filterModel));
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("IdNameList")]
        [HttpGet]
        public ActionResult<List<IdCodeName>> IdNameList()
        {
            try
            {
                return Ok(_menuService.IdNameList());
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
