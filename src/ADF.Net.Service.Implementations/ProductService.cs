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
    public class ProductService : IProductService
    {

        private readonly IMainService _serviceMain;
        private readonly IRepository<Product> _repositoryProduct;
        private readonly IRepository<Category> _repositoryCategory;
        public ProductService(IMainService serviceMain, IRepository<Product> repositoryProduct, IRepository<Category> repositoryCategory)
        {
            _serviceMain = serviceMain;
            _repositoryProduct = repositoryProduct;
            _repositoryCategory = repositoryCategory;
        }

        public ListModel<ProductModel> List(FilterModel filterModel)
        {
            var startDate = filterModel.StartDate.ResetTimeToStartOfDay();
            var endDate = filterModel.EndDate.ResetTimeToEndOfDay();
            Expression<Func<Product, bool>> expression;

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

            var listModel = filterModel.CreateMapped<FilterModel, ListModel<ProductModel>>();

            listModel.Paging ??= new Paging
            {
                PageSize = filterModel.PageSize,
                PageNumber = filterModel.PageNumber
            };

            var sortHelper = new SortHelper<Product>();
            sortHelper.OrderBy(x => x.DisplayOrder);

            var query = (IOrderedQueryable<Product>)_repositoryProduct
                .Join(x=>x.Category)
                .Where(expression);

            query = sortHelper.GenerateOrderedQuery(query);

            listModel.Paging.TotalItemCount = query.Count();
            var items = listModel.Paging.PageSize > 0 ? query.Skip((listModel.Paging.PageNumber - 1) * listModel.Paging.PageSize).Take(listModel.Paging.PageSize) : query;
            var modelItems = new HashSet<ProductModel>();
            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<Product, ProductModel>();
                modelItem.Category = new Tuple<Guid, string, string>(item.Category.Id, item.Category.Code, item.Category.Name);
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

        public DetailModel<ProductModel> Detail(Guid id)
        {
            var item = _repositoryProduct
                .Join(x => x.Category)
                .FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException();
            }

            var modelItem = item.CreateMapped<Product, ProductModel>();
            modelItem.Category = new Tuple<Guid, string, string>(item.Category.Id, item.Category.Code, item.Category.Name);
            return new DetailModel<ProductModel>
            {

                Item = modelItem
            };
        }

        public AddModel<ProductModel> Add(AddModel<ProductModel> addModel)
        {
            
            var validator = new FluentValidator<ProductModel, ProductValidationRules>(addModel.Item);
            var validationResults = validator.Validate();
            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var parent = _repositoryCategory.Get().FirstOrDefault(e => e.Id == addModel.Item.Category.Item1);

            if (parent == null)
            {
                throw new ParentNotFoundException();
            }

            var item = addModel.Item.CreateMapped<ProductModel, Product>();

            if (_repositoryProduct.Get().FirstOrDefault(e => e.Code == item.Code) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
            }
            item.Id = GuidHelper.NewGuid();
            item.Version = 1;
            item.CreationTime = DateTime.Now;
            item.LastModificationTime = DateTime.Now;
            item.Category = parent;
            item.DisplayOrder = 1;

            _repositoryProduct.Add(item, true);
            var maxDisplayOrder = _repositoryProduct.Get().Max(e => e.DisplayOrder);
            item.DisplayOrder = maxDisplayOrder + 1;
            var affectedItem = _repositoryProduct.Update(item, true);

            addModel.Item = affectedItem.CreateMapped<Product, ProductModel>();
            addModel.Item.Category = new Tuple<Guid, string, string>(parent.Id, parent.Code, parent.Name);

            return addModel;
        }


        public UpdateModel<ProductModel> Update(UpdateModel<ProductModel> updateModel)
        {
            IValidator validator = new FluentValidator<ProductModel, ProductValidationRules>(updateModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var parent = _repositoryCategory.Get().FirstOrDefault(e => e.Id == updateModel.Item.Category.Item1);

            if (parent == null)
            {
                throw new ParentNotFoundException();
            }

            var item = _repositoryProduct
                .Join(x => x.Category)
                .FirstOrDefault(e => e.Id == updateModel.Item.Id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            if (updateModel.Item.Code != item.Code)
            {
                if (_repositoryProduct.Get().Any(p => p.Code == updateModel.Item.Code))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
                }
            }

            item.Code = updateModel.Item.Code;
            item.Name = updateModel.Item.Name;
            item.Description = updateModel.Item.Description;
            item.IsApproved = updateModel.Item.IsApproved;
            item.LastModificationTime = DateTime.Now;
            item.Category = parent;
            var version = item.Version;
            item.Version = version + 1;
            var affectedItem = _repositoryProduct.Update(item, true);
            updateModel.Item = affectedItem.CreateMapped<Product, ProductModel>();
            updateModel.Item.Category = new Tuple<Guid, string, string>(parent.Id, parent.Code, parent.Name);
            return updateModel;
        }

        public void Delete(Guid id)
        {
            var item = _repositoryProduct.Get(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException();
            }

            _repositoryProduct.Delete(item, true);

        }
    }
}
