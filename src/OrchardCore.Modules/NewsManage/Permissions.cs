using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.Security.Permissions;

namespace NewsManage
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission NewManage = new Permission("NewManage", "管理新闻");
        public static readonly Permission NewEditor = new Permission("NewEditor", "编辑新闻");

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                   Name = "新闻管理者",
                   Permissions = new []
                   {
                       NewManage
                   }
                },
                new PermissionStereotype
                {
                    Name = "新闻编辑者",
                    Permissions = new[]
                    {
                        NewEditor
                    }
                }
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                NewManage,NewEditor
            };
        }
    }
}
