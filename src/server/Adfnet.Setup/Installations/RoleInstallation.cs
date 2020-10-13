using System;
using System.Collections.Generic;
using System.Linq;
using Adfnet.Core;
using Adfnet.Core.Constants;
using Adfnet.Core.Globalization;
using Adfnet.Core.Helpers;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Data.DataEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Adfnet.Setup.Installations
{
    public static class RoleInstallation
    {
        private static readonly List<Tuple<string, string, int>> Items = new List<Tuple<string, string, int>>
        {
            Tuple.Create(RoleConstants.Developer.Item1, RoleConstants.Developer.Item2, RoleConstants.Developer.Item3),
            Tuple.Create(RoleConstants.Manager.Item1, RoleConstants.Manager.Item2, RoleConstants.Manager.Item3),
            Tuple.Create(RoleConstants.Editor.Item1, RoleConstants.Editor.Item2, RoleConstants.Editor.Item3),
            Tuple.Create(RoleConstants.Subscriber.Item1, RoleConstants.Subscriber.Item2, RoleConstants.Subscriber.Item3),
            Tuple.Create(RoleConstants.Default.Item1, RoleConstants.Default.Item2, RoleConstants.Default.Item3)

        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var developerUser = repositoryUser.Get(x => x.Username == "atif.dag");
            var listRole = new List<Role>();
            var listRoleUserLine = new List<RoleUserLine>();

            var itemCounter = 1;
            var totalItemsCount = Items.Count;

            foreach (var (item1, item2, item3) in Items)
            {
                var itemRole = new Role
                {
                    Id = GuidHelper.NewGuid(),
                    Code = item1,
                    Name = item2,
                    Level = item3,
                    DisplayOrder = itemCounter,
                    IsApproved = true,
                    Version = 1,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    Creator = developerUser,
                    LastModifier = developerUser,
                   
                };

                listRole.Add(itemRole);

                Console.WriteLine(itemCounter + @"/" + totalItemsCount + @" Role (" + itemRole.Code + @")");
                itemCounter++;
            }

            var counterUserRoleList = 1;
            var userRoleListCount = UserInstallation.Items.Count;

            var firstLine = new RoleUserLine
            {
                Id = GuidHelper.NewGuid(),
                User = developerUser,
                Role = listRole.FirstOrDefault(x => x.Code == RoleConstants.Developer.Item1),
                Creator = developerUser,
                CreationTime = DateTime.Now,
                LastModifier = developerUser,
                LastModificationTime = DateTime.Now,
                DisplayOrder = counterUserRoleList,
                Version = 1

            };

            listRoleUserLine.Add(firstLine);

            foreach (var (item1, item2, item3, item4) in UserInstallation.Items)
            {
                var user = repositoryUser.Get(x => x.Username == item3);
                var role = listRole.FirstOrDefault(x => x.Code == item4);

                var line = new RoleUserLine
                {
                    Id = GuidHelper.NewGuid(),
                    User = user,
                    Role = role,
                    Creator = developerUser,
                    CreationTime = DateTime.Now,
                    LastModifier = developerUser,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterUserRoleList,
                    Version = 1

                };

                listRoleUserLine.Add(line);

                Console.WriteLine(counterUserRoleList + @"/" + userRoleListCount + @" RoleUserLine (" + user.Username + @" - " + role.Code + @")");

                counterUserRoleList++;

            }

            unitOfWork.Context.AddRange(listRole);
            unitOfWork.Context.AddRange(listRoleUserLine);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.Role);
            Console.WriteLine(@"");
        }
    }
}
