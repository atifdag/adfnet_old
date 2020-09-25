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
    public static class ClientInstallation
    {
        private static readonly List<Tuple<string, string,  string, bool>> Items = new List<Tuple<string, string, string, bool>>
        {
            Tuple.Create("Client1", "Client 1",  "ClientType1", true),
            Tuple.Create("Client2", "Client 2", "ClientType2", false),
            Tuple.Create("Client3", "Client 3",  "ClientType3", false)
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryClientType = provider.GetService<IRepository<ClientType>>();
            var listClient = new List<Client>();

            var counterClient = 1;
            var itemsCount = Items.Count;

            foreach (var (item1, item2, item3, item4) in Items)
            {
                var itemClientType = repositoryClientType.Get(x => x.Code == item3);
                var itemClient = new Client
                {
                    Id = GuidHelper.NewGuid(),
                    Code = item1,
                    Name = item2,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterClient,
                    Version = 1,
                    IsApproved = item4,
                    
                    ClientType = itemClientType
                };

                listClient.Add(itemClient);

                Console.WriteLine(counterClient + @"/" + itemsCount + @" Client (" + itemClient.Code + @")");
                counterClient++;
            }

            unitOfWork.Context.AddRange(listClient);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.Client);
            Console.WriteLine(@"");
        }
    }
}