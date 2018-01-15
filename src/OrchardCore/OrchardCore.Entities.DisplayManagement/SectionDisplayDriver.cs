using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;

namespace OrchardCore.Entities.DisplayManagement
{
    /// <summary>
    /// A concrete implementation of this class will be able to take part in the rendering of an <see cref="IEntity"/>
    /// shape instance for a specific section of the object. A section represents a property of an entity instance
    /// where the name of the property is the type of the section.
    /// </summary>
    /// <typeparam name="TSection">The type of the section this driver handles.</typeparam>
    public abstract class SectionDisplayDriver<TModel, TSection> : DisplayDriver<TModel>
        where TSection : new()
        where TModel : class, IEntity
    {
        public override Task<IDisplayResult> DisplayAsync(TModel model, BuildDisplayContext context)
        {
            JToken property;
            TSection section;

            var typeName = typeof(TSection).Name;

            if (!model.Properties.TryGetValue(typeName, out property))
            {
                section = new TSection();
            }
            else
            {
                section = property.ToObject<TSection>();
            }

            return DisplayAsync(section, context);
        }

        public override Task<IDisplayResult> EditAsync(TModel model, BuildEditorContext context)
        {
            JToken property;
            TSection section;

            var typeName = typeof(TSection).Name;

            if (!model.Properties.TryGetValue(typeName, out property))
            {
                section = new TSection();
            }
            else
            {
                section = property.ToObject<TSection>();
            }            

            return EditAsync(section, context);
        }

        public override Task<IDisplayResult> UpdateAsync(TModel model, UpdateEditorContext context)
        {
            JToken property;
            TSection section;

            var typeName = typeof(TSection).Name;

            if (!model.Properties.TryGetValue(typeName, out property))
            {
                section = new TSection();
            }
            else
            {
                section = property.ToObject<TSection>();
            }
            
            var result = UpdateAsync(section, context.Updater, context);

            if (result == null)
            {
                return Task.FromResult<IDisplayResult>(null);
            }

            if (context.Updater.ModelState.IsValid)
            {
                model.Properties[typeName] = JObject.FromObject(section);
            }

            return result;
        }

        public virtual Task<IDisplayResult> DisplayAsync(TSection section, BuildDisplayContext context)
        {
            return Task.FromResult(Display(section, context));
        }

        public virtual IDisplayResult Display(TSection section, BuildDisplayContext context)
        {
            return Display(section);
        }

        public virtual IDisplayResult Display(TSection section)
        {
            return null;
        }

        public virtual Task<IDisplayResult> EditAsync(TSection section, BuildEditorContext context)
        {
            return Task.FromResult(Edit(section, context));
        }

        public virtual IDisplayResult Edit(TSection section, BuildEditorContext context)
        {
            return Edit(section);
        }

        public virtual IDisplayResult Edit(TSection section)
        {
            return null;
        }

        public virtual Task<IDisplayResult> UpdateAsync(TSection section, IUpdateModel updater, BuildEditorContext context)
        {
            return UpdateAsync(section, context);
        }

        public virtual Task<IDisplayResult> UpdateAsync(TSection section, BuildEditorContext context)
        {
            return UpdateAsync(section, context.Updater, context.GroupId);
        }

        public virtual Task<IDisplayResult> UpdateAsync(TSection section, IUpdateModel updater, string groupId)
        {
            return Task.FromResult<IDisplayResult>(null);
        }
    }
}
