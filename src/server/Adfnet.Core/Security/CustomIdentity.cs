using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace Adfnet.Core.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Yetkilendirilme işlemleri için kimlik sınıfı
    /// </summary>
    public class CustomIdentity : ClaimsIdentity
    {
        public CustomIdentity() : base()
        {

        }

        public CustomIdentity(BinaryReader reader) : base(reader)
        {

        }

        public CustomIdentity(ClaimsIdentity other) : base(other)
        {

        }

        public CustomIdentity(IEnumerable<Claim> claims) : base(claims)
        {

        }

        public CustomIdentity(IEnumerable<Claim> claims, string authenticationType) : base(claims, authenticationType)
        {

        }

        /// <summary>
        /// Kullanıcı Id
        /// </summary>

        public Guid UserId
        {
            get
            {
                return new Guid(Claims.SingleOrDefault(s => s.Type == "UserId")?.Value);
            }
        }

        /// <summary>
        /// Kullanıcı adı
        /// </summary>
        public string Username
        {
            get
            {
                return Claims.SingleOrDefault(s => s.Type == "Username")?.Value;
            }
        }

        /// <summary>
        /// Kimlik kullanıcısının adı
        /// </summary>

        public string FirstName
        {
            get
            {
                return Claims.SingleOrDefault(s => s.Type == "FirstName")?.Value;
            }
        }

        /// <summary>
        /// Kimlik kullanıcısının soyadı
        /// </summary>
        public string LastName
        {
            get
            {
                return Claims.SingleOrDefault(s => s.Type == "LastName")?.Value;
            }
        }

        /// <summary>
        /// Kimlik kullanıcısının ekran adı
        /// </summary>

        public string DisplayName
        {
            get
            {
                return Claims.SingleOrDefault(s => s.Type == "DisplayName")?.Value;
            }
        }

        /// <summary>
        /// Kimlik kullanıcısının eposta adresi
        /// </summary>
        public string Email
        {
            get
            {
                return Claims.SingleOrDefault(s => s.Type == "Email")?.Value;
            }
        }

        /// <summary>
        /// Kullanıcı Id
        /// </summary>

        public Guid LanguageId
        {
            get
            {
                return new Guid(Claims.SingleOrDefault(s => s.Type == "LanguageId")?.Value);
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// Kimlik doğrulandı mı?
        /// </summary>

        public override bool IsAuthenticated
        {
            get
            {
                return Convert.ToBoolean(Claims.SingleOrDefault(s => s.Type == "IsAuthenticated")?.Value);
            }
        }

        /// <summary>
        /// Kimlik kullanıcısının rol Id'leri
        /// </summary>
        public List<Guid> Roles
        {
            get
            {
                return Claims.Where(s => s.Type == ClaimTypes.Role).Select(claim => new Guid(claim.Value)).ToList();
            }
        }
    }
}
