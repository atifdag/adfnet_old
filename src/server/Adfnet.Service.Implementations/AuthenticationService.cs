using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Adfnet.Core;
using Adfnet.Core.Constants;
using Adfnet.Core.Enums;
using Adfnet.Core.Exceptions;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Core.Security;
using Adfnet.Core.Validation.FluentValidation;
using Adfnet.Core.ValueObjects;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Data.DataEntities;
using Adfnet.Service.Implementations.EmailMessaging;
using Adfnet.Service.Implementations.ValidationRules.FluentValidation;
using Adfnet.Service.Models;

namespace Adfnet.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<SessionHistory> _repositorySessionHistory;
        private readonly IRepository<Session> _repositorySession;
        private readonly IRepository<Role> _repositoryRole;
        private readonly IRepository<Person> _repositoryPerson;
        private readonly IRepository<RoleUserLine> _repositoryRoleUserLine;
        private readonly IRepository<UserHistory> _repositoryUserHistory;
        private readonly ISmtp _smtp;
        
        private readonly IMainService _serviceMain;
        public AuthenticationService(IRepository<User> repositoryUser, IRepository<SessionHistory> repositorySessionHistory, IRepository<Session> repositorySession, IRepository<RoleUserLine> repositoryRoleUserLine, IRepository<Role> repositoryRole, IRepository<Person> repositoryPerson, IRepository<UserHistory> repositoryUserHistory, ISmtp smtp, IMainService serviceMain)
        {
            _repositoryUser = repositoryUser;
            _repositorySessionHistory = repositorySessionHistory;
            _repositorySession = repositorySession;
            _repositoryRoleUserLine = repositoryRoleUserLine;

            _repositoryRole = repositoryRole;

            _repositoryPerson = repositoryPerson;
            _repositoryUserHistory = repositoryUserHistory;
            _smtp = smtp;
            _serviceMain = serviceMain;
        }

      

        public void Login(LoginModel model)
        {
            IValidator validator = new FluentValidator<LoginModel, LoginModelValidationRules>(model);
            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var user = _repositoryUser
                .Join(x => x.Language)
                .Join(x => x.Person)
                .Join(x => x.Creator)
                .Join(x => x.RoleUserLines)
                .ThenJoin(x=>x.Role)
                .Join(x => x.SessionsCreatedBy)
                .FirstOrDefault(x => x.Username == model.Username);

            // Kullanıcı sistemde kayıtlı değilse
            if (user == null)
            {
                throw new NotFoundException(Messages.DangerUserNotFound);
            }

            // Şifresi yanlış ise
            if (model.Password.ToSha512() != user.Password)
            {
                throw new NotFoundException(Messages.DangerIncorrectPassword);
            }

            // Kullanıcı pasif durumda ise
            if (!user.IsApproved)
            {
                throw new NotApprovedException(Messages.DangerItemNotApproved);
            }

            // Kullanıcının hiç rolü yoksa
            if (user.RoleUserLines.Count <= 0)
            {
                throw new NotApprovedException(Messages.DangerUserHasNoRole);
            }

            var sessionIdList = new List<Guid>();

            // Açık kalan oturumu varsa 
            if (user.SessionsCreatedBy?.Count > 0)
            {
                foreach (var session in user.SessionsCreatedBy)
                {
                    // oturum bilgileri tarihçe tablosuna alınıyor
                    _repositorySessionHistory.Add(new SessionHistory
                    {
                        Id = GuidHelper.NewGuid(),
                        Creator = session.Creator,
                        CreationTime = session.CreationTime,
                        LastModificationTime = DateTime.Now,
                        LogoutType = SignOutOption.InvalidLogout.ToString()
                    }, true);
                    sessionIdList.Add(session.Id);
                }
            }

            // Oturumlar siliniyor
            foreach (var i in sessionIdList)
            {
                _repositorySession.Delete(_repositorySession.Get(e => e.Id == i), true);
            }

            // Yeni oturum kaydı yapılıyor
            _repositorySession.Add(new Session
            {
                Id = GuidHelper.NewGuid(),
                Creator = user,
                CreationTime = DateTime.Now

            }, true);


            var roles = user.RoleUserLines
                .Select(
                    line =>
                        _repositoryRoleUserLine
                            .Join(t => t.Role)

                            .FirstOrDefault(x => x.Id == line.Id).Role)
                .Select(role => new KeyValuePair<Guid, string>(role.Id, role.Name)).ToList();
            // Kimlik nesnesi
            var identity = new CustomIdentity();

            // Kullanıcıdaki bilgiler kullanılarak kimlik nesnesinin claim (hak) listesi ayarlanıyor
            var claims = new List<Claim>
            {
                new Claim("UserId",user.Id.ToString()),
                new Claim("Username",user.Username),
                new Claim("FirstName",user.Person.FirstName),
                new Claim("LastName",user.Person.LastName),
                new Claim("DisplayName",user.Person.DisplayName),
                new Claim("Email",user.Email),
                new Claim("LanguageId",user.Language.Id.ToString()),
                new Claim("IsAuthenticated","true",ClaimValueTypes.Boolean),
                new Claim("AuthenticationType","Normal")
            };

            // claim listesi kimlik nesnesine ekleniyor.
            identity.AddClaims(claims);

            // Kullanıcının rol id'leri kimlik nesnesine ekleniyor.
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.Key.ToString()));
            }

            // Yetkilendirilme işlemleri için temel nesne oluşturuluyor
            var principal = new CustomPrincipal(identity);

            // Thread geçerli kimlik bilgisi ayarlanıyor
            Thread.CurrentPrincipal = principal;
        }

        public void SignOut(SignOutOption signOutOption)
        {
            var identity = (CustomIdentity)Thread.CurrentPrincipal?.Identity;

     //       var cacheKeyProfile = CacheKeyOption.Profile + "-" + identity.UserId;

            var sessions = _repositorySession.Get().Where(e => e.Creator.Id == identity.UserId).ToList();
            if (sessions.Count > 0)
            {
                foreach (var session in sessions)
                {
                    _repositorySessionHistory.Add(new SessionHistory
                    {
                        Id = GuidHelper.NewGuid(),
                        Creator = session.Creator,
                        CreationTime = session.CreationTime,
                        LastModificationTime = DateTime.Now,
                        LogoutType = signOutOption.ToString(),

                    }, true);

                    _repositorySession.Delete(session,true);
                }
            }

            // Kimlik nesnesi boşaltılıp yeniden oluşturuluyor
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

            var principal = new CustomPrincipal(identity);

            // Thread geçerli kimlik bilgisi ayarlanıyor
            Thread.CurrentPrincipal = principal;
            //     _cacheService.Remove(cacheKeyProfile);
        }

        public void Register(RegisterModel model)
        {
            IValidator validator = new FluentValidator<RegisterModel, RegisterModelValidationRules>(model);
            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            // Kullanıcı adı kullanımda mı?
            if (_repositoryUser.Get(e => e.Username == model.Username) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Username));
            }

            // Email kullanımda mı?
            if (_repositoryUser.Get(e => e.Email == model.Email) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Email));
            }

            var password = model.Password.ToSha512();

            // kişi bilgisi veritabanında var mı?
            // Kişi bilgisi yoksa yeni kişi bilgisi oluşturuluyor
            Person person;


            var maxDisplayOrderPerson = _repositoryPerson.Get().Max(e => e.DisplayOrder);
            var maxDisplayOrderUser = _repositoryUser.Get().Max(e => e.DisplayOrder);

            if (model.IdentityCode != null)
            {
                if (_repositoryPerson.Get(x => x.IdentityCode == model.IdentityCode) != null)
                {
                    person = _repositoryPerson.Get(x => x.IdentityCode == model.IdentityCode);
                }
                else
                {
                    person = new Person
                    {
                        Id = GuidHelper.NewGuid(),
                        CreationTime = DateTime.Now,
                        LastModificationTime = DateTime.Now,
                        DisplayOrder = maxDisplayOrderPerson + 1,
                        Version = 1,
                        IsApproved = false,
                        IdentityCode = model.IdentityCode.Trim().Length > 0
                            ? model.IdentityCode
                            : GuidHelper.NewGuid().ToString(),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                    };
                }
            }

            else
            {
                person = new Person
                {
                    Id = GuidHelper.NewGuid(),
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = maxDisplayOrderPerson + 1,
                    Version = 1,
                    IsApproved = false,
                    IdentityCode = GuidHelper.NewGuid().ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };
            }

            var user = new User
            {
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                DisplayOrder = maxDisplayOrderUser + 1,
                Version = 1,
                IsApproved = false,
                Username = model.Username,
                Password = password,
                Email = model.Email,
                Language = _serviceMain.DefaultLanguage,
                Person = person,

            };

            person.CreatorId = user.Id;
            person.LastModifierId = user.Id;

            _repositoryPerson.Add(person, true);

            user.Creator = user;
            user.LastModifier = user;

            var affectedUser = _repositoryUser.Add(user, true);

            var role = _repositoryRole.Get(x => x.Code == RoleConstants.Default.Item1);

            if (role == null)
            {
                throw new NotFoundException();
            }

            _repositoryRoleUserLine.Add(new RoleUserLine
            {
                Id = GuidHelper.NewGuid(),
                User = affectedUser,
                Role = role,
                Creator = user,
                CreationTime = DateTime.Now,
                DisplayOrder = 1,
                LastModifier = user,
                LastModificationTime = DateTime.Now,
                Version = 1

            }, true);

            if (!_serviceMain.ApplicationSettings.SendMailAfterAddUser) return;

            var emailUser = new EmailUser
            {
                Id = affectedUser.Id,
                Username = affectedUser.Username,
                Password = password,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = affectedUser.Person.FirstName,
                LastName = affectedUser.Person.LastName
            };
            var emailSender = new EmailSender(_serviceMain,_smtp);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.Register);
        }

        public void ForgotPassword(string username)
        {

            var user = _repositoryUser
                .Join(x => x.Person)
                .Join(x => x.Creator)
                .Join(x => x.RoleUserLines)
                .Join(x => x.SessionsCreatedBy)
                .FirstOrDefault(x => x.Username == username);

            // Kullanıcı sistemde kayıtlı değilse
            if (user == null)
            {
                throw new NotFoundException(Messages.DangerUserNotFound);
            }

            // Kullanıcı pasif durumda ise
            if (!user.IsApproved)
            {
                throw new NotApprovedException(Messages.DangerItemNotApproved);
            }

            // Kullanıcının hiç rolü yoksa
            if (user.RoleUserLines.Count <= 0)
            {
                throw new NotApprovedException(Messages.DangerUserHasNoRole);
            }

            // yeni şifre üret
            var newPassword = SecurityHelper.CreatePassword(8);

            // TODO: kullanıcıya mail at hata olursa exception fırlat




            var sessionIdList = new List<Guid>();

            // Açık kalan oturumu varsa 
            if (user.SessionsCreatedBy?.Count > 0)
            {
                foreach (var session in user.SessionsCreatedBy)
                {
                    // oturum bilgileri tarihçe tablosuna alınıyor
                    _repositorySessionHistory.Add(new SessionHistory
                    {
                        Id = GuidHelper.NewGuid(),
                        Creator = session.Creator,
                        CreationTime = session.CreationTime,
                        LastModificationTime = DateTime.Now,
                        LogoutType = SignOutOption.InvalidLogout.ToString()
                    }, true);
                    sessionIdList.Add(session.Id);
                }
            }

            // Oturumlar siliniyor
            foreach (var i in sessionIdList)
            {
                _repositorySession.Delete(_repositorySession.Get(e => e.Id == i), true);
            }

            var userHistory = user.CreateMapped<User, UserHistory>();
            userHistory.Id = GuidHelper.NewGuid();
            userHistory.ReferenceId = user.Id;
            userHistory.CreatorId = user.Creator.Id;
            _repositoryUserHistory.Add(userHistory, true);

            user.Password = newPassword.ToSha512();
            user.LastModificationTime = DateTime.Now;
            user.LastModifier = user;
            user.Version += 1;
            var affectedUser = _repositoryUser.Update(user, true);
            affectedUser.Password = newPassword;
            var emailUser = new EmailUser
            {
                Username = affectedUser.Username,
                Password = newPassword,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = user.Person.FirstName,
                LastName = user.Person.LastName
            };

            var emailSender = new EmailSender(_serviceMain, _smtp);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.ForgotPassword);

        }

       
    }
}
