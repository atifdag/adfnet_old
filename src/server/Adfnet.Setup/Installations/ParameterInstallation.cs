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
    public static class ParameterInstallation
    {
        private static readonly List<KeyValuePair<string, string>> ParameterGroups = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("MailParameters", "Eposta Parametreleri"),
            new KeyValuePair<string, string>("ApplicationParameters", "Uygulama Parametreleri"),
        };

        private static readonly List<Tuple<string, string, string>> Parameters = new List<Tuple<string, string, string>>
        {
            new Tuple<string, string, string>("ApplicationParameters","DefaultLanguage","tr-TR"),
            new Tuple<string, string, string>("ApplicationParameters","ApplicationName","Adfnet"),
            new Tuple<string, string, string>("ApplicationParameters","ApplicationUrl","http://www.adfnet.com"),
            new Tuple<string, string, string>("ApplicationParameters","CorporationName","Adfnet"),
            new Tuple<string, string, string>("ApplicationParameters","TaxAdministration","Çankaya V.D."),
            new Tuple<string, string, string>("ApplicationParameters","TaxNumber","1234 5678 901"),
            new Tuple<string, string, string>("ApplicationParameters","Address","Ankara"),
            new Tuple<string, string, string>("ApplicationParameters","Phone","(312) 000 00 00"),
            new Tuple<string, string, string>("ApplicationParameters","Fax","(312) 000 00 00"),
            new Tuple<string, string, string>("ApplicationParameters","SendMailAfterUpdateUserInformation","false"),
            new Tuple<string, string, string>("ApplicationParameters","SendMailAfterUpdateUserPassword","false"),
            new Tuple<string, string, string>("ApplicationParameters","SendMailAfterAddUser","false"),
            new Tuple<string, string, string>("ApplicationParameters","SessionTimeOut","20"),
            new Tuple<string, string, string>("ApplicationParameters","PageSizeList","5,10,25,50,100,500"),
            new Tuple<string, string, string>("ApplicationParameters","DefaultPageSize","10"),
            new Tuple<string, string, string>("ApplicationParameters","EmailTemplatePath","wwwroot/EmailTemplates"),
            new Tuple<string, string, string>("ApplicationParameters","MemoryCacheMainKey","AdfnetWebApiCacheMainKey"),
            new Tuple<string, string, string>("MailParameters","SmtpServer","smtp.office365.com"),
            new Tuple<string, string, string>("MailParameters","SmtpPort","587"),
            new Tuple<string, string, string>("MailParameters","SmtpSsl","true"),
            new Tuple<string, string, string>("MailParameters","SmtpUser","web@adfnet.com"),
            new Tuple<string, string, string>("MailParameters","SmtpPassword",""),
            new Tuple<string, string, string>("MailParameters","SmtpSenderName","Adfnet Identity Manager"),
            new Tuple<string, string, string>("MailParameters","SmtpSenderMail","web@adfnet.com"),
            new Tuple<string, string, string>("MailParameters","UseDefaultCredentials","false"),
            new Tuple<string, string, string>("MailParameters","UseDefaultNetworkCredentials","false"),
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var developerUser = repositoryUser.Get(x => x.Username == "atif.dag");

            var listParameterGroup = new List<ParameterGroup>();
            var listParameter = new List<Parameter>();

            var counterParameterGroup = 1;

            foreach (var (key, value) in ParameterGroups)
            {

                var item = new ParameterGroup
                {
                    Id = GuidHelper.NewGuid(),
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterParameterGroup,
                    Version = 1,
                    IsApproved = true,
                    Code = key,
                    Name = value,
                    Description = value,
                    Creator = developerUser,
                    LastModifier = developerUser,
                };

                counterParameterGroup++;
                listParameterGroup.Add(item);

            }

            unitOfWork.Context.AddRange(listParameterGroup);
            unitOfWork.Context.SaveChanges();

            var counterParameter = 1;
            var listParameterCount = Parameters.Count;

            foreach (var (item1, item2, item3) in Parameters)
            {
                var itemParameterGroup = unitOfWork.Context.Set<ParameterGroup>().FirstOrDefault(x => x.Code == item1);

                var item = new Parameter
                {
                    Id = GuidHelper.NewGuid(),
                    Key = item2,
                    Value = item3,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterParameter,
                    Version = 1,
                    IsApproved = true,
                    Creator = developerUser,
                    LastModifier = developerUser,
                    ParameterGroup = itemParameterGroup
                };

                listParameter.Add(item);

                Console.WriteLine(counterParameter + @"/" + listParameterCount + @" Parameter (" + item.Key + @")");

                counterParameter++;

            }

            unitOfWork.Context.AddRange(listParameter);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.Parameter);
            Console.WriteLine(@"");
        }
    }
}

