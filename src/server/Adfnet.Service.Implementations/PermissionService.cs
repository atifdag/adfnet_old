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
    public class PermissionService : IPermissionService
    {

        private readonly IRepository<Permission> _repositoryPermission;
        private readonly IRepository<PermissionHistory> _repositoryPermissionHistory;
        private readonly IRepository<RolePermissionLine> _repositoryRolePermissionLine;
        private readonly IRepository<PermissionMenuLine> _repositoryPermissionMenuLine;
        private readonly IRepository<PermissionMenuLineHistory> _repositoryPermissionMenuLineHistory;
        private readonly IRepository<Menu> _repositoryMenu;
        private readonly IMainService _serviceMain;



        public PermissionService(IRepository<Permission> repositoryPermission, IRepository<PermissionHistory> repositoryPermissionHistory, IRepository<RolePermissionLine> repositoryRolePermissionLine, IRepository<PermissionMenuLine> repositoryPermissionMenuLine, IRepository<Menu> repositoryMenu, IRepository<PermissionMenuLineHistory> repositoryPermissionMenuLineHistory, IMainService serviceMain)
        {
            _repositoryPermission = repositoryPermission;
            _repositoryPermissionHistory = repositoryPermissionHistory;

            _repositoryRolePermissionLine = repositoryRolePermissionLine;
            _repositoryPermissionMenuLine = repositoryPermissionMenuLine;
            _repositoryMenu = repositoryMenu;
            _repositoryPermissionMenuLineHistory = repositoryPermissionMenuLineHistory;
            _serviceMain = serviceMain;
        }

        public ListModel<PermissionModel> List(FilterModel filterModel)
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

            Expression<Func<Permission, bool>> expression;

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

            var listModel = filterModel.CreateMapped<FilterModel, ListModel<PermissionModel>>();

            listModel.Paging ??= new Paging
            {
                PageSize = filterModel.PageSize,
                PageNumber = filterModel.PageNumber
            };

            var sortHelper = new SortHelper<Permission>();

            sortHelper.OrderBy(x => x.DisplayOrder);

            var query = (IOrderedQueryable<Permission>)_repositoryPermission
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person).Where(expression);

            query = sortHelper.GenerateOrderedQuery(query);

            listModel.Paging.TotalItemCount = query.Count();

            var items = listModel.Paging.PageSize > 0 ? query.Skip((listModel.Paging.PageNumber - 1) * listModel.Paging.PageSize).Take(listModel.Paging.PageSize) : query;

            var modelItems = new HashSet<PermissionModel>();

            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<Permission, PermissionModel>();
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

        public DetailModel<PermissionModel> Detail(Guid id)
        {

            var item = _repositoryPermission
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.PermissionMenuLines)
                .FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }
            var listMenu = new List<IdCodeNameSelected>();
            var allMenu = _repositoryMenu.Get().Where(x => x.IsApproved).ToList();

            var itemMenus = _repositoryPermissionMenuLine
                .Join(x => x.Permission).Where(x => x.Permission.Id == item.Id).Select(x => x.Menu.Id);

            foreach (var itemMenu in allMenu)
            {
                listMenu.Add(itemMenus.Contains(itemMenu.Id) ? new IdCodeNameSelected(itemMenu.Id, itemMenu.Code, itemMenu.Name + " (Kod: " + itemMenu.Code + " | Adres: " + itemMenu.Address + ")", true) : new IdCodeNameSelected(itemMenu.Id, itemMenu.Code, itemMenu.Name + " (Kod: " + itemMenu.Code + " | Adres: " + itemMenu.Address + ")", false));
            }


            var modelItem = item.CreateMapped<Permission, PermissionModel>();
            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id, item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            modelItem.Menus = listMenu;
            return new DetailModel<PermissionModel>
            {
                Item = modelItem
            };

        }

        public AddModel<PermissionModel> Add()
        {
            return new AddModel<PermissionModel>
            {
                Item = new PermissionModel()
            };
        }

        public AddModel<PermissionModel> Add(AddModel<PermissionModel> addModel)
        {

            var validator = new FluentValidator<PermissionModel, PermissionValidationRules>(addModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = addModel.Item.CreateMapped<PermissionModel, Permission>();

            if (_repositoryPermission.Get().FirstOrDefault(e => e.Code == item.Code) != null)
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

            _repositoryPermission.Add(item, true);

            var maxDisplayOrder = _repositoryPermission.Get().Max(e => e.DisplayOrder);

            item.DisplayOrder = maxDisplayOrder + 1;

            var affectedItem = _repositoryPermission.Update(item, true);

            addModel.Item = affectedItem.CreateMapped<Permission, PermissionModel>();

            addModel.Item.Creator = new IdCodeName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Username, _serviceMain.IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Username, _serviceMain.IdentityUser.Person.DisplayName);

            return addModel;

        }

        public UpdateModel<PermissionModel> Update(Guid id)
        {
            return new UpdateModel<PermissionModel>
            {
                Item = Detail(id).Item
            };
        }


        public UpdateModel<PermissionModel> Update(UpdateModel<PermissionModel> updateModel)
        {

            IValidator validator = new FluentValidator<PermissionModel, PermissionValidationRules>(updateModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = _repositoryPermission
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person).FirstOrDefault(e => e.Id == updateModel.Item.Id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            if (updateModel.Item.Code != item.Code)
            {
                if (_repositoryPermission.Get().Any(p => p.Code == updateModel.Item.Code))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
                }
            }

            var itemHistory = item.CreateMapped<Permission, PermissionHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.CreationTime = DateTime.Now;

            _repositoryPermissionHistory.Add(itemHistory, true);

            var version = item.Version;

            item.Code = updateModel.Item.Code;

            item.Name = updateModel.Item.Name;

            item.Description = updateModel.Item.Description;

            item.ControllerName = updateModel.Item.ControllerName;

            item.ActionName = updateModel.Item.ActionName;

            item.IsApproved = updateModel.Item.IsApproved;
            
            item.LastModificationTime = DateTime.Now;
            
            item.Version = version + 1;

            var affectedItem = _repositoryPermission.Update(item, true);
            foreach (var line in _repositoryPermissionMenuLine
                           .Join(x => x.Menu)
                           .Join(x => x.Permission)
                           .Where(x => x.Permission.Id == affectedItem.Id).ToList())
            {
                var lineHistory = line.CreateMapped<PermissionMenuLine, PermissionMenuLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.MenuId = line.Menu.Id;
                lineHistory.PermissionId = line.Permission.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.CreatorId = _serviceMain.IdentityUser.Id;
                _repositoryPermissionMenuLineHistory.Add(lineHistory, true);
                _repositoryPermissionMenuLine.Delete(line, true);
            }

            foreach (var idCodeNameSelected in updateModel.Item.Menus)
            {
                var itemMenu = _repositoryMenu.Get(x => x.Id == idCodeNameSelected.Id);

                var affectedLine = _repositoryPermissionMenuLine.Add(new PermissionMenuLine
                {
                    Id = GuidHelper.NewGuid(),
                    Menu = itemMenu,
                    Permission = affectedItem,
                    Creator = _serviceMain.IdentityUser,
                    CreationTime = DateTime.Now,
                    DisplayOrder = 1,
                    LastModifier = _serviceMain.IdentityUser,
                    LastModificationTime = DateTime.Now,
                    Version = 1

                }, true);

                var lineHistory = affectedLine.CreateMapped<PermissionMenuLine, PermissionMenuLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = affectedLine.Id;
                lineHistory.MenuId = affectedLine.Menu.Id;
                lineHistory.PermissionId = affectedLine.Permission.Id;
                lineHistory.CreatorId = affectedLine.Creator.Id;

                _repositoryPermissionMenuLineHistory.Add(lineHistory, true);
            }



            updateModel.Item = affectedItem.CreateMapped<Permission, PermissionModel>();

            updateModel.Item.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);

            updateModel.Item.LastModifier = new IdCodeName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Username, _serviceMain.IdentityUser.Person.DisplayName);

            return updateModel;

        }

        public void Delete(Guid id)
        {
            if (_repositoryRolePermissionLine.Get().Count(x => x.Permission.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }
            if (_repositoryPermissionMenuLine.Get().Count(x => x.Permission.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }

            var item = _repositoryPermission.Get().FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            var itemHistory = item.CreateMapped<Permission, PermissionHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreationTime = DateTime.Now;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.IsDeleted = true;

            _repositoryPermissionHistory.Add(itemHistory, true);

            _repositoryPermission.Delete(item, true);

        }

        public List<IdCodeName> List()
        {

            var list = _repositoryPermission.Get().Where(x => x.IsApproved).OrderBy(x => x.DisplayOrder).Select(x => new IdCodeName(x.Id, x.Code, x.Name));
            if (list.Any())
            {
                return list.ToList();
            }

            throw new NotFoundException();
        }
    }
}
