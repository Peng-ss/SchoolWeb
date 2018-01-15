using System;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.Liquid;

namespace OrchardCore.Contents.Filters
{
    public class BuildDisplayFilter : ILiquidFilter
    {
        public async Task<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var obj = input.ToObjectValue();

            if (!(obj is ContentItem contentItem))
            {
                contentItem = null;

                if (obj is JObject jObject)
                {
                    contentItem = jObject.ToObject<ContentItem>();
                }
            }

            // If input is a 'JObject' but which not represents a 'ContentItem',
            // a 'ContentItem' is still created but with some null properties.
            if (contentItem?.ContentItemId == null)
            {
                return NilValue.Instance;
            }

            if (!ctx.AmbientValues.TryGetValue("Services", out var services))
            {
                throw new ArgumentException("Services missing while invoking 'build_display'");
            }

            var displayType = arguments["type"].Or(arguments.At(0)).ToStringValue();
            var displayManager = ((IServiceProvider)services).GetRequiredService<IContentItemDisplayManager>();

            return FluidValue.Create(await displayManager.BuildDisplayAsync(contentItem, null, displayType));
        }
    }
}
