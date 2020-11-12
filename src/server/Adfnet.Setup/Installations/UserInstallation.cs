using System;
using System.Collections.Generic;
using System.IO;
using Adfnet.Core;
using Adfnet.Core.Constants;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Data.DataEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Adfnet.Setup.Installations
{
    public static class UserInstallation
    {
        public static List<Tuple<string, string, string, string>> Items = new List<Tuple<string, string, string, string>>
        {
            Tuple.Create("Yönetici","Kullanıcı","yonetici.kullanici", RoleConstants.Manager.Item1),
            Tuple.Create("Editör","Kullanıcı","editor.kullanici", RoleConstants.Editor.Item1),
            Tuple.Create("Abone","Kullanıcı","abone.kullanici", RoleConstants.Subscriber.Item1),
            Tuple.Create("Varsayılan","Kullanıcı","varsayilan.kullanici", RoleConstants.Default.Item1),
        };

        public static void Install(IServiceProvider provider)
        {
            var repositoryPerson = provider.GetService<IRepository<Person>>();
            var repositoryUser = provider.GetService<IRepository<User>>();

            var setupProjectRootPath = AppContext.BaseDirectory;
            if (AppContext.BaseDirectory.Contains("bin"))
            {
                setupProjectRootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
            }
            var setupProjectUserFiles = Path.Combine(setupProjectRootPath, "UserFiles");

            foreach (var directory in Directory.GetDirectories(setupProjectUserFiles))
            {
                if (directory.Contains("Public") || directory.Contains("DeletedUsers")) continue;
                var directoryInfo = new DirectoryInfo(directory);
                foreach (var file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (var dir in directoryInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
                Directory.Delete(directory);
            }

            var apiProjectUserFiles = setupProjectUserFiles.Replace("Adfnet.Setup", setupProjectUserFiles.Contains("\\") ? "Adfnet.Web.Api\\wwwroot" : "Adfnet.Web.Api/wwwroot");
           
            foreach (var directory in Directory.GetDirectories(apiProjectUserFiles))
            {
                if (directory.Contains("Public") || directory.Contains("DeletedUsers")) continue;
                var directoryInfo = new DirectoryInfo(directory);
                foreach (var file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (var dir in directoryInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
                Directory.Delete(directory);
            }

            var setupProjectPublicUserFiles = Path.Combine(setupProjectUserFiles, "Public");

            var firstPerson = new Person
            {
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                DisplayOrder = 1,
                Version = 1,
                IsApproved = true,
                IdentityCode = "12345678901",
                FirstName = "Atıf",
                LastName = "DAĞ",
            };


            var firstPassword = firstPerson.FirstName.ToStringForSeo().Substring(0, 1) + firstPerson.LastName.ToStringForSeo().Substring(0, 1) + ".123456";


            var firstLanguage = new Language
            {
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                DisplayOrder = 1,
                Version = 1,
                IsApproved = true,
                Code = LanguageInstallation.DefaultLanguage.Item1,
                Name = LanguageInstallation.DefaultLanguage.Item2
            };

            var firstUser = new User
            {
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                DisplayOrder = 1,
                Version = 1,
                IsApproved = true,
                Username = "atif.dag",
                Password = firstPassword.ToSha512(),
                Email = "atif.dag@adfnet.com",
            };

            firstUser.Creator = firstUser;
            firstUser.LastModifier = firstUser;
          

            firstPerson.CreatorId = firstUser.Id;
            firstPerson.LastModifierId = firstUser.Id;

            firstLanguage.CreatorId = firstUser.Id;
            firstLanguage.LastModifierId = firstUser.Id;


            firstUser.Person = firstPerson;
            firstUser.Language = firstLanguage;


            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            unitOfWork.Context.Add(firstPerson);
            unitOfWork.Context.Add(firstLanguage);
            unitOfWork.Context.Add(firstUser);
            unitOfWork.Context.SaveChanges();

            var firstUserDirectoryPath = Path.Combine(setupProjectUserFiles, firstUser.Id.ToString());
            if (!Directory.Exists(firstUserDirectoryPath))
            {
                Directory.CreateDirectory(firstUserDirectoryPath);
                FileHelper.CopyDirectory(setupProjectPublicUserFiles, firstUserDirectoryPath);
            }

            var itemCounter = 2;
            var totalItemsCount = Items.Count+1;

            foreach (var (item1, item2, item3, item4) in Items)
            {

                var itemPerson = new Person
                {
                    Id = GuidHelper.NewGuid(),
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = itemCounter,
                    Version = 1,
                    IsApproved = true,
                    IdentityCode = "1234567890" + itemCounter,
                    FirstName = item1,
                    LastName = item2,
                    CreatorId = firstUser.Id,
                    LastModifierId = firstUser.Id
                };

                var password = itemPerson.FirstName.ToStringForSeo().Substring(0, 1) + itemPerson.LastName.ToStringForSeo().Substring(0, 1) + ".123456";
                var affectedItemPerson = repositoryPerson.Add(itemPerson, true);


                var itemUser = new User
                {
                    Id = GuidHelper.NewGuid(),
                    Username = item3,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    Creator = firstUser,
                    LastModifier = firstUser,
                    DisplayOrder = itemCounter,
                    Email = item3 + "@adfnet.com",
                    Person = affectedItemPerson,
                    IsApproved = true,
                    Language = firstLanguage,
                    Password = password.ToSha512(),
                    Version = 1,
                };
                var affectedUser = repositoryUser.Add(itemUser, true);

                var userDirectoryPath = Path.Combine(setupProjectUserFiles, affectedUser.Id.ToString());
                if (!Directory.Exists(userDirectoryPath))
                {
                    Directory.CreateDirectory(userDirectoryPath);
                    FileHelper.CopyDirectory(setupProjectPublicUserFiles, userDirectoryPath);
                }

                Console.WriteLine(itemCounter + @"/" + totalItemsCount + @" User (" + affectedUser.Username + @")");
                itemCounter++;

            }

            FileHelper.CopyDirectory(setupProjectUserFiles, apiProjectUserFiles);

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.User);
            Console.WriteLine(@"");
        }
    }
}
