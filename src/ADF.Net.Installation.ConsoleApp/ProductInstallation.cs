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
    public static class ProductInstallation
    {
        private static readonly List<Tuple<string, string, decimal, string>> Items = new List<Tuple<string, string, decimal, string>>
        {
            Tuple.Create("Product1", "Product 1", 1.20m, "Category1"),
            Tuple.Create("Product2", "Product 2", 10.25m, "Category2"),
            Tuple.Create("Product3", "Product 3", 21.30m, "Category3")
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryCategory = provider.GetService<IRepository<Category>>();
            var listProduct = new List<Product>();

            var counterProduct = 1;
            var itemsCount = Items.Count;

            foreach (var (item1, item2, item3, item4) in Items)
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
                    IsApproved = true,
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