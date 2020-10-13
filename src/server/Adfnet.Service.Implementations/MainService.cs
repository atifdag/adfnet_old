using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Adfnet.Core;
using Adfnet.Core.Exceptions;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Core.Security;
using Adfnet.Core.ValueObjects;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Data.DataEntities;

namespace Adfnet.Service.Implementations
{
    public class MainService : IMainService
    {

        private readonly IRepository<RolePermissionLine> _repositoryRolePermissionLine;
        private readonly IRepository<Parameter> _repositoryParameter;
        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<Language> _repositoryLanguage;

        public MainService(IRepository<RolePermissionLine> repositoryRolePermissionLine, IRepository<Parameter> repositoryParameter, IRepository<User> repositoryUser, IRepository<Language> repositoryLanguage)
        {
            _repositoryRolePermissionLine = repositoryRolePermissionLine;
            _repositoryParameter = repositoryParameter;
            _repositoryUser = repositoryUser;
            _repositoryLanguage = repositoryLanguage;
        }

        public ApplicationSettings ApplicationSettings => new ApplicationSettings
        {
            Address = _repositoryParameter.Get(x => x.Key == "Address").Value,
            ApplicationName = _repositoryParameter.Get(x => x.Key == "ApplicationName").Value,
            ApplicationUrl = _repositoryParameter.Get(x => x.Key == "ApplicationUrl").Value,
            CorporationName = _repositoryParameter.Get(x => x.Key == "CorporationName").Value,
            DefaultLanguage = _repositoryParameter.Get(x => x.Key == "DefaultLanguage").Value,
            DefaultPageSize = _repositoryParameter.Get(x => x.Key == "DefaultPageSize").Value.ToInt(),
            EmailTemplatePath = _repositoryParameter.Get(x => x.Key == "EmailTemplatePath").Value,
            Fax = _repositoryParameter.Get(x => x.Key == "Fax").Value,
            MemoryCacheMainKey = _repositoryParameter.Get(x => x.Key == "MemoryCacheMainKey").Value,
            PageSizeList = _repositoryParameter.Get(x => x.Key == "PageSizeList").Value,
            Phone = _repositoryParameter.Get(x => x.Key == "Phone").Value,
            SendMailAfterAddUser = _repositoryParameter.Get(x => x.Key == "SendMailAfterAddUser").Value.ToBoolean(),
            SendMailAfterUpdateUserInformation = _repositoryParameter.Get(x => x.Key == "SendMailAfterUpdateUserInformation").Value.ToBoolean(),
            SendMailAfterUpdateUserPassword = _repositoryParameter.Get(x => x.Key == "SendMailAfterUpdateUserPassword").Value.ToBoolean(),
            SessionTimeOut = _repositoryParameter.Get(x => x.Key == "SessionTimeOut").Value.ToInt(),
            SmtpPassword = _repositoryParameter.Get(x => x.Key == "SmtpPassword").Value,
            SmtpPort = _repositoryParameter.Get(x => x.Key == "SmtpPort").Value.ToInt(),
            SmtpSenderMail = _repositoryParameter.Get(x => x.Key == "SmtpSenderMail").Value,
            SmtpSenderName = _repositoryParameter.Get(x => x.Key == "SmtpSenderName").Value,
            SmtpServer = _repositoryParameter.Get(x => x.Key == "SmtpServer").Value,
            SmtpSsl = _repositoryParameter.Get(x => x.Key == "SmtpSsl").Value.ToBoolean(),
            SmtpUser = _repositoryParameter.Get(x => x.Key == "SmtpUser").Value,
            TaxAdministration = _repositoryParameter.Get(x => x.Key == "TaxAdministration").Value,
            TaxNumber = _repositoryParameter.Get(x => x.Key == "TaxNumber").Value,
            UseDefaultCredentials = _repositoryParameter.Get(x => x.Key == "UseDefaultCredentials").Value.ToBoolean(),
            UseDefaultNetworkCredentials = _repositoryParameter.Get(x => x.Key == "UseDefaultNetworkCredentials").Value.ToBoolean(),
        };
        public List<Guid> GetActionRoles(string controller, string action)
        {
            return _repositoryRolePermissionLine
                .Join(x => x.Permission)
                .Join(x => x.Role)
                .Where(x => x.Permission.ControllerName == controller && x.Permission.ActionName == action)
                .Select(permissionRoleLine => permissionRoleLine.Role).Select(role => role.Id).ToList();
        }

        public User IdentityUser
        {
            get
            {
                // Thread'de kayıtlı kimlik bilgisi alınıyor
                var identity = (CustomIdentity)Thread.CurrentPrincipal?.Identity;

                User user;

                // Veritabanından sorgulanıyor
                user = _repositoryUser.Get()
                    .Join(x => x.Person)
                    .FirstOrDefault(a => a.Id == identity.UserId);

                // Kullanıcı bulunamadı ise
                if (user == null)
                {
                    throw new NotFoundException(Messages.DangerIdentityUserNotFound);
                }

                return user;
            }
        }
        public Language DefaultLanguage
        {
            get
            {
                return _repositoryLanguage.Get(x => x.DisplayOrder == 1);
            }
        }
    }
}
