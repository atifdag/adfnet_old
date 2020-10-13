using System;
using Adfnet.Core;
using Adfnet.Core.Exceptions;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Core.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace Adfnet.Web.Common
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class BaseCrudWithLanguageApiController<T> : ControllerBase where T : class, IServiceModel, new()
    {
        private readonly ICrudServiceWithLanguage<T> _service;

        public BaseCrudWithLanguageApiController(ICrudServiceWithLanguage<T> service)
        {
            _service = service;
        }

        [Route("List")]
        [HttpGet]
        public ActionResult<ListModel<T>> List([FromQuery] FilterModel filterModel, Guid languageId)
        {

            try
            
            {
                return Ok(_service.List(filterModel, languageId));
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.ToString());
                return BadRequest(ModelState);
            }
        }

        [Route("Detail/{id}-{languageId}")]
        [HttpGet]
        public ActionResult<DetailModel<T>> Detail(Guid id, Guid languageId)
        {

            try
            {
                return Ok(_service.Detail(id, languageId));
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
                var affectedModel = _service.Add(addModel);
                return Ok(affectedModel);
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

        [Route("Update/{id}-{languageId}")]
        [HttpGet]
        public ActionResult<UpdateModel<T>> Update(Guid id, Guid languageId)
        {
            try
            {
                return Ok(_service.Update(id, languageId));
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

        [Route("Delete/{id}-{languageId}")]
        [HttpDelete]
        public ActionResult Delete(Guid id, Guid languageId)
        {
            try
            {
                _service.Delete(id, languageId);
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
