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
    public static class MenuInstallation
    {
        private static readonly List<Menu> DeveloperRootMenus = new List<Menu>
        {
            new Menu {Address = "/User", Code = "UserManagement", Name =Dictionary.UserManagement,  Icon = "account_circle"},
            new Menu {Address = "/Role", Code = "RoleManagement", Name =Dictionary.RoleManagement,  Icon = "recent_actors"},
            new Menu {Address = "/ParameterGroup", Code = "ParameterGroupManagement", Name =Dictionary.ParameterGroupManagement,  Icon = "scatter_plot"},
            new Menu {Address = "/Parameter", Code = "ParameterManagement", Name =Dictionary.ParameterManagement,  Icon = "lens"},
            new Menu {Address = "/Permission", Code = "PermissionManagement", Name =Dictionary.PermissionManagement,  Icon = "lock"},
            new Menu {Address = "/Menu", Code = "MenuManagement", Name =Dictionary.MenuManagement,  Icon = "vertical_split"},
            new Menu {Address = "/Language", Code = "LanguageManagement", Name =Dictionary.LanguageManagement,  Icon = "language"},
            new Menu {Address = "/Category", Code = "CategoryManagement", Name = Dictionary.CategoryManagement,  Icon = "category"},

        };

        private static List<Tuple<string, string, string, string, string>> DeveloperChildMenus => new List<Tuple<string, string, string, string, string>>
            {
                Tuple.Create("/User/Add", "UserAdd", Dictionary.Add, "UserManagement", "pi pi-plus"),
                Tuple.Create("/User/List", "UserList", Dictionary.Add, "UserManagement", "pi pi-list"),

                Tuple.Create("/Role/Add", "RoleAdd", Dictionary.Add, "RoleManagement", "pi pi-plus"),
                Tuple.Create("/Role/List", "RoleList", Dictionary.Add, "RoleManagement", "pi pi-list"),

                Tuple.Create("/ParameterGroup/Add", "ParameterGroupAdd", Dictionary.Add, "ParameterGroupManagement", "pi pi-plus"),
                Tuple.Create("/ParameterGroup/List", "ParameterGroupList", Dictionary.Add, "ParameterGroupManagement", "pi pi-list"),

                Tuple.Create("/Parameter/Add", "ParameterAdd", Dictionary.Add, "ParameterManagement", "pi pi-plus"),
                Tuple.Create("/Parameter/List", "ParameterList", Dictionary.Add, "ParameterManagement", "pi pi-list"),

                Tuple.Create("/Permission/Add", "PermissionAdd", Dictionary.Add, "PermissionManagement", "pi pi-plus"),
                Tuple.Create("/Permission/List", "PermissionList", Dictionary.Add, "PermissionManagement", "pi pi-list"),

                Tuple.Create("/Menu/Add", "MenuAdd", Dictionary.Add, "MenuManagement", "pi pi-plus"),
                Tuple.Create("/Menu/List", "MenuList", Dictionary.Add, "MenuManagement", "pi pi-list"),

                Tuple.Create("/Language/Add", "LanguageAdd", Dictionary.Add, "LanguageManagement", "pi pi-plus"),
                Tuple.Create("/Language/List", "LanguageList", Dictionary.Add, "LanguageManagement", "pi pi-list"),

                Tuple.Create("/Category/Add", "CategoryAdd", Dictionary.Add, "CategoryManagement", "pi pi-plus"),
                Tuple.Create("/Category/List", "CategoryList", Dictionary.Add, "CategoryManagement", "pi pi-list"),

            };


        private static List<Tuple<string, string>> PermissionMenus => new List<Tuple<string, string>>
        {
            Tuple.Create("UserList","UserManagement"),
            Tuple.Create("UserList","UserList"),
            Tuple.Create("UserAdd","UserAdd"),

            Tuple.Create("RoleList","RoleManagement"),
            Tuple.Create("RoleList","RoleList"),
            Tuple.Create("RoleAdd","RoleAdd"),

            Tuple.Create("ParameterGroupList","ParameterGroupManagement"),
            Tuple.Create("ParameterGroupList","ParameterGroupList"),
            Tuple.Create("ParameterGroupAdd","ParameterGroupAdd"),

            Tuple.Create("ParameterList","ParameterManagement"),
            Tuple.Create("ParameterList","ParameterList"),
            Tuple.Create("ParameterAdd","ParameterAdd"),

            Tuple.Create("PermissionList","PermissionManagement"),
            Tuple.Create("PermissionList","PermissionList"),
            Tuple.Create("PermissionAdd","PermissionAdd"),

            Tuple.Create("MenuList","MenuManagement"),
            Tuple.Create("MenuList","MenuList"),
            Tuple.Create("MenuAdd","MenuAdd"),

            Tuple.Create("LanguageList","LanguageManagement"),
            Tuple.Create("LanguageList","LanguageList"),
            Tuple.Create("LanguageAdd","LanguageAdd"),

            Tuple.Create("CategoryList","CategoryManagement"),
            Tuple.Create("CategoryList","CategoryList"),
            Tuple.Create("CategoryAdd","CategoryAdd"),


        };


        private static readonly List<PermissionMenuLine> OtherPermissionMenuLines = new List<PermissionMenuLine>
        {
            // new PermissionMenuLine { Menu = new Menu{ Name = Dictionary.CacheManagement}, Permission =  new Permission {ControllerName = "Cache", ActionName = "List"}},
        };




        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var repositoryPermission = provider.GetService<IRepository<Permission>>();
            var developerUser = repositoryUser.Get(x => x.Username == "atif.dag");
            var listMenu = new List<Menu>();
            var listPermissionMenuLine = new List<PermissionMenuLine>();

            var totalDeveloperRootMenus = DeveloperRootMenus.Count;
            var counterDeveloperRootMenus = 1;

            var rootMenu = new Menu
            {
                Id = GuidHelper.NewGuid(),
                Code = MenuConstants.AdminRootMenuCode,
                Address = "#",
                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                DisplayOrder = 1,
                Version = 1,
                IsApproved = true,
                Creator = developerUser,
                LastModifier = developerUser,
                Name = "-"
            };

            rootMenu.ParentMenu = rootMenu;
            listMenu.Add(rootMenu);
            foreach (var item in DeveloperRootMenus)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = counterDeveloperRootMenus;
                item.Version = 1;
                item.IsApproved = true;
                item.Creator = developerUser;
                item.LastModifier = developerUser;
                item.ParentMenu = rootMenu;
                listMenu.Add(item);

                Console.WriteLine(counterDeveloperRootMenus + @"/" + totalDeveloperRootMenus + @" Root Menu (" + item.Code + @")");
                counterDeveloperRootMenus++;
            }

            var developerChildMenusCount = DeveloperChildMenus.Count;
            var developerChildMenusCounter = 1;


            foreach (var (item1, item2, item3, item4, item5) in DeveloperChildMenus)
            {
                var parentMenu = listMenu.FirstOrDefault(x => x.Code == item4);

                var itemMenu = new Menu
                {
                    Id = GuidHelper.NewGuid(),
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = developerChildMenusCounter,
                    Version = 1,
                    IsApproved = true,
                    Creator = developerUser,
                    LastModifier = developerUser,
                    Address = item1,
                    Code = item2,
                    Name = item3,
                    ParentMenu = parentMenu,
                    Icon = item5
                };

                listMenu.Add(itemMenu);

                Console.WriteLine(developerChildMenusCounter + @"/" + developerChildMenusCount + @" Child Menu (" + itemMenu.Code + @")");
                developerChildMenusCounter++;

            }

            PermissionMenuInstall(developerUser, listMenu, listPermissionMenuLine, repositoryPermission);

           

            var totalOtherPermissionMenuLines = OtherPermissionMenuLines.Count;
            var counterOtherPermissionMenuLines = 1;

            foreach (var otherPermissionMenuLine in OtherPermissionMenuLines)
            {
                var itemPermission = repositoryPermission.Get(x => x.ControllerName == otherPermissionMenuLine.Permission.ControllerName && x.ActionName == otherPermissionMenuLine.Permission.ActionName);

                var affectedMenu = new Menu
                {
                    Id = GuidHelper.NewGuid(),
                    Code = otherPermissionMenuLine.Menu.Code ?? otherPermissionMenuLine.Permission.ControllerName + otherPermissionMenuLine.Permission.ActionName,
                    Name = otherPermissionMenuLine.Menu.Name,
                    Address = otherPermissionMenuLine.Menu.Address ?? "/" + otherPermissionMenuLine.Permission.ControllerName + "/" +
                              otherPermissionMenuLine.Permission.ActionName,
                    Creator = developerUser,
                    IsApproved = true,
                    LastModifier = developerUser,
                    DisplayOrder = otherPermissionMenuLine.Menu.DisplayOrder,
                    CreationTime = DateTime.Now,
                    Version = 1,
                    Description = string.Empty,
                    LastModificationTime = DateTime.Now,
                    Icon = string.Empty,
                    ParentMenu = rootMenu
                };

                listMenu.Add(affectedMenu);

                var addedLine = new PermissionMenuLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = itemPermission,
                    Menu = affectedMenu,
                    CreationTime = DateTime.Now,
                    Creator = developerUser,
                    LastModificationTime = DateTime.Now,
                    LastModifier = developerUser,
                    DisplayOrder = counterOtherPermissionMenuLines,
                    Version = 1
                };

                listPermissionMenuLine.Add(addedLine);

                Console.WriteLine(counterOtherPermissionMenuLines + @"/" + totalOtherPermissionMenuLines + @" PermissionMenuLine (" + affectedMenu.Code + @")");
                counterOtherPermissionMenuLines++;
            }

            unitOfWork.Context.AddRange(listMenu);
            unitOfWork.Context.AddRange(listPermissionMenuLine);
            unitOfWork.Context.SaveChanges();

            Console.WriteLine(Messages.SuccessItemOk, Dictionary.Menu);
            Console.WriteLine(@"");
        }


        private static void PermissionMenuInstall(User user, ICollection<Menu> listMenu, ICollection<PermissionMenuLine> listPermissionMenuLine, IRepository<Permission> repositoryPermission)
        {

            var permissionMenusCount = PermissionMenus.Count;
            var permissionMenusCounter = 1;

            foreach (var (item1, item2) in PermissionMenus)
            {
                var itemPermission = repositoryPermission.Get(x => x.Code == item1);
                var itemMenu = listMenu.FirstOrDefault(x => x.Code == item2);

                var addedLine = new PermissionMenuLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = itemPermission,
                    Menu = itemMenu,
                    CreationTime = DateTime.Now,
                    Creator = user,
                    LastModificationTime = DateTime.Now,
                    LastModifier = user,
                    DisplayOrder = 1,
                    Version = 1
                };

                listPermissionMenuLine.Add(addedLine);

                Console.WriteLine(permissionMenusCounter + @"/" + permissionMenusCount + @" PermissionMenuLine (" + itemPermission.Code + @" - " + itemMenu.Code + @")");
                permissionMenusCounter++;
            }
        }

    }
}
