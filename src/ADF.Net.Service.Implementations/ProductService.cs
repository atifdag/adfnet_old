using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ADF.Net.Core.Exceptions;
using ADF.Net.Core.Globalization;
using ADF.Net.Core.Helpers;
using ADF.Net.Core.ValueObjects;
using ADF.Net.Data;
using ADF.Net.Data.DataEntities;
using ADF.Net.Service.GenericCrudModels;
using ADF.Net.Service.Models;

namespace ADF.Net.Service.Implementations
{
    public class ProductService : IProductService
    {

        private readonly IMainService _serviceMain;
        private readonly IRepository<Product> _repositoryProduct;

        public ProductService(IMainService serviceMain, IRepository<Product> repositoryProduct)
        {
            _serviceMain = serviceMain;
            _repositoryProduct = repositoryProduct;
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

            var model = filterModel.CreateMapped<FilterModel, ListModel<ProductModel>>();

            model.Paging ??= new Paging
            {
                PageSize = filterModel.PageSize,
                PageNumber = filterModel.PageNumber
            };

            var sortHelper = new SortHelper<Product>();
            sortHelper.OrderBy(x => x.DisplayOrder);

            var query = (IOrderedQueryable<Product>)_repositoryProduct.Get()
                .Where(expression);

            query = sortHelper.GenerateOrderedQuery(query);

            model.Paging.TotalItemCount = query.Count();
            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;
            var modelItems = new HashSet<ProductModel>();
            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<Product, ProductModel>();
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

        public DetailModel<ProductModel> Detail(Guid id)
        {
            var item = _repositoryProduct.Get()
                .FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var modelItem = item.CreateMapped<Product, ProductModel>();
            return new DetailModel<ProductModel>
            {
                Item = modelItem
            };
        }

        public AddModel<ProductModel> Add()
        {
            throw new NotImplementedException();
        }

        public AddModel<ProductModel> Add(AddModel<ProductModel> addModel)
        {
            throw new NotImplementedException();
        }

        public UpdateModel<ProductModel> Update(Guid id)
        {
            throw new NotImplementedException();
        }

        public UpdateModel<ProductModel> Update(UpdateModel<ProductModel> updateModel)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
