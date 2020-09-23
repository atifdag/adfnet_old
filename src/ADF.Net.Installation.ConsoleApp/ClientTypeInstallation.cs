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
    public static class ClientTypeInstallation
    {
        private static readonly List<Tuple<string, string, bool>> Items = new List<Tuple<string, string, bool>>
        {
            Tuple.Create("ClientType1", "ClientType 1", true),
            Tuple.Create("ClientType2", "ClientType 2", false),
            Tuple.Create("ClientType3", "ClientType 3", true)
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();

            var listClientType = new List<ClientType>();
           
            var counterClientType = 1;
            var itemsCount = Items.Count;

            foreach (var (item1, item2, item3) in Items)
            {
                var itemClientType = new ClientType
                {
                    Id = GuidHelper.NewGuid(),
                    Code = item1,
                    Name = item2,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterClientType,
                    Version = 1,
                    IsApproved = item3
                };

                listClientType.Add(itemClientType);

                Console.WriteLine(counterClientType + @"/" + itemsCount + @" ClientType (" + itemClientType.Code + @")");
                counterClientType++;
            }

            unitOfWork.Context.AddRange(listClientType);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.ClientType);
            Console.WriteLine(@"");
        }
    }
}
