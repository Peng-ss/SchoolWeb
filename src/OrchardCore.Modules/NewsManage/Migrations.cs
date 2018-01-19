using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace NewsManage
{
    public class Migrations : DataMigration
    {
        IContentDefinitionManager _contentDefinitionManager;

        public Migrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }
        public int Create()
        {
            _contentDefinitionManager.AlterTypeDefinition("新闻组管理", menu => menu
              .Creatable()
              .Draftable()
              .Listable()
              .WithPart("TitlePart", part => part.WithPosition("1"))
              .WithPart("NewPart", part => part.WithPosition("2"))
              .WithPart("AutoroutePart", part => part.WithPosition("3")
                                                       .WithSetting("ShowHomepageOption", "true")
              )
          );
            _contentDefinitionManager.AlterPartDefinition("TypeNewClassifyPart", builder => builder
                            .Attachable()
                            .WithDescription("Provides a way to add TypeNewClassifyPart" + " per content items.")
                        );
            return 1;
        }

        //public int UpdateFrom1()
        //{
        //    return 2;
        //}


    }
}
