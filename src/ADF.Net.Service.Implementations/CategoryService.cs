using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ADF.Net.Core;
using ADF.Net.Core.Exceptions;
using ADF.Net.Core.Globalization;
using ADF.Net.Core.Helpers;
using ADF.Net.Core.Validation.FluentValidation;
using ADF.Net.Core.ValueObjects;
using ADF.Net.Data;
using ADF.Net.Data.DataEntities;
using ADF.Net.Service.GenericCrudModels;
using ADF.Net.Service.Implementations.ValidationRules.FluentValidation;
using ADF.Net.Service.Models;

namespace ADF.Net.Service.Implementations
{
    public class CategoryService : ICategoryService
    {

        private readonly IMainService _serviceMain;

        private readonly IRepository<Category> _repositoryCategory;

        public CategoryService(IMainService serviceMain, IRepository<Category> repositoryCategory)
        {
            _serviceMain = serviceMain;
            _repositoryCategory = repositoryCategory;
        }

        public ListModel<CategoryModel> List(FilterModel filterModel)
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

            Expression<Func<Category, bool>> expression;

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

            var listModel = filterModel.CreateMapped<FilterModel, ListModel<CategoryModel>>();

            listModel.Paging ??= new Paging
            {
                PageSize = filterModel.PageSize,
                PageNumber = filterModel.PageNumber
            };

            var sortHelper = new SortHelper<Category>();

            sortHelper.OrderBy(x => x.DisplayOrder);

            var query = (IOrderedQueryable<Category>)_repositoryCategory.Get().Where(expression);

            query = sortHelper.GenerateOrderedQuery(query);

            listModel.Paging.TotalItemCount = query.Count();

            var items = listModel.Paging.PageSize > 0 ? query.Skip((listModel.Paging.PageNumber - 1) * listModel.Paging.PageSize).Take(listModel.Paging.PageSize) : query;

            var modelItems = new HashSet<CategoryModel>();

            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<Category, CategoryModel>();
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

        public DetailModel<CategoryModel> Detail(Guid id)
        {

            var item = _repositoryCategory.Get().FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            var modelItem = item.CreateMapped<Category, CategoryModel>();

            return new DetailModel<CategoryModel>
            {
                Item = modelItem
            };
        }

        public AddModel<CategoryModel> Add(AddModel<CategoryModel> addModel)
        {

            var validator = new FluentValidator<CategoryModel, CategoryValidationRules>(addModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = addModel.Item.CreateMapped<CategoryModel, Category>();

            if (_repositoryCategory.Get().FirstOrDefault(e => e.Code == item.Code) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
            }

            item.Id = GuidHelper.NewGuid();

            item.Version = 1;

            item.CreationTime = DateTime.Now;

            item.LastModificationTime = DateTime.Now;

            item.DisplayOrder = 1;

            _repositoryCategory.Add(item, true);

            var maxDisplayOrder = _repositoryCategory.Get().Max(e => e.DisplayOrder);

            item.DisplayOrder = maxDisplayOrder + 1;

            var affectedItem = _repositoryCategory.Update(item, true);

            addModel.Item = affectedItem.CreateMapped<Category, CategoryModel>();

            return addModel;
        }


        public UpdateModel<CategoryModel> Update(UpdateModel<CategoryModel> updateModel)
        {

            IValidator validator = new FluentValidator<CategoryModel, CategoryValidationRules>(updateModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = _repositoryCategory.Get().FirstOrDefault(e => e.Id == updateModel.Item.Id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            if (updateModel.Item.Code != item.Code)
            {
                if (_repositoryCategory.Get().Any(p => p.Code == updateModel.Item.Code))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
                }
            }

            var version = item.Version;

            item.Code = updateModel.Item.Code;

            item.Name = updateModel.Item.Name;

            item.Description = updateModel.Item.Description;

            item.IsApproved = updateModel.Item.IsApproved;

            item.LastModificationTime = DateTime.Now;
            
            item.Version = version + 1;

            var affectedItem = _repositoryCategory.Update(item, true);

            updateModel.Item = affectedItem.CreateMapped<Category, CategoryModel>();

            return updateModel;
        }

        public void Delete(Guid id)
        {
            var item = _repositoryCategory.Get().FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            _repositoryCategory.Delete(item, true);
        }
    }
}
