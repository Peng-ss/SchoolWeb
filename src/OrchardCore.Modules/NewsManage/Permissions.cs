using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.Security.Permissions;

namespace NewsManage
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission NewManage = new Permission("NewManage", "管理新闻");

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                   Name = "Administrator",
                   Permissions = new []
                   {
                       NewManage
                   }
                },
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                NewManage,
            };
        }
    }
}
