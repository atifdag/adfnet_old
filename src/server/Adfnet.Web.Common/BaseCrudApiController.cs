using System;
using Adfnet.Core;
using Adfnet.Core.Enums;
using Adfnet.Core.Exceptions;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Core.Globalization;
using Adfnet.Service;
using Microsoft.AspNetCore.Mvc;

namespace Adfnet.Web.Common
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class BaseCrudApiController<T> : ControllerBase where T : class, IServiceModel, new()
    {
        private readonly ICrudService<T> _service;
        private readonly IMainService _serviceMain;

        public BaseCrudApiController(ICrudService<T> service, IMainService serviceMain)
        {
            _service = service;
            _serviceMain = serviceMain;
        }

        [Route("List")]
        [HttpGet]
        public ActionResult<ListModel<T>> List()
        {

            try

            {
                var filterModel = new FilterModel
                {
                    StartDate = DateTime.Now.AddYears(-2),
                    EndDate = DateTime.Now,
                    Status = StatusOption.All.GetHashCode(),
                    PageNumber = 1,
                    PageSize = _serviceMain.ApplicationSettings.DefaultPageSize,
                    Searched = string.Empty
                };
                return Ok(_service.List(filterModel));
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.ToString());
                return BadRequest(ModelState);
            }
        }

        [Route("Filter")]
        [HttpPost]
        public ActionResult<ListModel<T>> List([FromBody] FilterModel filterModel)
        {

            try
            
            {
                return Ok(_service.List(filterModel));
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.ToString());
                return BadRequest(ModelState);
            }
        }

        [Route("Detail/{id}")]
        [HttpGet]
        public ActionResult<DetailModel<T>> Detail(Guid id)
        {

            try
            {
                return Ok(_service.Detail(id));
            }

            catch (NotFoundException)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return NotFound(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.ToString());
                return BadRequest(ModelState);
            }
        }

        [Route("Add")]
        [HttpGet]
        public ActionResult<AddModel<T>> Add()
        {
            try
            {
                return Ok(_service.Add());
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Add")]
        [HttpPost]
        public ActionResult Add([FromBody] AddModel<T> addModel)
        {

            try
            {
                return Ok(_service.Add(addModel));
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

        [Route("Update/{id}")]
        [HttpGet]
        public ActionResult<UpdateModel<T>> Update(Guid id)
        {
            try
            {
                return Ok(_service.Update(id));
            }

            catch (NotFoundException)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return NotFound(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Update")]
        [HttpPut]
        public ActionResult Update([FromBody] UpdateModel<T> updateModel)
        {

            try
            {
                return Ok(_service.Update(updateModel));
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

        [Route("Delete/{id}")]
        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _service.Delete(id);
                return Ok();
            }

            catch (InvalidTransactionException exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }

            catch (NotFoundException)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return NotFound(ModelState);
            }

            catch (Exception)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }
        }
    }
}
