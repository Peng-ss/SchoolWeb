﻿using Microsoft.AspNetCore.Routing;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Environment.Cache;

namespace OrchardCore.Contents.Handlers
{
    public class ContentsHandler : ContentHandlerBase
    {
        private readonly ITagCache _tagCache;

        public ContentsHandler(ITagCache tagCache)
        {
            _tagCache = tagCache;
        }

        public override void Published(PublishContentContext context)
        {
            _tagCache.RemoveTag($"contentitemid:{context.ContentItem.ContentItemId}");
        }

        public override void Removed(RemoveContentContext context)
        {
            _tagCache.RemoveTag($"contentitemid:{context.ContentItem.ContentItemId}");
        }

        public override void Unpublished(PublishContentContext context)
        {
            _tagCache.RemoveTag($"contentitemid:{context.ContentItem.ContentItemId}");
        }

        public override void GetContentItemAspect(ContentItemAspectContext context)
        {
            context.For<ContentItemMetadata>(metadata =>
            {
                if (metadata.CreateRouteValues == null)
                {
                    metadata.CreateRouteValues = new RouteValueDictionary {
                        {"Area", "OrchardCore.Contents"},
                        {"Controller", "Admin"},
                        {"Action", "Create"},
                        {"Id", context.ContentItem.ContentType}
                    };
                }

                if (metadata.EditorRouteValues == null)
                {
                    metadata.EditorRouteValues = new RouteValueDictionary {
                        {"Area", "OrchardCore.Contents"},
                        {"Controller", "Admin"},
                        {"Action", "Edit"},
                        {"ContentItemId", context.ContentItem.ContentItemId}
                    };
                }

                if (metadata.AdminRouteValues == null)
                {
                    metadata.AdminRouteValues = new RouteValueDictionary {
                        {"Area", "OrchardCore.Contents"},
                        {"Controller", "Admin"},
                        {"Action", "Edit"},
                        {"ContentItemId", context.ContentItem.ContentItemId}
                    };
                }

                if (metadata.DisplayRouteValues == null)
                {
                    metadata.DisplayRouteValues = new RouteValueDictionary {
                        {"Area", "OrchardCore.Contents"},
                        {"Controller", "Item"},
                        {"Action", "Display"},
                        {"ContentItemId", context.ContentItem.ContentItemId}
                    };
                }

                if (metadata.RemoveRouteValues == null)
                {
                    metadata.RemoveRouteValues = new RouteValueDictionary {
                        {"Area", "OrchardCore.Contents"},
                        {"Controller", "Admin"},
                        {"Action", "Remove"},
                        {"ContentItemId", context.ContentItem.ContentItemId}
                    };
                }
            });
        }
    }
}