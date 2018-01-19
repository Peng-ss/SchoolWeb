using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NewsManage.Models;
using NewsManage.ViewModels;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;

namespace NewsManage.Drivers
{
    public  class TypeNewClassifyPartDriver : ContentPartDisplayDriver<TypeNewClassifyPart>
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISiteService _siteService;

        public TypeNewClassifyPartDriver(
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            IServiceProvider serviceProvider,
            ISiteService siteService
            )
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _serviceProvider = serviceProvider;
            _siteService = siteService;
        }


        public override IDisplayResult Edit(TypeNewClassifyPart part)
        {
            return Shape<TypeNewClassifyPartVM>("TypeNewClassifyPart_Edit", m =>
            {
                m.NewClassifyType = part.NewClassifyType;
            });
        }

        public override async Task<IDisplayResult> UpdateAsync(TypeNewClassifyPart model, IUpdateModel updater)
        {
            await updater.TryUpdateModelAsync(model,Prefix, t=>t.NewClassifyType);

            return Edit(model);
        }
    }
}
