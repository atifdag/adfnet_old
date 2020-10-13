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
using Adfnet.Data.DataEntities;
using Adfnet.Service.Implementations.ValidationRules.FluentValidation;
using Adfnet.Service.Models;

namespace Adfnet.Service.Implementations
{
    public class LanguageService : ILanguageService
    {

        private readonly IRepository<Language> _repositoryLanguage;
        private readonly IRepository<LanguageHistory> _repositoryLanguageHistory;
        private readonly IMainService _serviceMain;
        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<CategoryLanguageLine> _repositoryCategoryLanguageLine;

        public LanguageService(IRepository<Language> repositoryLanguage, IRepository<LanguageHistory> repositoryLanguageHistory, IMainService serviceMain, IRepository<User> repositoryUser, IRepository<CategoryLanguageLine> repositoryCategoryLanguageLine)
        {
            _repositoryLanguage = repositoryLanguage;
            _repositoryLanguageHistory = repositoryLanguageHistory;
            _serviceMain = serviceMain;
            _repositoryUser = repositoryUser;
            _repositoryCategoryLanguageLine = repositoryCategoryLanguageLine;
        }


        public ListModel<LanguageModel> List(FilterModel filterModel)
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

            Expression<Func<Language, bool>> expression;

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

            var listModel = filterModel.CreateMapped<FilterModel, ListModel<LanguageModel>>();

            listModel.Paging ??= new Paging
            {
                PageSize = filterModel.PageSize,
                PageNumber = filterModel.PageNumber
            };

            var sortHelper = new SortHelper<Language>();

            sortHelper.OrderBy(x => x.DisplayOrder);

            var query = (IOrderedQueryable<Language>)_repositoryLanguage
                .Get().Where(expression);

            query = sortHelper.GenerateOrderedQuery(query);

            listModel.Paging.TotalItemCount = query.Count();

            var items = listModel.Paging.PageSize > 0 ? query.Skip((listModel.Paging.PageNumber - 1) * listModel.Paging.PageSize).Take(listModel.Paging.PageSize) : query;

            var modelItems = new HashSet<LanguageModel>();

            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<Language, LanguageModel>();

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

        public DetailModel<LanguageModel> Detail(Guid id)
        {

            var item = _repositoryLanguage.Get()
                .FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            var modelItem = item.CreateMapped<Language, LanguageModel>();

            return new DetailModel<LanguageModel>
            {
                Item = modelItem
            };

        }

        public AddModel<LanguageModel> Add()
        {
            return new AddModel<LanguageModel>
            {
                Item = new LanguageModel()
            };
        }

        public AddModel<LanguageModel> Add(AddModel<LanguageModel> addModel)
        {

            var validator = new FluentValidator<LanguageModel, LanguageValidationRules>(addModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = addModel.Item.CreateMapped<LanguageModel, Language>();

            if (_repositoryLanguage.Get().FirstOrDefault(e => e.Code == item.Code) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
            }

            item.Id = GuidHelper.NewGuid();

            item.Version = 1;

            item.CreationTime = DateTime.Now;

            item.LastModificationTime = DateTime.Now;

            item.CreatorId = _serviceMain.IdentityUser.Id;

            item.LastModifierId = _serviceMain.IdentityUser.Id;

            item.DisplayOrder = 1;

            _repositoryLanguage.Add(item, true);

            var maxDisplayOrder = _repositoryLanguage.Get().Max(e => e.DisplayOrder);

            item.DisplayOrder = maxDisplayOrder + 1;

            var affectedItem = _repositoryLanguage.Update(item, true);

            addModel.Item = affectedItem.CreateMapped<Language, LanguageModel>();

            return addModel;

        }

        public UpdateModel<LanguageModel> Update(Guid id)
        {
            return new UpdateModel<LanguageModel>
            {
                Item = Detail(id).Item
            };
        }


        public UpdateModel<LanguageModel> Update(UpdateModel<LanguageModel> updateModel)
        {

            IValidator validator = new FluentValidator<LanguageModel, LanguageValidationRules>(updateModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = _repositoryLanguage
                .Get()
                .FirstOrDefault(e => e.Id == updateModel.Item.Id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            if (updateModel.Item.Code != item.Code)
            {
                if (_repositoryLanguage.Get().Any(p => p.Code == updateModel.Item.Code))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
                }
            }

            var itemHistory = item.CreateMapped<Language, LanguageHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.CreationTime = DateTime.Now;

            _repositoryLanguageHistory.Add(itemHistory, true);

            var version = item.Version;

            item.Code = updateModel.Item.Code;

            item.Name = updateModel.Item.Name;

            item.Description = updateModel.Item.Description;

            item.IsApproved = updateModel.Item.IsApproved;
            
            item.LastModificationTime = DateTime.Now;

            item.LastModifierId = _serviceMain.IdentityUser.Id;

            item.Version = version + 1;

            var affectedItem = _repositoryLanguage.Update(item, true);

            updateModel.Item = affectedItem.CreateMapped<Language, LanguageModel>();


            return updateModel;

        }

        public void Delete(Guid id)
        {
            if (_repositoryUser.Get().Count(x => x.Language.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }

            if (_repositoryCategoryLanguageLine.Get().Count(x => x.Language.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }
            
            var item = _repositoryLanguage.Get().FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            var itemHistory = item.CreateMapped<Language, LanguageHistory>();

            itemHistory.Id = GuidHelper.NewGuid();

            itemHistory.ReferenceId = item.Id;

            itemHistory.CreationTime = DateTime.Now;

            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            itemHistory.IsDeleted = true;

            _repositoryLanguageHistory.Add(itemHistory, true);

            _repositoryLanguage.Delete(item, true);

        }

    }
}
