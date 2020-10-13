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
    public class ParameterService : IParameterService
    {

        private readonly IRepository<Parameter> _repositoryParameter;
        private readonly IRepository<ParameterHistory> _repositoryParameterHistory;
        private readonly IRepository<ParameterGroup> _repositoryParameterGroup;
        private readonly IMainService _serviceMain;



        public ParameterService(IRepository<Parameter> repositoryParameter, IRepository<ParameterHistory> repositoryParameterHistory, IRepository<ParameterGroup> repositoryParameterGroup, IMainService serviceMain)
        {
            _repositoryParameter = repositoryParameter;
            _repositoryParameterHistory = repositoryParameterHistory;
            _repositoryParameterGroup = repositoryParameterGroup;
            _serviceMain = serviceMain;
        }

        public ListModel<ParameterModel> List(FilterModel filterModel)
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
            var model = filterModel.CreateMapped<FilterModel, ListModel<ParameterModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, Guid.Empty, model);
        }

        public ListModel<ParameterModel> List(FilterModelWithParent filterModel)
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

            var model = filterModel.CreateMapped<FilterModelWithParent, ListModel<ParameterModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, filterModel.Parent.Id, model);
        }

        private ListModel<ParameterModel> List(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, int status, string searched, Guid parentId, ListModel<ParameterModel> model)
        {
            var resetedStartDate = startDate.ResetTimeToStartOfDay();
            var resetedEndDate = endDate.ResetTimeToEndOfDay();

            Expression<Func<Parameter, bool>> expression;

            if (model.Paging == null)
            {
                model.Paging = new Paging
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };
            }

            if (status != -1)
            {
                var bStatus = status.ToString().ToBoolean();
                if (searched != null)
                {
                    if (bStatus)
                    {
                        expression = c => c.IsApproved && c.Key.Contains(searched);
                    }
                    else
                    {
                        expression = c => c.IsApproved == false && c.Key.Contains(searched);
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
                    expression = c => c.Key.Contains(searched);
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }

            expression = expression.And(e => e.CreationTime >= resetedStartDate && e.CreationTime <= resetedEndDate);
            if (parentId != Guid.Empty)
            {
                expression = expression.And(e => e.ParameterGroup.Id == parentId);
            }

            var sortHelper = new SortHelper<Parameter>();
            sortHelper.OrderBy(x => x.DisplayOrder);
            var query = (IOrderedQueryable<Parameter>)_repositoryParameter
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.ParameterGroup)
                .Where(expression);
            query = sortHelper.GenerateOrderedQuery(query);

            model.Paging.TotalItemCount = query.Count();
            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;
            var modelItems = new HashSet<ParameterModel>();
            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<Parameter, ParameterModel>();
                modelItem.Creator = new IdName(item.Creator.Id, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdName(item.LastModifier.Id, item.LastModifier.Person.DisplayName);
                modelItem.ParameterGroup = new IdCodeName(item.ParameterGroup.Id, item.ParameterGroup.Code, item.ParameterGroup.Name);

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

            // ilk sayfa ise

            if (model.Paging.PageNumber == 1)
            {
                if (model.Paging.TotalItemCount > 0)
                {
                    model.Paging.IsFirstPage = true;
                }

                // tek sayfa ise

                if (model.Paging.PageCount == 1)
                {
                    model.Paging.IsLastPage = true;
                }

            }

            // son sayfa ise
            else if (model.Paging.PageNumber == model.Paging.PageCount)
            {
                model.Paging.HasNextPage = false;
                // tek sayfa değilse

                if (model.Paging.PageCount > 1)
                {
                    model.Paging.IsLastPage = true;
                    model.Paging.HasPreviousPage = true;
                }
            }

            // ara sayfa ise
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

        public DetailModel<ParameterModel> Detail(Guid id)
        {

            var item = _repositoryParameter
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.ParameterGroup)

                .FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }
            var modelItem = item.CreateMapped<Parameter, ParameterModel>();
            modelItem.Creator = new IdName(item.Creator.Id, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdName(item.LastModifier.Id, item.LastModifier.Person.DisplayName);
            modelItem.ParameterGroup = new IdCodeName(item.ParameterGroup.Id, item.ParameterGroup.Code, item.ParameterGroup.Name);

            return new DetailModel<ParameterModel>
            {
                Item = modelItem
            };

        }

        public AddModel<ParameterModel> Add()
        {
            return new AddModel<ParameterModel>
            {
                Item = new ParameterModel
                {
                    IsApproved = false
                }
            };
        }

        public AddModel<ParameterModel> Add(AddModel<ParameterModel> addModel)
        {

            var validator = new FluentValidator<ParameterModel, ParameterValidationRules>(addModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var parent = _repositoryParameterGroup.Get(e => e.Id == addModel.Item.ParameterGroup.Id);

            if (parent == null)
            {
                throw new ParentNotFoundException();
            }

            var item = addModel.Item.CreateMapped<ParameterModel, Parameter>();

            if (_repositoryParameter.Get().FirstOrDefault(e => e.Key == item.Key) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Key));
            }

            item.Id = GuidHelper.NewGuid();

            item.Version = 1;

            item.CreationTime = DateTime.Now;

            item.LastModificationTime = DateTime.Now;

            item.ParameterGroup = parent;

            item.DisplayOrder = 1;

            item.Creator = _serviceMain.IdentityUser ?? throw new IdentityUserException(Messages.DangerIdentityUserNotFound);
            item.LastModifier = _serviceMain.IdentityUser;

            _repositoryParameter.Add(item, true);

            var maxDisplayOrder = _repositoryParameter.Get().Max(e => e.DisplayOrder);

            item.DisplayOrder = maxDisplayOrder + 1;

            var affectedItem = _repositoryParameter.Update(item, true);

            addModel.Item = affectedItem.CreateMapped<Parameter, ParameterModel>();

            addModel.Item.Creator = new IdName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Person.DisplayName);
            addModel.Item.ParameterGroup = new IdCodeName(parent.Id, parent.Code, parent.Name);
            return addModel;

        }

        public UpdateModel<ParameterModel> Update(Guid id)
        {
            return new UpdateModel<ParameterModel>
            {
                Item = Detail(id).Item
            };
        }


        public UpdateModel<ParameterModel> Update(UpdateModel<ParameterModel> updateModel)
        {

            IValidator validator = new FluentValidator<ParameterModel, ParameterValidationRules>(updateModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }


            var parent = _repositoryParameterGroup.Get(e => e.Id == updateModel.Item.ParameterGroup.Id);

            if (parent == null)
            {
                throw new ParentNotFoundException();
            }

            var item = _repositoryParameter
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person).FirstOrDefault(e => e.Id == updateModel.Item.Id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            if (updateModel.Item.Key != item.Key)
            {
                if (_repositoryParameter.Get().Any(p => p.Key == updateModel.Item.Key))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Key));
                }
            }

            var itemHistory = item.CreateMapped<Parameter, ParameterHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.CreationTime = DateTime.Now;

            _repositoryParameterHistory.Add(itemHistory, true);

            var version = item.Version;

            item.Key = updateModel.Item.Key;
            item.Value = updateModel.Item.Value;
            item.Erasable = updateModel.Item.Erasable;
            item.Description = updateModel.Item.Description;

            item.IsApproved = updateModel.Item.IsApproved;
            
            item.LastModificationTime = DateTime.Now;
            
            item.Version = version + 1;

            item.ParameterGroup = parent;

            var affectedItem = _repositoryParameter.Update(item, true);

            updateModel.Item = affectedItem.CreateMapped<Parameter, ParameterModel>();

            updateModel.Item.Creator = new IdName(item.Creator.Id, item.Creator.Person.DisplayName);

            updateModel.Item.LastModifier = new IdName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Person.DisplayName);

            updateModel.Item.ParameterGroup = new IdCodeName(item.ParameterGroup.Id, item.ParameterGroup.Code, item.ParameterGroup.Name);

            return updateModel;

        }

        public void Delete(Guid id)
        {
            var item = _repositoryParameter.Get().FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            var itemHistory = item.CreateMapped<Parameter, ParameterHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreationTime = DateTime.Now;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.IsDeleted = true;

            _repositoryParameterHistory.Add(itemHistory, true);

            _repositoryParameter.Delete(item, true);

        }

    }
}
