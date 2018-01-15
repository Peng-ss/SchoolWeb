using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace OrchardCore.DisplayManagement.Theming
{
    public class ThemeViewStart : RazorPage<dynamic>
    {
        public override Task ExecuteAsync()
        {
            Layout = "~/Views/Shared/_Layout" + RazorViewEngine.ViewExtension;
            return Task.CompletedTask;
        }
    }
}
