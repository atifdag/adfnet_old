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
    public static class ProductInstallation
    {
        private static readonly List<Tuple<string, string, decimal, string, bool>> Items = new List<Tuple<string, string, decimal, string, bool>>
        {
            Tuple.Create("Product1", "Product 1", 1.20m, "Category1", true),
            Tuple.Create("Product2", "Product 2", 10.25m, "Category2", false),
            Tuple.Create("Product3", "Product 3", 21.30m, "Category3", false)
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryCategory = provider.GetService<IRepository<Category>>();
            var listProduct = new List<Product>();

            var counterProduct = 1;
            var itemsCount = Items.Count;

            foreach (var (item1, item2, item3, item4, item5) in Items)
            {
                var itemCategory = repositoryCategory.Get(x => x.Code == item4);
                var itemProduct = new Product
                {
                    Id = GuidHelper.NewGuid(),
                    Code = item1,
                    Name = item2,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterProduct,
                    Version = 1,
                    IsApproved = item5,
                    UnitPrice = item3,
                    Category = itemCategory
                };

                listProduct.Add(itemProduct);

                Console.WriteLine(counterProduct + @"/" + itemsCount + @" Product (" + itemProduct.Code + @")");
                counterProduct++;
            }

            unitOfWork.Context.AddRange(listProduct);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.Product);
            Console.WriteLine(@"");
        }
    }
}