using Adfnet.Core.GenericCrudModels;
using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;
using Microsoft.AspNetCore.Mvc;
using System;

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
    }
}
