using System.Threading.Tasks;
using OrchardCore.Demo.Models;
using OrchardCore.Demo.ViewModels;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Entities.DisplayManagement;
using OrchardCore.Users.Models;

namespace OrchardCore.Demo.Drivers
{
    public class UserProfileDisplayDriver : SectionDisplayDriver<User, UserProfile>
    {
        public override IDisplayResult Edit(UserProfile profile, BuildEditorContext context)
        {
            return Shape<EditUserProfileViewModel>("UserProfile_Edit", model =>
            {
                model.Age = profile.Age;
                model.Name = profile.Name;
            }).Location("Content:2");
        }

        public override async Task<IDisplayResult> UpdateAsync(UserProfile profile, IUpdateModel updater, BuildEditorContext context)
        {
            var model = new EditUserProfileViewModel();

            if (await context.Updater.TryUpdateModelAsync(model, Prefix))
            {
                profile.Age = model.Age;
                profile.Name = model.Name;
            }

            return Edit(profile);
        }
    }
}
