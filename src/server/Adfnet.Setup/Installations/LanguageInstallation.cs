using System;
using System.Collections.Generic;
using Adfnet.Core;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Data.DataEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Adfnet.Setup.Installations
{
    public static class LanguageInstallation
    {
        public static readonly Tuple<string, string> DefaultLanguage = new Tuple<string, string>("tr", "Türkçe");

        private static readonly List<Tuple<string, string, bool>> OtherItems = new List<Tuple<string, string, bool>>
        {
            Tuple.Create("en", "English", true),
            Tuple.Create("ar", "العربية", true)
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var developerUser = repositoryUser.Get(x => x.Username == "atif.dag");
            var listLanguage = new List<Language>();

            var itemCounter = 2;
            var totalItemsCount = OtherItems.Count+1;

            foreach (var (item1, item2, item3) in OtherItems)
            {
                var itemLanguage = new Language
                {
                    Id = GuidHelper.NewGuid(),
                    Code = item1,
                    Name = item2,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    CreatorId = developerUser.Id,
                    LastModifierId = developerUser.Id,
                    DisplayOrder = itemCounter,
                    IsApproved = item3,
                    Version=1


                };

                listLanguage.Add(itemLanguage);

                Console.WriteLine(itemCounter + @"/" + totalItemsCount + @" Language (" + itemLanguage.Code + @")");
                itemCounter++;
            }

            unitOfWork.Context.AddRange(listLanguage);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.Language);
            Console.WriteLine(@"");
        }
    }
}
