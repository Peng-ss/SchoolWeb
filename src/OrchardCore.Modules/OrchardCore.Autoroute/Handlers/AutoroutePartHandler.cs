using System;
using System.Linq;
using Fluid;
using OrchardCore.Autoroute.Model;
using OrchardCore.Autoroute.Models;
using OrchardCore.Autoroute.Services;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Environment.Cache;
using OrchardCore.Liquid;
using OrchardCore.Settings;
using YesSql;

namespace OrchardCore.Autoroute.Handlers
{
    public class AutoroutePartHandler : ContentPartHandler<AutoroutePart>
    {
        private readonly IAutorouteEntries _entries;
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ISiteService _siteService;
        private readonly ITagCache _tagCache;
        private readonly YesSql.ISession _session;

        public AutoroutePartHandler(
            IAutorouteEntries entries,
            ILiquidTemplateManager liquidTemplateManager,
            IContentDefinitionManager contentDefinitionManager,
            ISiteService siteService,
            ITagCache tagCache,
            YesSql.ISession session)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _entries = entries;
            _liquidTemplateManager = liquidTemplateManager;
            _siteService = siteService;
            _tagCache = tagCache;
            _session = session;
        }

        public override void Published(PublishContentContext context, AutoroutePart part)
        {
            if (!String.IsNullOrWhiteSpace(part.Path))
            {
                _entries.AddEntry(part.ContentItem.ContentItemId, part.Path);
            }

            if (part.SetHomepage)
            {
                var site = _siteService.GetSiteSettingsAsync().GetAwaiter().GetResult();
                var homeRoute = site.HomeRoute;

                homeRoute["area"] = "OrchardCore.Contents";
                homeRoute["controller"] = "Item";
                homeRoute["action"] = "Display";
                homeRoute["contentItemId"] = context.ContentItem.ContentItemId;

                // Once we too the flag into account we can dismiss it.
                part.SetHomepage = false;
                _siteService.UpdateSiteSettingsAsync(site).GetAwaiter().GetResult();
            }

            // Evict any dependent item from cache
            RemoveTag(part);
        }

        public override void Unpublished(PublishContentContext context, AutoroutePart part)
        {
            if (!String.IsNullOrWhiteSpace(part.Path))
            {
                _entries.RemoveEntry(part.ContentItem.ContentItemId, part.Path);

                // Evict any dependent item from cache
                RemoveTag(part);
            }
        }

        public override void Removed(RemoveContentContext context, AutoroutePart part)
        {
            if (!String.IsNullOrWhiteSpace(part.Path))
            {
                _entries.RemoveEntry(part.ContentItem.ContentItemId, part.Path);

                // Evict any dependent item from cache
                RemoveTag(part);
            }
        }

        public override void Updated(UpdateContentContext context, AutoroutePart part)
        {
            // Compute the Path only if it's empty
            if (!String.IsNullOrWhiteSpace(part.Path))
            {
                return;
            }

            var pattern = GetPattern(part);

            if (!String.IsNullOrEmpty(pattern))
            {
                var templateContext = new TemplateContext();
                templateContext.SetValue("ContentItem", part.ContentItem);

                part.Path = _liquidTemplateManager.RenderAsync(pattern, templateContext).GetAwaiter().GetResult();

                if (!IsPathUnique(part.Path, part))
                {
                    part.Path = GenerateUniquePath(part.Path, part);
                }

                part.Apply();
            }
        }

        private void RemoveTag(AutoroutePart part)
        {
            _tagCache.RemoveTag($"alias:{part.Path}");
        }

        /// <summary>
        /// Get the pattern from the AutoroutePartSettings property for its type
        /// </summary>
        private string GetPattern(AutoroutePart part)
        {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(part.ContentItem.ContentType);
            var contentTypePartDefinition = contentTypeDefinition.Parts.FirstOrDefault(x => String.Equals(x.PartDefinition.Name, "AutoroutePart", StringComparison.Ordinal));
            var pattern = contentTypePartDefinition.Settings.ToObject<AutoroutePartSettings>().Pattern;

            return pattern;
        }

        private string GenerateUniquePath(string path, AutoroutePart context)
        {
            var version = 1;
            var unversionedPath = path;

            var versionSeparatorPosition = path.LastIndexOf('-');
            if (versionSeparatorPosition > -1)
            {
                int.TryParse(path.Substring(versionSeparatorPosition).TrimStart('-'), out version);
                unversionedPath = path.Substring(0, versionSeparatorPosition);
            }

            while (true)
            {
                var versionedPath = $"{unversionedPath}-{version++}";
                if (IsPathUnique(versionedPath, context))
                {
                    return versionedPath;
                }
            }
        }

        private bool IsPathUnique(string path, AutoroutePart context)
        {
            return _session.QueryIndex<AutoroutePartIndex>(o => o.ContentItemId != context.ContentItem.ContentItemId && o.Path == path).CountAsync().GetAwaiter().GetResult() == 0;
        }
    }
}