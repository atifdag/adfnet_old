using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Adfnet.Core;
using Adfnet.Core.Constants;
using Adfnet.Core.Enums;
using Adfnet.Core.Exceptions;
using Adfnet.Core.GenericCrudModels;
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
    public class UserService : IUserService
    {

        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<RolePermissionLine> _repositoryRolePermissionLine;
        private readonly IRepository<PermissionMenuLine> _repositoryPermissionMenuLine;
        private readonly IRepository<RoleUserLine> _repositoryRoleUserLine;
        private readonly IRepository<UserHistory> _repositoryUserHistory;
        private readonly IRepository<Person> _repositoryPerson;
        private readonly IRepository<PersonHistory> _repositoryPersonHistory;
        private readonly IRepository<Role> _repositoryRole;
        private readonly IRepository<RoleUserLineHistory> _repositoryRoleUserLineHistory;
        private readonly IMainService _serviceMain;
        private readonly ISmtp _smtp;
        private readonly IRepository<Language> _repositoryLanguage;
        private readonly List<IdName> _languages;

        public UserService(IRepository<User> repositoryUser, IRepository<RolePermissionLine> repositoryRolePermissionLine, IRepository<PermissionMenuLine> repositoryPermissionMenuLine, IRepository<RoleUserLine> repositoryRoleUserLine, IRepository<UserHistory> repositoryUserHistory, ISmtp smtp, IRepository<Person> repositoryPerson, IRepository<PersonHistory> repositoryPersonHistory, IRepository<Role> repositoryRole, IRepository<RoleUserLineHistory> repositoryRoleUserLineHistory, IMainService serviceMain, IRepository<Language> repositoryLanguage)
        {
            _repositoryUser = repositoryUser;
            _repositoryRolePermissionLine = repositoryRolePermissionLine;
            _repositoryPermissionMenuLine = repositoryPermissionMenuLine;
            _repositoryRoleUserLine = repositoryRoleUserLine;
            _repositoryUserHistory = repositoryUserHistory;
            _smtp = smtp;
            _repositoryPerson = repositoryPerson;
            _repositoryPersonHistory = repositoryPersonHistory;
            _repositoryRole = repositoryRole;
            _repositoryRoleUserLineHistory = repositoryRoleUserLineHistory;
            _serviceMain = serviceMain;
            _repositoryLanguage = repositoryLanguage;

            _languages = _repositoryLanguage.Get().Where(x => x.IsApproved).OrderBy(x => x.DisplayOrder)
                .Select(t => new IdName(t.Id, t.Name)).ToList();
        }
        public ListModel<UserModel> List(FilterModel filterModel)
        {
            var model = filterModel.CreateMapped<FilterModel, ListModel<UserModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, null, model);
        }

        private ListModel<UserModel> List(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, int status, string searched, List<Guid> parentIds, ListModel<UserModel> model)
        {
            var resetedStartDate = startDate.ResetTimeToStartOfDay();
            var resetedEndDate = endDate.ResetTimeToEndOfDay();

            Expression<Func<User, bool>> expression;

            model.Paging ??= new Paging
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };
            
            if (status != -1)
            {
                var bStatus = status.ToString().ToBoolean();

                if (searched != null)
                {
                    if (bStatus)
                    {
                        expression = c => c.IsApproved && c.Username.Contains(searched);
                    }
                    else
                    {
                        expression = c => c.IsApproved == false && c.Username.Contains(searched);
                    }
                }
                else
                {
                    if (bStatus)
                    {
                        expression = c => c.IsApproved;
                    }
                    else
                    {
                        expression = c => c.IsApproved == false;
                    }
                }

            }
            else
            {
                if (searched != null)
                {
                    expression = c => c.Username.Contains(searched);
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }

            expression = expression.And(e => e.CreationTime >= resetedStartDate && e.CreationTime <= resetedEndDate);

            if (parentIds != null)
            {
                if (parentIds.Count > 0)
                {
                    expression = expression.And(e => e.RoleUserLines.Any(x => parentIds.Contains(x.Role.Id)));
                }
            }

            var identityUserMinRoleLevel = _serviceMain.IdentityUserMinRoleLevel;

            expression = expression.And(x => x.RoleUserLines.All(t => t.Role.Level > identityUserMinRoleLevel));

            var sortHelper = new SortHelper<User>();

            sortHelper.OrderBy(x => x.DisplayOrder);

            var query = (IOrderedQueryable<User>)_repositoryUser
                .Join(x => x.Person)
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.RoleUserLines)
                .ThenJoin(x => x.Role)
                .Where(expression);

            query = sortHelper.GenerateOrderedQuery(query);

            model.Paging.TotalItemCount = query.Count();

            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;

            var modelItems = new HashSet<UserModel>();

            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<User, UserModel>();
                modelItem.Creator = new IdName(item.Creator.Id, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdName(item.LastModifier.Id, item.LastModifier.Person.DisplayName);
                modelItem.IdentityCode = item.Person.IdentityCode;
                modelItem.FirstName = item.Person.FirstName;
                modelItem.LastName = item.Person.LastName;
                modelItem.Roles = item.RoleUserLines.Select(t => t.Role).Select(role => new IdCodeNameSelected(role.Id, role.Code, role.Name, true)).ToList();
                modelItems.Add(modelItem);
            }

            model.Items = modelItems.ToList();

            var pageSizeDescription = _serviceMain.ApplicationSettings.PageSizeList;

            var pageSizes = pageSizeDescription.Split(',').Select(s => new KeyValuePair<int, string>(s.ToInt(), s)).ToList();

            pageSizes.Insert(0, new KeyValuePair<int, string>(-1, "[" + Dictionary.All + "]"));

            model.Paging.PageSizes = pageSizes;

            model.Paging.PageCount = (int)Math.Ceiling((float)model.Paging.TotalItemCount / model.Paging.PageSize);

            if (model.Paging.TotalItemCount > model.Items.Count)
            {
                model.Paging.HasNextPage = true;
            }

            if (model.Paging.PageNumber == 1)
            {
                if (model.Paging.TotalItemCount > 0)
                {
                    model.Paging.IsFirstPage = true;
                }


                if (model.Paging.PageCount == 1)
                {
                    model.Paging.IsLastPage = true;
                }

            }

            else if (model.Paging.PageNumber == model.Paging.PageCount)
            {
                model.Paging.HasNextPage = false;

                if (model.Paging.PageCount > 1)
                {
                    model.Paging.IsLastPage = true;
                    model.Paging.HasPreviousPage = true;
                }
            }

            else
            {
                model.Paging.HasNextPage = true;
                model.Paging.HasPreviousPage = true;
            }

            if (model.Paging.TotalItemCount > model.Items.Count && model.Items.Count <= 0)
            {
                model.Message = Messages.DangerRecordNotFoundInPage;
            }

            if (model.Paging.TotalItemCount == 0)
            {
                model.Message = Messages.DangerRecordNotFound;
            }

            return model;

        }

       
        public DetailModel<UserModel> Detail(Guid id)
        {
            var identityUserMinRoleLevel = _serviceMain.IdentityUserMinRoleLevel;

            var item = _repositoryUser
                .Join(x => x.Language)
                .Join(x => x.Person)
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.RoleUserLines)
                .FirstOrDefault(x => x.Id == id && x.RoleUserLines.All(t => t.Role.Level > identityUserMinRoleLevel));

            if (item == null)
            {
                throw new NotFoundException();
            }

            var roleList = new List<IdCodeNameSelected>();

            var allRoles = _repositoryRole.Get().Where(x => x.IsApproved && x.Level > identityUserMinRoleLevel).ToList();
            var userRoles = _repositoryRoleUserLine
                .Join(x => x.Role)
                .Where(x => x.User.Id == item.Id).Select(x => x.Role.Id).ToList();

            foreach (var role in allRoles)
            {
                roleList.Add(userRoles.Contains(role.Id) ? new IdCodeNameSelected(role.Id, role.Code, role.Name, true) : new IdCodeNameSelected(role.Id, role.Code, role.Name, false));
            }

            var modelItem = item.CreateMapped<User, UserModel>();
            modelItem.Roles = roleList;
            modelItem.Creator = new IdName(item.Creator.Id, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdName(item.LastModifier.Id, item.LastModifier.Person.DisplayName);
            modelItem.Language = new IdName(item.Language.Id, item.Language.Name);
            modelItem.IdentityCode = item.Person.IdentityCode;
            modelItem.FirstName = item.Person.FirstName;
            modelItem.LastName = item.Person.LastName;
            modelItem.Languages = _languages;
            return new DetailModel<UserModel>
            {
                Item = modelItem
            };
        }

        public AddModel<UserModel> Add()
        {
            return new AddModel<UserModel>
            {
                Item = new UserModel()
            };
        }

        public AddModel<UserModel> Add(AddModel<UserModel> addModel)
        {

            var validator = new FluentValidator<UserModel, UserValidationRules>(addModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var language = _repositoryLanguage.Get(e => e.Id == addModel.Item.Language.Id);

            if (language == null)
            {
                throw new ParentNotFoundException();
            }

            var item = addModel.Item.CreateMapped<UserModel, User>();

            if (_repositoryUser.Get().FirstOrDefault(e => e.Username == item.Username) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Username));
            }

            if (_repositoryUser.Get(e => e.Email == item.Email) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Email));
            }

            var password = addModel.Item.Password.ToSha512();


            // kişi bilgisi veritabanında var mı?
            // Kişi bilgisi yoksa yeni kişi bilgisi oluşturuluyor
            Person person;

            var maxDisplayOrderPerson = _repositoryPerson.Get().Max(e => e.DisplayOrder);
            var maxDisplayOrderUser = _repositoryUser.Get().Max(e => e.DisplayOrder);

            if (addModel.Item.IdentityCode != null)
            {
                if (_repositoryPerson.Get(x => x.IdentityCode == addModel.Item.IdentityCode) != null)
                {
                    person = _repositoryPerson.Get(x => x.IdentityCode == addModel.Item.IdentityCode);
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
                        IdentityCode = addModel.Item.IdentityCode.Trim().Length > 0 ? addModel.Item.IdentityCode : GuidHelper.NewGuid().ToString(),
                        FirstName = addModel.Item.FirstName,
                        LastName = addModel.Item.LastName,
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
                    FirstName = addModel.Item.FirstName,
                    LastName = addModel.Item.LastName,
                };
            }


            person.CreatorId = _serviceMain.IdentityUser.Id;
            person.LastModifierId = _serviceMain.IdentityUser.Id;

            _repositoryPerson.Add(person, true);
            item.Id = GuidHelper.NewGuid();
            item.Creator = _serviceMain.IdentityUser;
            item.CreationTime = DateTime.Now;

            item.LastModificationTime = DateTime.Now;
            item.LastModifier = _serviceMain.IdentityUser;

            item.DisplayOrder = maxDisplayOrderUser + 1;
            item.Person = person;
            item.Language = language;
            item.Version = 1;

            var affectedUser = _repositoryUser.Add(item, true);

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
                Creator = _serviceMain.IdentityUser,
                CreationTime = DateTime.Now,
                DisplayOrder = 1,
                LastModifier = _serviceMain.IdentityUser,
                LastModificationTime = DateTime.Now,
                Version = 1

            }, true);

            addModel.Item = affectedUser.CreateMapped<User, UserModel>();

            addModel.Item.Creator = new IdName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Person.DisplayName);
            addModel.Item.Language = new IdName(affectedUser.Language.Id, affectedUser.Language.Name);

            addModel.Item.IdentityCode = affectedUser.Person.IdentityCode;
            addModel.Item.FirstName = affectedUser.Person.FirstName;
            addModel.Item.LastName = affectedUser.Person.LastName;

            if (!_serviceMain.ApplicationSettings.SendMailAfterAddUser) return addModel;

            var emailUser = new EmailUser
            {
                Username = affectedUser.Username,
                Password = password,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = affectedUser.Person.FirstName,
                LastName = affectedUser.Person.LastName
            };
            var emailSender = new EmailSender(_serviceMain, _smtp);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.Add);
            return addModel;

        }

        public UpdateModel<UserModel> Update(Guid id)
        {
            return new UpdateModel<UserModel>
            {
                Item = Detail(id).Item
            };
        }


        public UpdateModel<UserModel> Update(UpdateModel<UserModel> updateModel)
        {

            IValidator validator = new FluentValidator<UserModel, UserValidationRules>(updateModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var language = _repositoryLanguage.Get(e => e.Id == updateModel.Item.Language.Id);

            if (language == null)
            {
                throw new ParentNotFoundException();
            }

            var identityUserMinRoleLevel = _serviceMain.IdentityUserMinRoleLevel;


            var itemUser = _repositoryUser
                
                .Join(x => x.Person)
                .Join(x => x.Language)
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .FirstOrDefault(x => x.Id == updateModel.Item.Id && x.RoleUserLines.All(t => t.Role.Level > identityUserMinRoleLevel));
            if (itemUser == null)
            {
                throw new NotFoundException();
            }

            var person = itemUser.Person;

            if (person == null)
            {
                throw new ParentNotFoundException();
            }
            if (updateModel.Item.Username != itemUser.Username)
            {
                if (_repositoryUser.Get().Any(p => p.Username == updateModel.Item.Username))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Username));
                }
            }

            if (updateModel.Item.Email != itemUser.Email)
            {
                if (_repositoryUser.Get().Any(p => p.Email == updateModel.Item.Email))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Email));
                }
            }


            var versionUser = itemUser.Version;
            var versionPerson = person.Version;


            var personHistory = person.CreateMapped<Person, PersonHistory>();
            personHistory.Id = GuidHelper.NewGuid();
            personHistory.ReferenceId = person.Id;
            personHistory.CreatorId = _serviceMain.IdentityUser.Id;
            personHistory.CreationTime = DateTime.Now;
            _repositoryPersonHistory.Add(personHistory, true);

            person.FirstName = updateModel.Item.FirstName;
            person.LastName = updateModel.Item.LastName;

            person.LastModificationTime = DateTime.Now;
            person.LastModifierId = _serviceMain.IdentityUser.Id;
            person.Version = versionPerson + 1;
            var afffectedPerson = _repositoryPerson.Update(person, true);


            var userHistory = itemUser.CreateMapped<User, UserHistory>();
            userHistory.Id = GuidHelper.NewGuid();
            userHistory.ReferenceId = itemUser.Id;
            userHistory.PersonId = afffectedPerson.Id;
            userHistory.LanguageId = language.Id;
            userHistory.CreatorId = _serviceMain.IdentityUser.Id;
            userHistory.CreationTime = DateTime.Now;
            _repositoryUserHistory.Add(userHistory, true);

            itemUser.Username = updateModel.Item.Username;
            itemUser.Email = updateModel.Item.Email;
            itemUser.IsApproved = updateModel.Item.IsApproved;
            itemUser.Language = language;
            itemUser.Person = afffectedPerson;
            itemUser.LastModificationTime = DateTime.Now;
            itemUser.LastModifier = _serviceMain.IdentityUser;
            itemUser.Version = versionUser + 1;

            var affectedUser = _repositoryUser.Update(itemUser, true);

            foreach (var line in _repositoryRoleUserLine

                .Join(x => x.Role)
                .Join(x => x.User)
                .Where(x => x.User.Id == affectedUser.Id).ToList())
            {
                var lineHistory = line.CreateMapped<RoleUserLine, RoleUserLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.RoleId = line.Role.Id;
                lineHistory.UserId = line.User.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.CreatorId = _serviceMain.IdentityUser.Id;
                _repositoryRoleUserLineHistory.Add(lineHistory, true);
                _repositoryRoleUserLine.Delete(line, true);
            }

            foreach (var idCodeNameSelected in updateModel.Item.Roles)
            {
                var itemRole = _repositoryRole.Get(x => x.Id == idCodeNameSelected.Id);

                var affectedLine = _repositoryRoleUserLine.Add(new RoleUserLine
                {
                    Id = GuidHelper.NewGuid(),
                    User = affectedUser,
                    Role = itemRole,
                    Creator = _serviceMain.IdentityUser,
                    CreationTime = DateTime.Now,
                    DisplayOrder = 1,
                    LastModifier = _serviceMain.IdentityUser,
                    LastModificationTime = DateTime.Now,
                    Version = 1

                }, true);

                var lineHistory = affectedLine.CreateMapped<RoleUserLine, RoleUserLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = affectedLine.Id;
                lineHistory.RoleId = affectedLine.Role.Id;
                lineHistory.UserId = affectedLine.User.Id;
                lineHistory.CreatorId = affectedLine.Creator.Id;

                _repositoryRoleUserLineHistory.Add(lineHistory, true);
            }

            updateModel.Item = affectedUser.CreateMapped<User, UserModel>();

            updateModel.Item.Creator = new IdName(itemUser.Creator.Id, itemUser.Creator.Person.DisplayName);
            updateModel.Item.LastModifier = new IdName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Person.DisplayName);
            updateModel.Item.Language = new IdName(affectedUser.Language.Id, affectedUser.Language.Name);

            updateModel.Item.IdentityCode = itemUser.Person.IdentityCode;
            updateModel.Item.FirstName = itemUser.Person.FirstName;
            updateModel.Item.LastName = itemUser.Person.LastName;

            if (!_serviceMain.ApplicationSettings.SendMailAfterUpdateUserInformation) return updateModel;
            var emailUser = new EmailUser
            {
                Username = affectedUser.Username,
                Password = string.Empty,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = affectedUser.Person.FirstName,
                LastName = affectedUser.Person.LastName
            };
            var emailSender = new EmailSender(_serviceMain, _smtp);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.Update);
            return updateModel;


        }

        public void Delete(Guid id)
        {

            var identityUserMinRoleLevel = _serviceMain.IdentityUserMinRoleLevel;


            var item = _repositoryUser.Get(x => x.Id == id && x.RoleUserLines.All(t => t.Role.Level > identityUserMinRoleLevel));
            if (item == null)
            {
                throw new NotFoundException();
            }

            foreach (var line in _repositoryRoleUserLine

                .Join(x => x.Role)
                .Join(x => x.User)
                .Where(x => x.User.Id == item.Id).ToList())
            {
                var lineHistory = line.CreateMapped<RoleUserLine, RoleUserLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.RoleId = line.Role.Id;
                lineHistory.UserId = line.User.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.CreatorId = _serviceMain.IdentityUser.Id;
                _repositoryRoleUserLineHistory.Add(lineHistory, true);
                _repositoryRoleUserLine.Delete(line, true);
            }


            var itemHistory = item.CreateMapped<User, UserHistory>();

            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CreationTime = DateTime.Now;

            itemHistory.PersonId = item.Person.Id;
            itemHistory.LanguageId = item.Language.Id;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;
            itemHistory.IsDeleted = true;

            _repositoryUserHistory.Add(itemHistory, true);
            _repositoryUser.Delete(item, true);

        }

        public ListModel<UserModel> List(FilterModelWithMultiParent filterModel)
        {
            var model = filterModel.CreateMapped<FilterModelWithMultiParent, ListModel<UserModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, filterModel.Parents.Select(t => t.Id).ToList(), model);
        }

        public MyProfileModel MyProfile()
        {
            var lastLoginTime = DateTime.Now;
            MyProfileModel myProfileModel;
            UserModel userModel;
            var identity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
            var cacheKeyProfile = CacheKeyOption.Profile + "-" + identity.UserId;
            var identityUser = _repositoryUser
                .Join(x => x.Language)
                .Join(x => x.Person)
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.SessionsCreatedBy)
                .Join(x => x.SessionHistoriesCreatedBy)
                .FirstOrDefault(e => e.Id == identity.UserId);

            if (identityUser == null) throw new NotFoundException(Messages.DangerRecordNotFound);

            var menuList = new List<Menu>();

            var roleUserLines = _repositoryRoleUserLine

                .Join(x => x.Role)
                .Join(x => x.Role.RolePermissionLines)
                .Where(x => x.User.Id == identityUser.Id && x.Role.IsApproved).ToList();

            foreach (var roleUserLine in roleUserLines)
            {
                var role = roleUserLine.Role;

                var rolePermissionLines = _repositoryRolePermissionLine

                    .Join(x => x.Permission.PermissionMenuLines)
                    .Where(x => x.Role.Id == role.Id && x.Permission.IsApproved).OrderBy(x => x.Permission.DisplayOrder).ToList();

                foreach (var rolePermissionLine in rolePermissionLines)
                {
                    var permission = rolePermissionLine.Permission;

                    var permissionMenuLines = _repositoryPermissionMenuLine

                        .Join(x => x.Permission)
                          .Join(x => x.Menu.ParentMenu)

                          .Join(x => x.Menu.ChildMenus)
                        .Where(x => x.Permission.Id == permission.Id && x.Menu.IsApproved).OrderBy(x => x.Menu.DisplayOrder).ToList();


                    foreach (var permissionPermissionMenuLine in permissionMenuLines)
                    {
                        var menu = permissionPermissionMenuLine.Menu;
                        if (menuList.FirstOrDefault(x => x.Id == menu.Id) == null)
                        {
                            menuList.Add(menu);
                        }
                    }
                }

            }

            var rootMenus = new List<RootMenu>();
            foreach (var menuEntity in menuList.OrderBy(x => x.Code))
            {
                if (menuEntity.ParentMenu.Code != MenuConstants.AdminRootMenuCode) continue;
                var rootMenu = menuEntity.CreateMapped<Menu, RootMenu>();
                if (menuEntity.ChildMenus.Any())
                {
                    rootMenu.ChildMenus = new List<ChildMenu>();
                    foreach (var childMenuEntity in menuEntity.ChildMenus)
                    {
                        var childMenu = childMenuEntity.CreateMapped<Menu, ChildMenu>();

                        if (childMenuEntity.ChildMenus != null)
                        {

                            if (childMenuEntity.ChildMenus.Any())
                            {
                                childMenu.LeafMenus = new List<LeafMenu>();

                                foreach (var leafMenuEntity in childMenuEntity.ChildMenus)
                                {
                                    var leafMenu = leafMenuEntity.CreateMapped<Menu, LeafMenu>();
                                    leafMenu.Parent = childMenu;
                                    childMenu.LeafMenus.Add(leafMenu);
                                }


                            }
                        }

                        rootMenu.ChildMenus.Add(childMenu);
                    }
                }
                rootMenus.Add(rootMenu);
            }




            var sessionHistories = identityUser.SessionHistoriesCreatedBy;
            userModel = identityUser.CreateMapped<User, UserModel>();
            userModel.Languages = _languages;
            if (!(sessionHistories?.Count > 0))
            {

               
           
               
                userModel.Creator = new IdName(identityUser.Creator.Id, identityUser.Creator.Person.DisplayName);
                userModel.LastModifier = new IdName(identityUser.LastModifier.Id, identityUser.LastModifier.Person.DisplayName);
                userModel.Language = new IdName(identityUser.Language.Id, identityUser.Language.Name);

                userModel.IdentityCode = identityUser.Person.IdentityCode;
                userModel.FirstName = identityUser.Person.FirstName;
                userModel.LastName = identityUser.Person.LastName;

                userModel.Password = null;
                userModel.Roles = roleUserLines.Select(t => new IdCodeNameSelected(t.Role.Id, t.Role.Code, t.Role.Name, true)).ToList();
                myProfileModel = new MyProfileModel
                {
                    UserModel = userModel,
                    LastLoginTime = lastLoginTime,
                    RootMenus = rootMenus,
                };

                return myProfileModel;

            }

            var lastSession = sessionHistories.OrderByDescending(e => e.LastModificationTime).FirstOrDefault();
            if (lastSession != null)
            {
                lastLoginTime = lastSession.LastModificationTime;
            }

            userModel.Creator = new IdName(identityUser.Creator.Id, identityUser.Creator.Person.DisplayName);
            userModel.LastModifier = new IdName(identityUser.LastModifier.Id, identityUser.LastModifier.Person.DisplayName);
            userModel.Language = new IdName(identityUser.Language.Id, identityUser.Language.Name);

            userModel.IdentityCode = identityUser.Person.IdentityCode;
            userModel.FirstName = identityUser.Person.FirstName;
            userModel.LastName = identityUser.Person.LastName;
            userModel.Password = null;
            userModel.Roles = roleUserLines.Select(t => new IdCodeNameSelected(t.Role.Id, t.Role.Code, t.Role.Name, true)).ToList();

            myProfileModel = new MyProfileModel
            {
                UserModel = userModel,
                LastLoginTime = lastLoginTime,
                RootMenus = rootMenus
            };

            return myProfileModel;
        }



        public void UpdateMyPassword(UpdatePasswordModel model)
        {
            IValidator validator = new FluentValidator<UpdatePasswordModel, UpdatePasswordModelValidationRules>(model);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var identity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
            var user = _repositoryUser
                .Join(x => x.Language)
                .Join(x => x.Person)
                .FirstOrDefault(e => e.Id == identity.UserId);

            if (user == null)
            {
                throw new NotFoundException();
            }

            if (model.OldPassword.ToSha512() != user.Password)
            {
                throw new NotFoundException(Messages.DangerIncorrectOldPassword);
            }

            var userHistory = user.CreateMapped<User, UserHistory>();
            userHistory.Id = GuidHelper.NewGuid();
            userHistory.ReferenceId = user.Id;
            userHistory.CreatorId = user.Creator.Id;

            userHistory.PersonId = user.Person.Id;
            userHistory.LanguageId = user.Language.Id;


            _repositoryUserHistory.Add(userHistory, true);


            var password = model.Password;
            user.Password = password.ToSha512();
            user.LastModificationTime = DateTime.Now;
            user.LastModifier = user;
            var version = user.Version;
            user.Version = version + 1;
            var affectedUser = _repositoryUser.Update(user, true);
            if (!_serviceMain.ApplicationSettings.SendMailAfterUpdateUserPassword) return;

            var emailUser = new EmailUser
            {
                Username = affectedUser.Username,
                Password = password,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = affectedUser.Person.FirstName,
                LastName = affectedUser.Person.LastName
            };
            var emailSender = new EmailSender(_serviceMain, _smtp);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.UpdateMyPassword);
        }

        public void UpdateMyInformation(UpdateInformationModel model)
        {
            IValidator validator = new FluentValidator<UpdateInformationModel, UpdateInformationModelValidationRules>(model);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }


            var language = _repositoryLanguage.Get(e => e.Id == model.Language.Id);

            if (language == null)
            {
                throw new ParentNotFoundException();
            }


            var identity = (CustomIdentity)Thread.CurrentPrincipal?.Identity;
            var user = _repositoryUser
                .Join(x=>x.Creator)
                .Join(x => x.Language)
                .Join(x => x.Person)
                .FirstOrDefault(e => e.Id == identity.UserId);

            if (user == null)
            {
                throw new NotFoundException();
            }


            var person = user.Person;

            if (model.Username != user.Username)
            {
                if (_repositoryUser.Get().Any(p => p.Username == model.Username))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Username));
                }
            }
            if (model.Email != user.Email)
            {
                if (_repositoryUser.Get().Any(p => p.Email == model.Email))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Email));
                }
            }


            var personHistory = person.CreateMapped<Person, PersonHistory>();
            personHistory.Id = GuidHelper.NewGuid();
            personHistory.ReferenceId = user.Id;
            personHistory.CreatorId = user.Creator.Id;
            _repositoryPersonHistory.Add(personHistory, true);

            person.FirstName = model.FirstName;
            person.LastName = model.LastName;


            person.LastModificationTime = DateTime.Now;
            person.LastModifierId = user.Id;
            var versionPerson = person.Version;
            person.Version = versionPerson + 1;
            _repositoryPerson.Update(person, true);

            var userHistory = user.CreateMapped<User, UserHistory>();
            userHistory.Id = GuidHelper.NewGuid();
            userHistory.ReferenceId = user.Id;
            userHistory.CreatorId = user.Creator.Id;
            userHistory.PersonId = user.Person.Id;
            userHistory.LanguageId = user.Language.Id;



            _repositoryUserHistory.Add(userHistory, true);

            user.Username = model.Username;
            user.Email = model.Email;


            user.LastModificationTime = DateTime.Now;
            user.LastModifier = user;
            user.Language = language;

            var versionUser = user.Version;
            user.Version = versionUser + 1;

            var affectedUser = _repositoryUser.Update(user, true);
            if (!_serviceMain.ApplicationSettings.SendMailAfterUpdateUserInformation) return;

            var emailUser = new EmailUser
            {
                Username = affectedUser.Username,
                Password = string.Empty,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = affectedUser.Person.FirstName,
                LastName = affectedUser.Person.LastName
            };
            var emailSender = new EmailSender(_serviceMain, _smtp);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.UpdateMyInformation);
        }

    }
}
