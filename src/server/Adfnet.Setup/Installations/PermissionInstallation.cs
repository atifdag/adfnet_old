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
    public static class PermissionInstallation
    {

        private static readonly List<Permission> DefaultUserPermissions = new List<Permission>
        {
            new Permission {ControllerName = "Home", ActionName = "Index"},
            new Permission {ControllerName = "Authentication", ActionName = "MyProfile"},
            new Permission {ControllerName = "Authentication", ActionName = "UpdateMyInformation"},
            new Permission {ControllerName = "Authentication", ActionName = "UpdateMyPassword"}
        };

        private static readonly List<string> DeveloperControllers = new List<string>
        {
            "User",
            "Role",
            "ParameterGroup",
            "Parameter",
            "Permission",
            "Menu",
            "Language",
            "Category",
        };

        private static readonly List<string> Actions = new List<string>
        {
            "List",
            "Filter",
            "Detail",
            "Add",
            "Update",
            "Delete",
        };

        private static readonly List<RolePermissionLine> OtherPermissions = new List<RolePermissionLine>
            {
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Add"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Delete"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Add"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Manager.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Delete"}},

                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Add"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "User", ActionName = "Delete"}},

                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Add"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = RoleConstants.Editor.Item1}, Permission = new Permission {ControllerName = "Category", ActionName = "Delete"}},

            };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var repositoryRole = provider.GetService<IRepository<Role>>();
            var developerUser = repositoryUser.Get(x => x.Username == "atif.dag");
            var developerRole = repositoryRole.Get(x => x.Code == RoleConstants.Developer.Item1);
            var defaultRole = repositoryRole.Get(x => x.Code == RoleConstants.Default.Item1);

            var listPermission = new List<Permission>();
            var listRolePermissionLine = new List<RolePermissionLine>();

            var countDefaultUserPermissions = DefaultUserPermissions.Count;
            var counterDefaultUserPermissions = 1;

            foreach (var item in DefaultUserPermissions)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = counterDefaultUserPermissions;
                item.Version = 1;
                item.IsApproved = true;
                item.Creator = developerUser;
                item.LastModifier = developerUser;
                item.Code = item.ControllerName + item.ActionName;
                item.Name = item.ControllerName + " " + item.ActionName;
                listPermission.Add(item);

                var line = new RolePermissionLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = item,
                    Role = defaultRole,
                    Creator = developerUser,
                    CreationTime = DateTime.Now,
                    LastModifier = developerUser,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterDefaultUserPermissions,
                    Version = 1

                };

                listRolePermissionLine.Add(line);
                Console.WriteLine(counterDefaultUserPermissions + @"/" + countDefaultUserPermissions + @" RolePermissionLine (" + defaultRole.Code + @" - " + item.Code + @")");
                counterDefaultUserPermissions++;

            }

            var developerPermissions = new List<Permission>();
            var counterDeveloperControllers = 1;
            foreach (var controller in DeveloperControllers)
            {
                foreach (var action in Actions)
                {
                    developerPermissions.Add(new Permission
                    {
                        Id = GuidHelper.NewGuid(),
                        CreationTime = DateTime.Now,
                        LastModificationTime = DateTime.Now,
                        DisplayOrder = counterDeveloperControllers,
                        Version = 1,
                        IsApproved = true,
                        Creator = developerUser,
                        LastModifier = developerUser,
                        Code = controller + action,
                        Name = controller + " " + action,
                        ControllerName = controller,
                        ActionName = action,
                        Description = string.Empty
                    });
                    counterDeveloperControllers++;
                }
            }

            var totalCountDeveloperPermissions = developerPermissions.Count;
            var counterDeveloperPermissions = 1;

            foreach (var item in developerPermissions)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = counterDeveloperPermissions;
                item.Version = 1;
                item.IsApproved = true;
                item.Creator = developerUser;
                item.LastModifier = developerUser;
                item.Code = item.ControllerName + item.ActionName;
                item.Name = item.ControllerName + " " + item.ActionName;

                listPermission.Add(item);

                var line = new RolePermissionLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = item,
                    Role = developerRole,
                    Creator = developerUser,
                    CreationTime = DateTime.Now,
                    LastModifier = developerUser,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterDeveloperPermissions,
                    Version = 1

                };

                listRolePermissionLine.Add(line);

                Console.WriteLine(counterDeveloperPermissions + @"/" + totalCountDeveloperPermissions + @" RolePermissionLine (" + developerRole.Code + @" - " + item.Code + @")");

                counterDeveloperPermissions++;
            }


            var totalOtherPermissions = OtherPermissions.Count;
            var counterOtherPermissions = 1;

            foreach (var line in OtherPermissions)
            {
                var itemRole = repositoryRole.Get(x => x.Code == line.Role.Code);

                var itemPermission = listPermission.FirstOrDefault(x => x.Code == line.Permission.ControllerName + line.Permission.ActionName);

                if (itemPermission == null)
                {
                    var newPermission = new Permission
                    {
                        Id = GuidHelper.NewGuid(),
                        Code = line.Permission.ControllerName + line.Permission.ActionName,
                        Name = line.Permission.ControllerName + " " + line.Permission.ActionName,
                        ControllerName = line.Permission.ControllerName,
                        ActionName = line.Permission.ActionName,
                        Creator = developerUser,
                        IsApproved = true,
                        LastModifier = developerUser,
                        DisplayOrder = counterOtherPermissions,
                        Description = "",
                        CreationTime = DateTime.Now,
                        LastModificationTime = DateTime.Now,
                        Version = 1
                    };

                    listPermission.Add(newPermission);
                    itemPermission = newPermission;
                }

                var affectedRolePermissionLine = new RolePermissionLine
                {
                    Id = GuidHelper.NewGuid(),
                    Role = itemRole,
                    Permission = itemPermission,
                    Creator = developerUser,
                    LastModifier = developerUser,
                    DisplayOrder = counterOtherPermissions,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    Version = 1
                };



                listRolePermissionLine.Add(affectedRolePermissionLine);

                Console.WriteLine(counterOtherPermissions + @"/" + totalOtherPermissions + @" RolePermissionLine (" + itemRole.Code + @" - " + itemPermission.Code + @")");

                counterOtherPermissions++;
            }

            unitOfWork.Context.AddRange(listPermission);
            unitOfWork.Context.AddRange(listRolePermissionLine);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.Permission);
            Console.WriteLine(@"");
        }
    }
}
