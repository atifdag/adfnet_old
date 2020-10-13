using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Adfnet.Core;
using Adfnet.Core.Exceptions;
using Adfnet.Core.GenericCrudModels;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Core.Validation.FluentValidation;
using Adfnet.Core.ValueObjects;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Data.DataEntities;
using Adfnet.Service.Implementations.ValidationRules.FluentValidation;
using Adfnet.Service.Models;

namespace Adfnet.Service.Implementations
{
    public class RoleService : IRoleService
    {

        private readonly IRepository<Role> _repositoryRole;
        private readonly IRepository<RoleHistory> _repositoryRoleHistory;
        private readonly IRepository<RolePermissionLine> _repositoryRolePermissionLine;
        private readonly IRepository<RoleUserLine> _repositoryRoleUserLine;
        private readonly IRepository<Permission> _repositoryPermission;
        private readonly IRepository<RolePermissionLineHistory> _repositoryRolePermissionLineHistory;
        private readonly IMainService _serviceMain;


        public RoleService(IRepository<Role> repositoryRole, IRepository<RoleHistory> repositoryRoleHistory, IRepository<RolePermissionLine> repositoryRolePermissionLine, IRepository<RoleUserLine> repositoryRoleUserLine, IRepository<Permission> repositoryPermission, IRepository<RolePermissionLineHistory> repositoryRolePermissionLineHistory, IMainService serviceMain)
        {
            _repositoryRole = repositoryRole;
            _repositoryRoleHistory = repositoryRoleHistory;
            _repositoryRolePermissionLine = repositoryRolePermissionLine;

            _repositoryRoleUserLine = repositoryRoleUserLine;
            _repositoryPermission = repositoryPermission;
            _repositoryRolePermissionLineHistory = repositoryRolePermissionLineHistory;
            _serviceMain = serviceMain;
        }

        public ListModel<RoleModel> List(FilterModel filterModel)
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


            var startDate = filterModel.StartDate.ResetTimeToStartOfDay();

            var endDate = filterModel.EndDate.ResetTimeToEndOfDay();

            Expression<Func<Role, bool>> expression;

            if (filterModel.Status != -1)
            {
                var status = filterModel.Status.ToString().ToBoolean();

                if (filterModel.Searched != null)
                {
                    if (status)
                    {
                        expression = c => c.IsApproved && c.Name.Contains(filterModel.Searched);
                    }
                    else
                    {
                        expression = c => c.IsApproved == false && c.Name.Contains(filterModel.Searched);
                    }
                }
                else
                {
                    if (status)
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
                if (filterModel.Searched != null)
                {
                    expression = c => c.Name.Contains(filterModel.Searched);
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }

            expression = expression.And(e => e.CreationTime >= startDate && e.CreationTime <= endDate);

            var listModel = filterModel.CreateMapped<FilterModel, ListModel<RoleModel>>();

            listModel.Paging ??= new Paging
            {
                PageSize = filterModel.PageSize,
                PageNumber = filterModel.PageNumber
            };

            var sortHelper = new SortHelper<Role>();

            sortHelper.OrderBy(x => x.DisplayOrder);

            var query = (IOrderedQueryable<Role>)_repositoryRole
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person).Where(expression);

            query = sortHelper.GenerateOrderedQuery(query);

            listModel.Paging.TotalItemCount = query.Count();

            var items = listModel.Paging.PageSize > 0 ? query.Skip((listModel.Paging.PageNumber - 1) * listModel.Paging.PageSize).Take(listModel.Paging.PageSize) : query;

            var modelItems = new HashSet<RoleModel>();

            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<Role, RoleModel>();
                modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdCodeName(item.LastModifier.Id, item.LastModifier.Username, item.LastModifier.Person.DisplayName);

                modelItems.Add(modelItem);
            }

            listModel.Items = modelItems.ToList();

            var pageSizeDescription = _serviceMain.ApplicationSettings.PageSizeList;

            var pageSizes = pageSizeDescription.Split(',').Select(s => new KeyValuePair<int, string>(s.ToInt(), s)).ToList();

            pageSizes.Insert(0, new KeyValuePair<int, string>(-1, "[" + Dictionary.All + "]"));

            listModel.Paging.PageSizes = pageSizes;

            listModel.Paging.PageCount = (int)Math.Ceiling((float)listModel.Paging.TotalItemCount / listModel.Paging.PageSize);

            if (listModel.Paging.TotalItemCount > listModel.Items.Count)
            {
                listModel.Paging.HasNextPage = true;
            }

            if (listModel.Paging.PageNumber == 1)
            {
                if (listModel.Paging.TotalItemCount > 0)
                {
                    listModel.Paging.IsFirstPage = true;
                }


                if (listModel.Paging.PageCount == 1)
                {
                    listModel.Paging.IsLastPage = true;
                }

            }

            else if (listModel.Paging.PageNumber == listModel.Paging.PageCount)
            {
                listModel.Paging.HasNextPage = false;

                if (listModel.Paging.PageCount > 1)
                {
                    listModel.Paging.IsLastPage = true;
                    listModel.Paging.HasPreviousPage = true;
                }
            }

            else
            {
                listModel.Paging.HasNextPage = true;
                listModel.Paging.HasPreviousPage = true;
            }

            if (listModel.Paging.TotalItemCount > listModel.Items.Count && listModel.Items.Count <= 0)
            {
                listModel.Message = Messages.DangerRecordNotFoundInPage;
            }

            if (listModel.Paging.TotalItemCount == 0)
            {
                listModel.Message = Messages.DangerRecordNotFound;
            }

            return listModel;

        }

        public DetailModel<RoleModel> Detail(Guid id)
        {

            var item = _repositoryRole
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.RolePermissionLines).FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            var listPermission = new List<IdCodeNameSelected>();
            var allPermission = _repositoryPermission.Get().Where(x => x.IsApproved).ToList();

            var itemPermissions = _repositoryRolePermissionLine
                .Join(x => x.Permission)
                .Where(x => x.Role.Id == item.Id).Select(x => x.Permission.Id);

            foreach (var itemPermission in allPermission)
            {
                listPermission.Add(itemPermissions.Contains(itemPermission.Id) ? new IdCodeNameSelected(itemPermission.Id, itemPermission.Code, itemPermission.Name, true) : new IdCodeNameSelected(itemPermission.Id, itemPermission.Code, itemPermission.Name, false));
            }

            var modelItem = item.CreateMapped<Role, RoleModel>();
            modelItem.Permissions = listPermission;

            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id, item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            return new DetailModel<RoleModel>
            {
                Item = modelItem
            };

        }

        public AddModel<RoleModel> Add()
        {
            return new AddModel<RoleModel>
            {
                Item = new RoleModel()
            };
        }

        public AddModel<RoleModel> Add(AddModel<RoleModel> addModel)
        {

            var validator = new FluentValidator<RoleModel, RoleValidationRules>(addModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = addModel.Item.CreateMapped<RoleModel, Role>();

            if (_repositoryRole.Get().FirstOrDefault(e => e.Code == item.Code) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
            }

            item.Id = GuidHelper.NewGuid();

            item.Version = 1;

            item.CreationTime = DateTime.Now;

            item.LastModificationTime = DateTime.Now;

            item.DisplayOrder = 1;

            item.Creator = _serviceMain.IdentityUser ?? throw new IdentityUserException(Messages.DangerIdentityUserNotFound);
            item.LastModifier = _serviceMain.IdentityUser;

            _repositoryRole.Add(item, true);

            var maxDisplayOrder = _repositoryRole.Get().Max(e => e.DisplayOrder);

            item.DisplayOrder = maxDisplayOrder + 1;

            var affectedItem = _repositoryRole.Update(item, true);

            addModel.Item = affectedItem.CreateMapped<Role, RoleModel>();

            addModel.Item.Creator = new IdCodeName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Username, _serviceMain.IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Username, _serviceMain.IdentityUser.Person.DisplayName);

            return addModel;

        }

        public UpdateModel<RoleModel> Update(Guid id)
        {
            return new UpdateModel<RoleModel>
            {
                Item = Detail(id).Item
            };
        }


        public UpdateModel<RoleModel> Update(UpdateModel<RoleModel> updateModel)
        {

            IValidator validator = new FluentValidator<RoleModel, RoleValidationRules>(updateModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = _repositoryRole
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person).FirstOrDefault(e => e.Id == updateModel.Item.Id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            if (updateModel.Item.Code != item.Code)
            {
                if (_repositoryRole.Get().Any(p => p.Code == updateModel.Item.Code))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
                }
            }

            var itemHistory = item.CreateMapped<Role, RoleHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.CreationTime = DateTime.Now;

            _repositoryRoleHistory.Add(itemHistory, true);

            var version = item.Version;

            item.Code = updateModel.Item.Code;

            item.Name = updateModel.Item.Name;

            item.Description = updateModel.Item.Description;

            item.Level = updateModel.Item.Level;

            item.IsApproved = updateModel.Item.IsApproved;
            
            item.LastModificationTime = DateTime.Now;
            
            item.Version = version + 1;

            var affectedItem = _repositoryRole.Update(item, true);

            foreach (var line in _repositoryRolePermissionLine
                .Join(x => x.Role)
                .Join(x => x.Permission)
                .Where(x => x.Role.Id == affectedItem.Id).ToList())
            {
                var lineHistory = line.CreateMapped<RolePermissionLine, RolePermissionLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.RoleId = line.Role.Id;
                lineHistory.PermissionId = line.Permission.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.CreatorId = _serviceMain.IdentityUser.Id;
                _repositoryRolePermissionLineHistory.Add(lineHistory, true);
                _repositoryRolePermissionLine.Delete(line, true);
            }

            foreach (var idCodeNameSelected in updateModel.Item.Permissions)
            {
                var itemPermission = _repositoryPermission.Get(x => x.Id == idCodeNameSelected.Id);

                var affectedLine = _repositoryRolePermissionLine.Add(new RolePermissionLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = itemPermission,
                    Role = affectedItem,
                    Creator = _serviceMain.IdentityUser,
                    CreationTime = DateTime.Now,
                    DisplayOrder = 1,
                    LastModifier = _serviceMain.IdentityUser,
                    LastModificationTime = DateTime.Now,
                    Version = 1

                }, true);

                var lineHistory = affectedLine.CreateMapped<RolePermissionLine, RolePermissionLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = affectedLine.Id;
                lineHistory.RoleId = affectedLine.Role.Id;
                lineHistory.PermissionId = affectedLine.Permission.Id;
                lineHistory.CreatorId = affectedLine.Creator.Id;

                _repositoryRolePermissionLineHistory.Add(lineHistory, true);
            }


            updateModel.Item = affectedItem.CreateMapped<Role, RoleModel>();

            updateModel.Item.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);

            updateModel.Item.LastModifier = new IdCodeName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Username, _serviceMain.IdentityUser.Person.DisplayName);

            return updateModel;

        }

        public void Delete(Guid id)
        {
            if (_repositoryRolePermissionLine.Get().Count(x => x.Role.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }
            if (_repositoryRoleUserLine.Get().Count(x => x.Role.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }
            
            var item = _repositoryRole.Get().FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            var itemHistory = item.CreateMapped<Role, RoleHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreationTime = DateTime.Now;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.IsDeleted = true;

            _repositoryRoleHistory.Add(itemHistory, true);

            _repositoryRole.Delete(item, true);

        }

        public List<IdCodeName> List()
        {
            var identityUserMinRoleLevel = _serviceMain.IdentityUser.RoleUserLines.Select(x => x.Role.Level).Min();

            var list = _repositoryRole.Get().Where(x => x.IsApproved && x.Level > identityUserMinRoleLevel).OrderBy(x => x.DisplayOrder).Select(x => new IdCodeName(x.Id, x.Code, x.Name));
            if (list.Any())
            {
                return list.ToList();
            }

            throw new NotFoundException();
        }
    }
}
