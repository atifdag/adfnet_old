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
    public class CategoryService : ICategoryService
    {

        private readonly IRepository<Category> _repositoryCategory;
        private readonly IRepository<CategoryHistory> _repositoryCategoryHistory;
        private readonly IRepository<CategoryLanguageLine> _repositoryCategoryLanguageLine;
        private readonly IRepository<CategoryLanguageLineHistory> _repositoryCategoryLanguageLineHistory;
        private readonly IMainService _serviceMain;
        private readonly IRepository<Language> _repositoryLanguage;

        public CategoryService(IRepository<Category> repositoryCategory, IRepository<CategoryHistory> repositoryCategoryHistory, IRepository<CategoryLanguageLine> repositoryCategoryLanguageLine, IRepository<CategoryLanguageLineHistory> repositoryCategoryLanguageLineHistory, IMainService serviceMain, IRepository<Language> repositoryLanguage)
        {
            _repositoryCategory = repositoryCategory;
            _repositoryCategoryHistory = repositoryCategoryHistory;
            _repositoryCategoryLanguageLine = repositoryCategoryLanguageLine;
            _repositoryCategoryLanguageLineHistory = repositoryCategoryLanguageLineHistory;
            _serviceMain = serviceMain;
            _repositoryLanguage = repositoryLanguage;
        }

        public ListModel<CategoryModel> List(FilterModel filterModel)
        {
            return List(filterModel, _serviceMain.DefaultLanguage.Id);
        }

        public ListModel<CategoryModel> List(FilterModel filterModel, Guid languageId)
        {
           var model = filterModel.CreateMapped<FilterModel, ListModel<CategoryModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, languageId, model);
        }

        public DetailModel<CategoryModel> Detail(Guid id)
        {
            return Detail(id, _serviceMain.DefaultLanguage.Id);
        }

        private ListModel<CategoryModel> List(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, int status, string searched, Guid languageId, ListModel<CategoryModel> listModel)
        {

            var resetedStartDate = startDate.ResetTimeToStartOfDay();
            var resetedEndDate = endDate.ResetTimeToEndOfDay();
            var language = languageId != Guid.Empty ? _repositoryLanguage.Get(x => x.Id == languageId) : _serviceMain.DefaultLanguage;

            Expression<Func<Category, bool>> expression;

            if (status != -1)
            {
                var bStatus = status.ToString().ToBoolean();
                if (searched != null)
                {
                    if (bStatus)
                    {
                        expression = c => c.CategoryLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.CategoryLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved == false);
                    }
                }
                else
                {
                    if (bStatus)
                    {
                        expression = c => c.CategoryLanguageLines.Any(x => x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.CategoryLanguageLines.Any(x => x.IsApproved == false);
                    }
                }

            }
            else
            {
                if (searched != null)
                {
                    expression = c => c.CategoryLanguageLines.Any(x => x.Name.Contains(searched));
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }


            expression = expression.And(e => e.CreationTime >= resetedStartDate && e.CreationTime <= resetedEndDate);


            listModel.Paging ??= new Paging
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            var sortHelper = new SortHelper<Category>();

            sortHelper.OrderBy(x => x.Code);
            var query = (IOrderedQueryable<Category>)_repositoryCategory
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.CategoryLanguageLines)
                .ThenJoin(x => x.Language)
                .Where(expression);

            query = sortHelper.GenerateOrderedQuery(query);

            listModel.Paging.TotalItemCount = query.Count();

            var items = listModel.Paging.PageSize > 0 ? query.Skip((listModel.Paging.PageNumber - 1) * listModel.Paging.PageSize).Take(listModel.Paging.PageSize) : query;

            var modelItems = new HashSet<CategoryModel>();

            foreach (var item in items)
            {
                CategoryModel modelItem;
                if (item.CategoryLanguageLines == null)
                {
                    modelItem = new CategoryModel();
                }
                else
                {
                    var itemLine = item.CategoryLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);
                    modelItem = itemLine != null ? itemLine.CreateMapped<CategoryLanguageLine, CategoryModel>() : new CategoryModel();
                }

                modelItem.Creator = new IdName(item.Creator.Id, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdName(item.LastModifier.Id, item.LastModifier.Person.DisplayName);
                modelItem.Language = new IdCodeName(language.Id, language.Code, language.Name);

                modelItem.Category = new IdCode(item.Id, item.Code);

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

            // ilk sayfa ise

            if (listModel.Paging.PageNumber == 1)
            {
                if (listModel.Paging.TotalItemCount > 0)
                {
                    listModel.Paging.IsFirstPage = true;
                }

                // tek sayfa ise

                if (listModel.Paging.PageCount == 1)
                {
                    listModel.Paging.IsLastPage = true;
                }

            }

            // son sayfa ise
            else if (listModel.Paging.PageNumber == listModel.Paging.PageCount)
            {
                listModel.Paging.HasNextPage = false;
                // tek sayfa değilse

                if (listModel.Paging.PageCount > 1)
                {
                    listModel.Paging.IsLastPage = true;
                    listModel.Paging.HasPreviousPage = true;
                }
            }

            // ara sayfa ise
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

        public DetailModel<CategoryModel> Detail(Guid id, Guid languageId)
        {

            var language = _repositoryLanguage.Get(x => x.Id == languageId);

            var item = _repositoryCategory
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.CategoryLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException();
            }

            CategoryModel modelItem;
            if (item.CategoryLanguageLines == null)
            {
                modelItem = new CategoryModel();
            }
            else
            {
                var itemLine = item.CategoryLanguageLines.FirstOrDefault(x => x.Language.Id == languageId);
                if (itemLine != null)
                {
                    modelItem = itemLine.CreateMapped<CategoryLanguageLine, CategoryModel>();
                    modelItem.Creator = new IdName(itemLine.Creator.Id, itemLine.Creator.Person.DisplayName);
                    modelItem.LastModifier = new IdName(itemLine.LastModifier.Id, itemLine.LastModifier.Person.DisplayName);
                }
                else
                {
                    modelItem = new CategoryModel();
                }

            }

            modelItem.Creator = new IdName(item.Creator.Id, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdName(item.LastModifier.Id, item.LastModifier.Person.DisplayName);
            modelItem.Language = new IdCodeName(language.Id, language.Code, language.Name);
            modelItem.Category = new IdCode(item.Id, item.Code);

            return new DetailModel<CategoryModel>
            {
                Item = modelItem
            };

        }

        public AddModel<CategoryModel> Add()
        {
            return new AddModel<CategoryModel>
            {
                Item = new CategoryModel
                {
                    IsApproved = true
                }
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

            var language = _repositoryLanguage.Get(e => e.Id == addModel.Item.Language.Id);

            if (language == null)
            {
                throw new ParentNotFoundException();
            }

            var line = addModel.Item.CreateMapped<CategoryModel, CategoryLanguageLine>();

            var item = new Category
            {
                Code = addModel.Item.Code,
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                Creator = _serviceMain.IdentityUser,

                LastModificationTime = DateTime.Now,
                LastModifier = _serviceMain.IdentityUser

            };

            var affectedItem = _repositoryCategory.Add(item, true);

            var maxLineDisplayOrder = _repositoryCategoryLanguageLine.Get().Where(x => x.Language.Id == addModel.Item.Language.Id).Max(e => e.DisplayOrder);

            line.Id = GuidHelper.NewGuid();
            line.Version = 1;
            line.DisplayOrder = maxLineDisplayOrder + 1;
            line.CreationTime = DateTime.Now;
            line.Language = language;
            line.Category = affectedItem;
            line.LastModificationTime = DateTime.Now;
            line.Creator = _serviceMain.IdentityUser;
            line.LastModifier = _serviceMain.IdentityUser;
            _repositoryCategoryLanguageLine.Add(line, true);


            addModel.Item = affectedItem.CreateMapped<Category, CategoryModel>();



            addModel.Item.Creator = new IdName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Person.DisplayName);
            addModel.Item.Language = new IdCodeName(language.Id, language.Code, language.Name);
            return addModel;

        }

        public UpdateModel<CategoryModel> Update(Guid id)
        {
            return Update(id, _serviceMain.DefaultLanguage.Id);
        }

        public UpdateModel<CategoryModel> Update(Guid id, Guid languageId)
        {
            return new UpdateModel<CategoryModel>
            {
                Item = Detail(id, languageId).Item
            };
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

            var language = _repositoryLanguage.Get(e => e.Id == updateModel.Item.Language.Id);

            if (language == null)
            {
                throw new ParentNotFoundException();
            }

            var item = _repositoryCategory
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.CategoryLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == updateModel.Item.Category.Id);

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


            CategoryModel modelItem;
            if (item.CategoryLanguageLines == null)
            {
                modelItem = new CategoryModel();
            }
            else
            {
                var itemLine = item.CategoryLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);

                // güncelleme yapılacak
                if (itemLine != null)
                {
                    var lineHistory = itemLine.CreateMapped<CategoryLanguageLine, CategoryLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = itemLine.Id;
                    lineHistory.CreatorId = _serviceMain.IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.CategoryId = item.Id;
                    lineHistory.LanguageId = language.Id;
                    _repositoryCategoryLanguageLineHistory.Add(lineHistory, true);

                    itemLine.Name = updateModel.Item.Name;
                    itemLine.Language = language;
                    itemLine.Category = item;
                    itemLine.Code = item.Code+"-"+ language.Code;

                    itemLine.Description = updateModel.Item.Description;
                    var version = itemLine.Version;
                    itemLine.Version = version + 1;

                    itemLine.IsApproved = updateModel.Item.IsApproved;
                    itemLine.Category = item;
                    itemLine.LastModifier = _serviceMain.IdentityUser;
                    itemLine.LastModificationTime = DateTime.Now;
                    var affectedItemLine = _repositoryCategoryLanguageLine.Update(itemLine, true);
                    modelItem = affectedItemLine.CreateMapped<CategoryLanguageLine, CategoryModel>();


                    modelItem.Creator = new IdName(itemLine.Creator.Id, itemLine.Creator.Person.DisplayName);
                    modelItem.LastModifier = new IdName(_serviceMain.IdentityUser.Id, _serviceMain.IdentityUser.Person.DisplayName);

                }

                // ekleme yapılacak
                else
                {
                    modelItem = new CategoryModel();

                    var maxLineDisplayOrder = _repositoryCategoryLanguageLine.Get().Where(x => x.Language.Id == language.Id).Max(e => e.DisplayOrder);

                    var line = updateModel.Item.CreateMapped<CategoryModel, CategoryLanguageLine>();

                    line.Id = GuidHelper.NewGuid();
                    line.Version = 1;
                    line.DisplayOrder = maxLineDisplayOrder + 1;
                    line.CreationTime = DateTime.Now;
                    line.Language = language;
                    line.Category = item;
                    line.Code = item.Code + "-" + language.Code;
                    line.LastModificationTime = DateTime.Now;
                    line.Creator = _serviceMain.IdentityUser;
                    line.LastModifier = _serviceMain.IdentityUser;
                    var affectedLine = _repositoryCategoryLanguageLine.Add(line, true);

                    var lineHistory = affectedLine.CreateMapped<CategoryLanguageLine, CategoryLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = affectedLine.Id;
                    lineHistory.CreatorId = _serviceMain.IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.CategoryId = affectedLine.Category.Id;
                    lineHistory.LanguageId = affectedLine.Language.Id;

                    _repositoryCategoryLanguageLineHistory.Add(lineHistory, true);


                }
            }

            var itemHistory = item.CreateMapped<Category, CategoryHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CreatorId = _serviceMain.IdentityUser.Id;

            _repositoryCategoryHistory.Add(itemHistory, true);
            item.LastModificationTime = DateTime.Now;
            item.LastModifier = _serviceMain.IdentityUser;
            item.Code = updateModel.Item.Code;

            var affectedItem = _repositoryCategory.Update(item, true);
            modelItem.Category = new IdCode(affectedItem.Id, affectedItem.Code);
            modelItem.Language = new IdCodeName(language.Id, language.Code, language.Name);

            updateModel.Item = modelItem;

            return updateModel;

        }

        public void Delete(Guid id)
        {
            Delete(id, _serviceMain.DefaultLanguage.Id);
        }


        public void Delete(Guid id, Guid languageId)
        {
            // diğer dillerde kayıt yoksa bu kayıt da silinecek

            var line = _repositoryCategoryLanguageLine
                .Join(x=>x.Language)
                .FirstOrDefault(x => x.Category.Id == id && x.Language.Id == languageId);
            if (line == null)
            {
                throw new NotFoundException();
            }

            var item = _repositoryCategory.Get(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException();
            }

            var lineHistory = line.CreateMapped<CategoryLanguageLine, CategoryLanguageLineHistory>();

            lineHistory.Id = GuidHelper.NewGuid();
            lineHistory.ReferenceId = line.Id;
            lineHistory.CreationTime = DateTime.Now;
            lineHistory.CategoryId = line.Category.Id;
            lineHistory.LanguageId = line.Language.Id;
            lineHistory.CreatorId = _serviceMain.IdentityUser.Id;

            _repositoryCategoryLanguageLineHistory.Add(lineHistory, true);
            _repositoryCategoryLanguageLine.Delete(line, true);

            if (_repositoryCategoryLanguageLine.Get().Any(x => x.Category.Id == id)) return;
            {
                var itemHistory = item.CreateMapped<Category, CategoryHistory>();

                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreationTime = DateTime.Now;
                itemHistory.CreatorId = _serviceMain.IdentityUser.Id;
                itemHistory.IsDeleted = true;

                _repositoryCategoryHistory.Add(itemHistory, true);
                _repositoryCategory.Delete(item, true);
            }

        }

    }
}
