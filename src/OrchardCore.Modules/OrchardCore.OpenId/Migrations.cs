using OrchardCore.Data.Migration;
using OrchardCore.OpenId.Indexes;

namespace OrchardCore.OpenId
{
    public class Migrations : DataMigration
    {
        public int Create()
        {
            SchemaBuilder.CreateMapIndexTable(nameof(OpenIdApplicationIndex), table => table
                .Column<string>(nameof(OpenIdApplicationIndex.ClientId))
                .Column<string>(nameof(OpenIdApplicationIndex.LogoutRedirectUri))
                .Column<string>(nameof(OpenIdApplicationIndex.RedirectUri))
            );

            SchemaBuilder.CreateReduceIndexTable(nameof(OpenIdApplicationByRoleNameIndex), table => table
                .Column<string>(nameof(OpenIdApplicationByRoleNameIndex.RoleName))
                .Column<int>(nameof(OpenIdApplicationByRoleNameIndex.Count))
            );

            SchemaBuilder.CreateMapIndexTable(nameof(OpenIdTokenIndex), table => table
                .Column<int>(nameof(OpenIdTokenIndex.AppId))
                .Column<string>(nameof(OpenIdTokenIndex.Subject))
                .Column<int>(nameof(OpenIdTokenIndex.TokenId)));

            return 1;
        }
    }
}