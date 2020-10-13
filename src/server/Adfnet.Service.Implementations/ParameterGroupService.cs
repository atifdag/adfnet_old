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
    public class ParameterGroupService : IParameterGroupService
    {

        private readonly IRepository<ParameterGroup> _repositoryParameterGroup;
        private readonly IRepository<ParameterGroupHistory> _repositoryParameterGroupHistory;
        private readonly IRepository<Parameter> _repositoryParameter;
        private readonly IMainService _serviceMain;


        public ParameterGroupService(IRepository<ParameterGroup> repositoryParameterGroup, IRepository<ParameterGroupHistory> repositoryParameterGroupHistory, IRepository<Parameter> repositoryParameter, IMainService serviceMain)
        {
            _repositoryParameterGroup = repositoryParameterGroup;
            _repositoryParameterGroupHistory = repositoryParameterGroupHistory;

            _repositoryParameter = repositoryParameter;
            _serviceMain = serviceMain;
        }


        public ListModel<ParameterGroupModel> List(FilterModel filterModel)
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

            Expression<Func<ParameterGroup, bool>> expression;

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

            var listModel = filterModel.CreateMapped<FilterModel, ListModel<ParameterGroupModel>>();

            listModel.Paging ??= new Paging
            {
                PageSize = filterModel.PageSize,
                PageNumber = filterModel.PageNumber
            };

            var sortHelper = new SortHelper<ParameterGroup>();

            sortHelper.OrderBy(x => x.DisplayOrder);

            var query = (IOrderedQueryable<ParameterGroup>)_repositoryParameterGroup
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person).Where(expression);

            query = sortHelper.GenerateOrderedQuery(query);

            listModel.Paging.TotalItemCount = query.Count();

            var items = listModel.Paging.PageSize > 0 ? query.Skip((listModel.Paging.PageNumber - 1) * listModel.Paging.PageSize).Take(listModel.Paging.PageSize) : query;

            var modelItems = new HashSet<ParameterGroupModel>();

            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<ParameterGroup, ParameterGroupModel>();
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

        public DetailModel<ParameterGroupModel> Detail(Guid id)
        {

            var item = _repositoryParameterGroup.Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person).FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            var modelItem = item.CreateMapped<ParameterGroup, ParameterGroupModel>();

            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id, item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            return new DetailModel<ParameterGroupModel>
            {
                Item = modelItem
            };

        }

        public AddModel<ParameterGroupModel> Add()
        {
            return new AddModel<ParameterGroupModel>
            {
                Item = new ParameterGroupModel()
            };
        }

        public AddModel<ParameterGroupModel> Add(AddModel<ParameterGroupModel> addModel)
        {

            var validator = new FluentValidator<ParameterGroupModel, ParameterGroupValidationRules>(addModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = addModel.Item.CreateMapped<ParameterGroupModel, ParameterGroup>();

            if (_repositoryParameterGroup.Get().FirstOrDefault(e => e.Code == item.Code) != null)
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

            _repositoryParameterGroup.Add(item, true);

            var maxDisplayOrder = _repositoryParameterGroup.Get().Max(e => e.DisplayOrder);

            item.DisplayOrder = maxDisplayOrder + 1;

            var affectedItem = _repositoryParameterGroup.Update(item, true);

            addModel.Item = affectedItem.CreateMapped<ParameterGroup, ParameterGroupModel>();

            addModel.Item.Creator = new IdCodeName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Username, _serviceMain.IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Username, _serviceMain.IdentityUser.Person.DisplayName);

            return addModel;

        }

        public UpdateModel<ParameterGroupModel> Update(Guid id)
        {
            return new UpdateModel<ParameterGroupModel>
            {
                Item = Detail(id).Item
            };
        }


        public UpdateModel<ParameterGroupModel> Update(UpdateModel<ParameterGroupModel> updateModel)
        {

            IValidator validator = new FluentValidator<ParameterGroupModel, ParameterGroupValidationRules>(updateModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = _repositoryParameterGroup
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person).FirstOrDefault(e => e.Id == updateModel.Item.Id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            if (updateModel.Item.Code != item.Code)
            {
                if (_repositoryParameterGroup.Get().Any(p => p.Code == updateModel.Item.Code))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
                }
            }

            var itemHistory = item.CreateMapped<ParameterGroup, ParameterGroupHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.CreationTime = DateTime.Now;

            _repositoryParameterGroupHistory.Add(itemHistory, true);

            var version = item.Version;

            item.Code = updateModel.Item.Code;

            item.Name = updateModel.Item.Name;

            item.Description = updateModel.Item.Description;

            item.IsApproved = updateModel.Item.IsApproved;
            
            item.LastModificationTime = DateTime.Now;
            
            item.Version = version + 1;

            var affectedItem = _repositoryParameterGroup.Update(item, true);

            updateModel.Item = affectedItem.CreateMapped<ParameterGroup, ParameterGroupModel>();

            updateModel.Item.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);

            updateModel.Item.LastModifier = new IdCodeName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Username, _serviceMain.IdentityUser.Person.DisplayName);

            return updateModel;

        }

        public void Delete(Guid id)
        {
            if (_repositoryParameter.Get().Count(x => x.ParameterGroup.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }

            var item = _repositoryParameterGroup.Get().FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            var itemHistory = item.CreateMapped<ParameterGroup, ParameterGroupHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreationTime = DateTime.Now;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.IsDeleted = true;

            _repositoryParameterGroupHistory.Add(itemHistory, true);

            _repositoryParameterGroup.Delete(item, true);

        }

    }
}
