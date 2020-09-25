using System;
using System.Collections.Generic;
using ADF.Net.Core;
using ADF.Net.Core.Globalization;
using ADF.Net.Core.Helpers;
using ADF.Net.Data.DataAccess.EntityFramework;
using ADF.Net.Data.DataEntities;
using Microsoft.Extensions.DependencyInjection;

namespace ADF.Net.Installation.ConsoleApp
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

            var listCategory = new List<Category>();
           
            var counterCategory = 1;
            var itemsCount = Items.Count;

            foreach (var (item1, item2, item3) in Items)
            {
                var itemCategory = new Category
                {
                    Id = GuidHelper.NewGuid(),
                    Code = item1,
                    Name = item2,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterCategory,
                    Version = 1,
                    IsApproved = item3
                };

                listCategory.Add(itemCategory);

                Console.WriteLine(counterCategory + @"/" + itemsCount + @" Category (" + itemCategory.Code + @")");
                counterCategory++;
            }

            unitOfWork.Context.AddRange(listCategory);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.Category);
            Console.WriteLine(@"");
        }
    }
}
