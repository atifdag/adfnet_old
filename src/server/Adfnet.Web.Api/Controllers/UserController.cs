using System;
using System.Collections.Generic;
using Adfnet.Core.Exceptions;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Core.Globalization;
using Adfnet.Core.ValueObjects;
using Adfnet.Service;
using Adfnet.Service.Models;
using Adfnet.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace Adfnet.Web.Api.Controllers
{

    public class UserController : BaseCrudApiController<UserModel>
    {
        private readonly IUserService _serviceUser;
        public UserController(IUserService serviceUser, IMainService serviceMain) : base(serviceUser, serviceMain)
        {
            _serviceUser = serviceUser;
        }

        [Route("FilterWithMultiParent")]
        [HttpPost]

        public ActionResult<ListModel<UserModel>> FilterWithMultiParent(FilterModelWithMultiParent filterModel)
        {
            try
            {
                return Ok(_serviceUser.List(filterModel));
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("MyProfile")]
        [HttpGet]
        public ActionResult<MyProfileModel> MyProfile()
        {
            try
            {
                return Ok(_serviceUser.MyProfile());
            }

            catch (NotFoundException)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound + " " + exception);
                return BadRequest(ModelState);
            }
        }


        [Route("UpdateMyInformation")]
        [HttpPut]
        public ActionResult UpdateMyInformation(UpdateInformationModel model)
        {

            try
            {
                _serviceUser.UpdateMyInformation(model);
                return Ok();
            }

            catch (ValidationException exception)
            {
                var validationResult = exception.ValidationResult;
                foreach (var t in validationResult)
                {
                    ModelState.AddModelError(t.PropertyName, t.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }


        [Route("UpdateMyPassword")]
        [HttpPut]
        public ActionResult UpdateMyPassword(UpdatePasswordModel model)
        {

            try
            {
                _serviceUser.UpdateMyPassword(model);
                return Ok();
            }

            catch (ValidationException exception)
            {
                var validationResult = exception.ValidationResult;
                foreach (var t in validationResult)
                {
                    ModelState.AddModelError(t.PropertyName, t.ErrorMessage);
                }
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
