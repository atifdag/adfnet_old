using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using Adfnet.Core.Helpers;
using Adfnet.Core.Security;
using Adfnet.Service;
using Newtonsoft.Json.Linq;

namespace Adfnet.Desktop.WindowsForms
{
    internal class FileBaseIdentityService : IIdentityService
    {
        private readonly string _key;

        private static string FileName
        {
            get
            {
                var rootPath = AppContext.BaseDirectory;
                if (AppContext.BaseDirectory.Contains("bin"))
                {
                    rootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
                }

                var appData= Path.Combine(rootPath, "AppData");
                return Path.Combine(appData, "token.txt");
            }
        }

        public FileBaseIdentityService(string key)
        {
            _key = key;
        }

        public void Set(CustomIdentity identity, DateTime expires, bool rememberMe)
        {
            var jwt = SecurityHelper.GetJwt(identity, _key);

           

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }

            using var fs = File.Create(FileName);
            var title = new UTF8Encoding(true).GetBytes(jwt);
            fs.Write(title, 0, title.Length);
        }

        public CustomIdentity Get()
        {
            var identity = new CustomIdentity();

            if (!File.Exists(FileName))
            {
                identity = new CustomIdentity();
                identity.AddClaims(new[]
                {
                    new Claim("UserId",Guid.Empty.ToString()),
                    new Claim("Username",string.Empty),
                    new Claim("Password",string.Empty),
                    new Claim("FirstName",string.Empty),
                    new Claim("LastName",string.Empty),
                    new Claim("DisplayName",string.Empty),
                    new Claim("Email",string.Empty),
                    new Claim("LanguageId",Guid.Empty.ToString()),
                    new Claim("IsAuthenticated",false.ToString()),
                });
                Set(identity, DateTime.Now.AddMinutes(20), false);
                return identity;
            }

            

            var text = File.ReadAllText(FileName);
            var jwt = text.Replace("Bearer ", "");

            var takenHeader = Encoding.UTF8.GetString(Convert.FromBase64String(jwt.Split('.')[0]));
            var takenPayload = Encoding.UTF8.GetString(Convert.FromBase64String(jwt.Split('.')[1]));
            var takenSignature = Encoding.UTF8.GetString(Convert.FromBase64String(jwt.Split('.')[2]));

            var newFirstSection = Convert.ToBase64String(Encoding.UTF8.GetBytes(takenHeader)) + "." + Convert.ToBase64String(Encoding.UTF8.GetBytes(takenPayload));
            var newSignature = Encoding.UTF8.GetString(Convert.FromBase64String(SecurityHelper.ToHmacSha256(newFirstSection, _key)));

            if (takenSignature != newSignature) return identity;
            dynamic jObject = JObject.Parse(takenPayload);

            Guid userId = jObject.UserId;
            string username = jObject.Username;
            string firstName = jObject.FirstName;
            string lastName = jObject.LastName;
            string displayName = jObject.DisplayName;
            string email = jObject.Email;
            Guid languageId = jObject.LanguageId;
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
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
        }
    }
}
