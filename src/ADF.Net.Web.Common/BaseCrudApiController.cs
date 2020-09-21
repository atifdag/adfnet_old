using System;
using ADF.Net.Core.Exceptions;
using ADF.Net.Core.Globalization;
using ADF.Net.Service;
using ADF.Net.Service.GenericCrudModels;
using Microsoft.AspNetCore.Mvc;

namespace ADF.Net.Web.Common
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class BaseCrudApiController<T> : ControllerBase where T : class, IServiceModel, new()
    {
        private readonly IMainService _serviceMain;

        private readonly ICrudService<T> _service;

        public BaseCrudApiController(IMainService serviceMain, ICrudService<T> service)
        {
            _serviceMain = serviceMain;
            _service = service;
        }

        [HttpGet]
        
        public ActionResult<ListModel<T>> Get([FromQuery] FilterModel filterModel)
        {

            if (filterModel.StartDate == default)
            {
                filterModel.StartDate = DateTime.Now.AddYears(-2);
            }

            if (filterModel.EndDate == default)
            {
                filterModel.EndDate = DateTime.Now;
            }

            if (filterModel.PageNumber == default)
            {
                filterModel.PageNumber = 1;
            }

            if (filterModel.PageSize == default)
            {
                filterModel.PageSize = _serviceMain.ApplicationSettings.DefaultPageSize;
            }

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

        [HttpGet("{id}")]
        public ActionResult<DetailModel<T>> Get(Guid id)
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

        [HttpPost]
        public ActionResult Post([FromBody] AddModel<T> addModel)
        {

            try
            {
                var affectedModel = _service.Add(addModel);
                return CreatedAtAction("Get", new { id = affectedModel.Item.Id }, affectedModel);
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


        [HttpPut]
        public ActionResult Put([FromBody] UpdateModel<T> updateModel)
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
    

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _service.Delete(id);
                return Ok();
            }

            catch (NotFoundException)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return NotFound(ModelState);
            }

            catch (InvalidTransactionException exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }

            catch (Exception)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }
        }
    }
}
