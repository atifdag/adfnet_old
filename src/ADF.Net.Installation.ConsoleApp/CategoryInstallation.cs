using System;
using System.Collections.Generic;
using ADF.Net.Core.Globalization;
using ADF.Net.Core.Helpers;
using ADF.Net.Data;
using ADF.Net.Data.DataAccess.EF;
using ADF.Net.Data.DataEntities;
using Microsoft.Extensions.DependencyInjection;

namespace ADF.Net.Installation.ConsoleApp
{
    public static class CategoryInstallation
    {
        private static readonly List<Tuple<string, string>> Items = new List<Tuple<string, string>>
        {
            Tuple.Create("Category1", "Category 1"),
            Tuple.Create("Category2", "Category 2"),
            Tuple.Create("Category3", "Category 3")
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();

            var listCategory = new List<Category>();
           
            var counterCategory = 1;
            var itemsCount = Items.Count;

            foreach (var (item1, item2) in Items)
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
                    IsApproved = true
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
