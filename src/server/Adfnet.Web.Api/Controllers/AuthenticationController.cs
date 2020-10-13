using System;
using System.Threading;
using Adfnet.Core.Enums;
using Adfnet.Core.Exceptions;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Core.Security;
using Adfnet.Service;
using Adfnet.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Adfnet.Web.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAuthenticationService _serviceAuthentication;
        private readonly IIdentityService _serviceIdentity;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IAuthenticationService serviceAuthentication, IIdentityService serviceIdentity, IConfiguration configuration)
        {
            _serviceAuthentication = serviceAuthentication;
            _serviceIdentity = serviceIdentity;
            _configuration = configuration;
        }


        [Route("Login")]
        [HttpPost]
        public ActionResult<string> Login([FromBody] LoginModel model)
        {

            try
            {
                if (model.Key == _configuration.GetSection("JwtSecurityKey").Value)
                {
                    _serviceAuthentication.Login(model);
                    var principal = (CustomPrincipal)Thread.CurrentPrincipal;
                    var identity = (CustomIdentity)Thread.CurrentPrincipal.Identity;

                    _serviceIdentity.Set(identity, DateTime.Now.AddMinutes(20), false);
                    HttpContext.User = principal;
                    return  Ok(SecurityHelper.GetJwt(identity, _configuration.GetSection("JwtSecurityKey").Value));
                }
                HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                return BadRequest(Messages.DangerInvalidApiClient);

            }

            catch (Exception exception)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                return BadRequest(exception.Message);
            }
        }

        [Route("Register")]
        [HttpPost]
        public ActionResult Register([FromBody] RegisterModel model)
        {
            try
            {
                model.IdentityCode ??= GuidHelper.NewGuid().ToString();
                _serviceAuthentication.Register(model);
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


        [Route("SignOut")]
        [HttpGet]
        public void SignOut(SignOutOption signOutOption = SignOutOption.ValidLogout)
        {
            _serviceAuthentication.SignOut(signOutOption);
            var principal = (CustomPrincipal)Thread.CurrentPrincipal;
            HttpContext.User = principal;
        }


        [Route("ForgotPassword")]
        [HttpGet]
        public ActionResult<string> ForgotPassword([FromQuery] string username)
        {
            try
            {
                _serviceAuthentication.ForgotPassword(username);
                return Ok(Messages.InfoPasswordSentSuccesfully);
            }

            catch (Exception exception)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return BadRequest(exception.Message);
            }
        }


    }
}