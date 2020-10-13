using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Adfnet.Core.Helpers;
using Adfnet.Core.Security;
using Adfnet.Service;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Adfnet.Web.Common
{
    public class TokenBaseIdentityService : IIdentityService
    {
        private readonly string _key;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpContext Context => _httpContextAccessor.HttpContext;

        public TokenBaseIdentityService(IHttpContextAccessor httpContextAccessor, string key)
        {
            _httpContextAccessor = httpContextAccessor;
            _key = key;
        }

        public void Set(CustomIdentity identity, DateTime expires, bool rememberMe)
        {
            var jwt = SecurityHelper.GetJwt(identity, _key);
            Context.Response.Headers.Add("Authorization", jwt);
        }

        public CustomIdentity Get()
        {
            var identity = new CustomIdentity();
            string authorization = Context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorization))
            {
                return null;
            }
            var jwt = authorization.Replace("Bearer ", "");
            var takenHeader = Encoding.UTF8.GetString(Convert.FromBase64String(jwt.Split('.')[0]));
            var takenPayload = Encoding.UTF8.GetString(Convert.FromBase64String(jwt.Split('.')[1]));
            var takenSignature = Encoding.UTF8.GetString(Convert.FromBase64String(jwt.Split('.')[2]));

            var newFirstSection = Convert.ToBase64String(Encoding.UTF8.GetBytes(takenHeader)) + "." + Convert.ToBase64String(Encoding.UTF8.GetBytes(takenPayload));
            var newSignature = Encoding.UTF8.GetString(Convert.FromBase64String(SecurityHelper.ToHmacSha256(newFirstSection, _key)));

            if (takenSignature != newSignature) return identity;
            dynamic jObject = JObject.Parse(takenPayload);

            Guid userId = jObject.UserId;
            Guid languageId = jObject.LanguageId;
            string username = jObject.Username;
            string firstName = jObject.FirstName;
            string lastName = jObject.LastName;
            string displayName = jObject.DisplayName;
            string email = jObject.Email;
            JArray roles = jObject.Roles;

            var claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString()),
                new Claim("Username", username),
                new Claim("FirstName", firstName),
                new Claim("LastName", lastName),
                new Claim("DisplayName", displayName),
                new Claim("Email", email),
                new Claim("LanguageId", languageId.ToString()),
                new Claim("IsAuthenticated", "true", ClaimValueTypes.Boolean),
            };

            identity.AddClaims(claims);

            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
            }
            return identity;
        }

        public void Remove()
        {
            string authHeader = Context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                Context.Request.Headers.Add("Authorization", string.Empty);
            }
            Context.Request.Headers.Remove("Authorization");
        }


    }
}
