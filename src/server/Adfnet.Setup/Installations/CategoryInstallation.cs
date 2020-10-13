using System;
using System.Collections.Generic;
using System.Linq;
using Adfnet.Core;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Data.DataEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Adfnet.Setup.Installations
{
    public static class CategoryInstallation
    {
        private static readonly List<Tuple<string, string, bool>> Items = new List<Tuple<string, string, bool>>
        {
            Tuple.Create("Category1", "Category 1", true),
            Tuple.Create("Category2", "Category 2", false),
            Tuple.Create("Category3", "Category 3", true)
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var repositoryLanguage = provider.GetService<IRepository<Language>>();
            var languages = repositoryLanguage.Get().Where(x => x.IsApproved).ToList();
            var developerUser = repositoryUser.Get(x => x.Username == "atif.dag");
            var listCategory = new List<Category>();
            var listCategoryLanguageLine = new List<CategoryLanguageLine>();

            var itemCounter = 1;
            var totalItemsCount = Items.Count;

            foreach (var (item1, item2, item3) in Items)
            {
                var itemCategory = new Category
                {
                    Id = GuidHelper.NewGuid(),
                    Code = item1,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    Creator = developerUser,
                    LastModifier = developerUser

                };

                listCategory.Add(itemCategory);

                var lineCounter = 1;
                var totalLanguagesCount = languages.Count;
                foreach (var line in languages.Select(language => new CategoryLanguageLine
                {
                    Id = GuidHelper.NewGuid(),
                    Code = itemCategory.Code + "-" + language.Code,
                    Name = item2 + "-" + language.Code,
                    DisplayOrder = itemCounter,
                    Language = language,
                    Category = itemCategory,
                    CreationTime = DateTime.Now,
                    Creator = developerUser,
                    LastModificationTime = DateTime.Now,
                    LastModifier = developerUser,
                    Version = 1,
                    IsApproved = true
                }))
                {
                    listCategoryLanguageLine.Add(line);
                    Console.WriteLine(lineCounter + @"/" + totalLanguagesCount + @" CategoryLanguageLine (" + line.Code + @") (" + line.Code + @")");
                    lineCounter++;
                }

                Console.WriteLine(itemCounter + @"/" + totalItemsCount + @" Category (" + itemCategory.Code + @")");
                itemCounter++;
            }

            unitOfWork.Context.AddRange(listCategory);
            unitOfWork.Context.AddRange(listCategoryLanguageLine);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.Category);
            Console.WriteLine(@"");
        }
    }
}
