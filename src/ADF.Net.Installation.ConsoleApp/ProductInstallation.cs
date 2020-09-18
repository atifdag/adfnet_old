using System;
using System.Collections.Generic;
using ADF.Net.Core.Helpers;
using ADF.Net.Data;
using ADF.Net.Data.DataAccess.EF;
using ADF.Net.Data.DataEntities;
using Microsoft.Extensions.DependencyInjection;

namespace ADF.Net.Installation.ConsoleApp
{
    public static class ProductInstallation
    {
        private static readonly List<Product> Items = new List<Product>
        {
            new Product {Code = "tr", Name = "Türkçe"},
            new Product {Code = "ar", Name = "العربية"},
            new Product {Code = "en", Name = "English"}
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();

            var listProduct = new List<Product>();
           
            var counterProduct = 1;
            var itemsCount = Items.Count;

            foreach (var item in Items)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = counterProduct;
                item.Version = 1;
                item.IsApproved = true;
                listProduct.Add(item);

                Console.WriteLine(counterProduct + @"/" + itemsCount + @" Product (" + item.Code + @")");
                counterProduct++;
            }

            unitOfWork.Context.AddRange(listProduct);
            unitOfWork.Context.SaveChanges();
        }
    }
}
