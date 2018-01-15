using System.Threading.Tasks;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Title.Model;
using OrchardCore.Title.ViewModels;

namespace OrchardCore.Title.Drivers
{
    public class TitlePartDisplay : ContentPartDisplayDriver<TitlePart>
    {
        public override IDisplayResult Display(TitlePart titlePart)
        {
            return Shape<TitlePartViewModel>("TitlePart", model =>
            {
                model.Title = titlePart.Title;
                model.TitlePart = titlePart;
            })
            .Location("Detail", "Header:5")
            .Location("Summary", "Header:5");
        }

        public override IDisplayResult Edit(TitlePart titlePart)
        {
            return Shape<TitlePartViewModel>("TitlePart_Edit", model =>
            {
                model.Title = titlePart.Title;
                model.TitlePart = titlePart;

                return Task.CompletedTask;
            });
        }

        public override async Task<IDisplayResult> UpdateAsync(TitlePart model, IUpdateModel updater)
        {
            await updater.TryUpdateModelAsync(model, Prefix, t => t.Title);

            return Edit(model);
        }
    }
}
