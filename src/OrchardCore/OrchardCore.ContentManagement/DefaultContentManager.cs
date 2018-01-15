using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Builders;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Modules;
using YesSql;

namespace OrchardCore.ContentManagement
{
    public class DefaultContentManager : IContentManager
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ISession _session;
        private readonly ILogger _logger;
        private readonly DefaultContentManagerSession _contentManagerSession;
        private readonly IContentItemIdGenerator _idGenerator;

        public DefaultContentManager(
            IContentDefinitionManager contentDefinitionManager,
            IEnumerable<IContentHandler> handlers,
            ISession session,
            IContentItemIdGenerator idGenerator,
            ILogger<DefaultContentManager> logger)
        {
            _contentDefinitionManager = contentDefinitionManager;
            Handlers = handlers;
            ReversedHandlers = handlers.Reverse().ToArray();
            _session = session;
            _idGenerator = idGenerator;
            _contentManagerSession = new DefaultContentManagerSession();
            _logger = logger;
        }

        public IEnumerable<IContentHandler> Handlers { get; private set; }
        public IEnumerable<IContentHandler> ReversedHandlers { get; private set; }

        public ContentItem New(string contentType)
        {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(contentType);
            if (contentTypeDefinition == null)
            {
                contentTypeDefinition = new ContentTypeDefinitionBuilder().Named(contentType).Build();
            }

            // create a new kernel for the model instance
            var context = new ActivatingContentContext
            {
                ContentType = contentTypeDefinition.Name,
                Definition = contentTypeDefinition,
                Builder = new ContentItemBuilder(contentTypeDefinition)
            };

            // invoke handlers to weld aspects onto kernel
            Handlers.Invoke(handler => handler.Activating(context), _logger);

            var context2 = new ActivatedContentContext(context.Builder.Build());

            context2.ContentItem.ContentItemId = _idGenerator.GenerateUniqueId(context2.ContentItem);
            context2.ContentItem.ContentItemVersionId = _idGenerator.GenerateUniqueId(context2.ContentItem);

            ReversedHandlers.Invoke(handler => handler.Activated(context2), _logger);

            var context3 = new InitializingContentContext(context2.ContentItem);

            Handlers.Invoke(handler => handler.Initializing(context3), _logger);
            ReversedHandlers.Invoke(handler => handler.Initialized(context3), _logger);

            // composite result is returned
            return context3.ContentItem;
        }

        public Task<ContentItem> GetAsync(string contentItemId)
        {
            if (contentItemId == null)
            {
                throw new ArgumentNullException(nameof(contentItemId));
            }

            return GetAsync(contentItemId, VersionOptions.Published);
        }

        public async Task<ContentItem> GetAsync(string contentItemId, VersionOptions options)
        {
            ContentItem contentItem = null;

            if (options.IsLatest)
            {
                contentItem = await _session
                    .Query<ContentItem, ContentItemIndex>()
                    .Where(x => x.ContentItemId == contentItemId && x.Latest == true)
                    .FirstOrDefaultAsync();
            }
            else if (options.IsDraft && !options.IsDraftRequired)
            {
                contentItem = await _session
                    .Query<ContentItem, ContentItemIndex>()
                    .Where(x =>
                        x.ContentItemId == contentItemId &&
                        x.Published == false &&
                        x.Latest == true)
                    .FirstOrDefaultAsync();
            }
            else if (options.IsDraft || options.IsDraftRequired)
            {
                // Loaded whatever is the latest as it will be cloned
                contentItem = await _session
                    .Query<ContentItem, ContentItemIndex>()
                    .Where(x =>
                        x.ContentItemId == contentItemId &&
                        x.Latest == true)
                    .FirstOrDefaultAsync();
            }
            else if (options.IsPublished)
            {
                // If the published version is requested and is already loaded, we can
                // return it right away
                if(_contentManagerSession.RecallPublishedItemId(contentItemId, out contentItem))
                {
                    return contentItem;
                }

                contentItem = await _session
                    .Query<ContentItem, ContentItemIndex>()
                    .Where(x => x.ContentItemId == contentItemId && x.Published == true)
                    .FirstOrDefaultAsync();
            }

            if (contentItem == null)
            {
                if (!options.IsDraftRequired)
                {
                    return null;
                }
            }

            // Return item if obtained earlier in session
            // If IsPublished is required then the test has already been checked before
            ContentItem recalled = null;
            if (!_contentManagerSession.RecallVersionId(contentItem.Id, out recalled))
            {
                // store in session prior to loading to avoid some problems with simple circular dependencies
                _contentManagerSession.Store(contentItem);

                // create a context with a new instance to load
                var context = new LoadContentContext(contentItem);

                // invoke handlers to acquire state, or at least establish lazy loading callbacks
                Handlers.Invoke(handler => handler.Loading(context), _logger);
                ReversedHandlers.Invoke(handler => handler.Loaded(context), _logger);

                contentItem = context.ContentItem;
            }
            else
            {
                contentItem = recalled;
            }

            if (options.IsDraftRequired)
            {
                // When draft is required and latest is published a new version is added
                if (contentItem.Published)
                {
                    // Save the previous version
                    _session.Save(contentItem);

                    contentItem = await BuildNewVersionAsync(contentItem);
                }

                // Save the new version
                _session.Save(contentItem);
            }

            return contentItem;
        }

        public async Task<ContentItem> GetVersionAsync(string contentItemVersionId)
        {
            var contentItem = await _session
                .Query<ContentItem, ContentItemIndex>(x => x.ContentItemVersionId == contentItemVersionId)
                .FirstOrDefaultAsync();

            if (contentItem == null)
            {
                return null;
            }

            if (!_contentManagerSession.RecallVersionId(contentItem.Id, out contentItem))
            {
                // store in session prior to loading to avoid some problems with simple circular dependencies
                _contentManagerSession.Store(contentItem);

                // create a context with a new instance to load
                var context = new LoadContentContext(contentItem);

                // invoke handlers to acquire state, or at least establish lazy loading callbacks
                Handlers.Invoke(handler => handler.Loading(context), _logger);
                ReversedHandlers.Invoke(handler => handler.Loaded(context), _logger);

                contentItem = context.ContentItem;
            }

            return contentItem;
        }

        public async Task PublishAsync(ContentItem contentItem)
        {
            if (contentItem.Published)
            {
                return;
            }

            // Create a context for the item and it's previous published record
            // Because of this query the content item will need to be re-enlisted
            // to be saved.
            var previous = await _session
                .Query<ContentItem, ContentItemIndex>(x =>
                    x.ContentItemId == contentItem.ContentItemId && x.Published)
                .FirstOrDefaultAsync();

            var context = new PublishContentContext(contentItem, previous);

            // invoke handlers to acquire state, or at least establish lazy loading callbacks
            Handlers.Invoke(handler => handler.Publishing(context), _logger);

            if (context.Cancel)
            {
                return;
            }

            if (previous != null)
            {
                _session.Save(previous);
                previous.Published = false;
            }

            contentItem.Published = true;

            _session.Save(contentItem);

            ReversedHandlers.Invoke(handler => handler.Published(context), _logger);
        }

        public async Task UnpublishAsync(ContentItem contentItem)
        {
            // This method needs to be called using the latest version
            if (!contentItem.Latest)
            {
                throw new InvalidOperationException("Not the latest version.");
            }

            ContentItem publishedItem;
            if (contentItem.Published)
            {
                // The version passed in is the published one
                publishedItem = contentItem;
            }
            else
            {
                // Try to locate the published version of this item
                publishedItem = await GetAsync(contentItem.ContentItemId, VersionOptions.Published);
            }

            if (publishedItem == null)
            {
                // No published version exists. no work to perform.
                return;
            }

            // Create a context for the item. the publishing version is null in this case
            // and the previous version is the one active prior to unpublishing. handlers
            // should take this null check into account
            var context = new PublishContentContext(contentItem, publishedItem)
            {
                PublishingItem = null
            };

            Handlers.Invoke(handler => handler.Unpublishing(context), _logger);

            publishedItem.Published = false;
            
            _session.Save(publishedItem);

            ReversedHandlers.Invoke(handler => handler.Unpublished(context), _logger);
        }

        protected async Task<ContentItem> BuildNewVersionAsync(ContentItem existingContentItem)
        {
            var buildingContentItem = New(existingContentItem.ContentType);

            ContentItem latestVersion;

            if (existingContentItem.Latest)
            {
                latestVersion = existingContentItem;
            }
            else
            {
                latestVersion = await _session
                    .Query<ContentItem, ContentItemIndex>(x =>
                        x.ContentItemId == existingContentItem.ContentItemId &&
                        x.Latest)
                    .FirstOrDefaultAsync();
            }

            if (latestVersion != null)
            {
                latestVersion.Latest = false;
                buildingContentItem.Number = latestVersion.Number + 1;
            }
            else
            {
                buildingContentItem.Number = 1;
            }

            buildingContentItem.ContentItemId = existingContentItem.ContentItemId;
            buildingContentItem.ContentItemVersionId = _idGenerator.GenerateUniqueId(existingContentItem);
            buildingContentItem.Latest = true;
            buildingContentItem.Data = new JObject(existingContentItem.Data);

            var context = new VersionContentContext(existingContentItem, buildingContentItem);

            Handlers.Invoke(handler => handler.Versioning(context), _logger);
            ReversedHandlers.Invoke(handler => handler.Versioned(context), _logger);

            return context.BuildingContentItem;
        }

        public void Create(ContentItem contentItem)
        {
            Create(contentItem, VersionOptions.Published);
        }

        public void Create(ContentItem contentItem, VersionOptions options)
        {
            if (contentItem.Number == 0)
            {
                contentItem.Number = 1;
                contentItem.Latest = true;
                contentItem.Published = true;
            }

            if (String.IsNullOrEmpty(contentItem.ContentItemVersionId))
            {
                contentItem.ContentItemVersionId = _idGenerator.GenerateUniqueId(contentItem);
            }
            
            // Draft flag on create is required for explicitly-published content items
            if (options.IsDraft)
            {
                contentItem.Published = false;
            }

            // Build a context with the initialized instance to create
            var context = new CreateContentContext(contentItem);

            // invoke handlers to add information to persistent stores
            Handlers.Invoke(handler => handler.Creating(context), _logger);

            ReversedHandlers.Invoke(handler => handler.Created(context), _logger);

            _session.Save(contentItem);
            _contentManagerSession.Store(contentItem);

            if (options.IsPublished)
            {
                var publishContext = new PublishContentContext(contentItem, null);

                // invoke handlers to acquire state, or at least establish lazy loading callbacks
                Handlers.Invoke(handler => handler.Publishing(publishContext), _logger);

                // invoke handlers to acquire state, or at least establish lazy loading callbacks
                ReversedHandlers.Invoke(handler => handler.Published(publishContext), _logger);
            }
        }

        public TAspect PopulateAspect<TAspect>(IContent content, TAspect aspect)
        {
            var context = new ContentItemAspectContext
            {
                ContentItem = content.ContentItem,
                Aspect = aspect
            };

            Handlers.Invoke(handler => handler.GetContentItemAspect(context), _logger);

            return aspect;
        }

        public async Task RemoveAsync(ContentItem contentItem)
        {
            var activeVersions = await _session.Query<ContentItem, ContentItemIndex>()
                .Where(x =>
                    x.ContentItemId == contentItem.ContentItemId &&
                    (x.Published || x.Latest)).ListAsync();

            var context = new RemoveContentContext(contentItem);

            Handlers.Invoke(handler => handler.Removing(context), _logger);

            foreach (var version in activeVersions)
            {
                version.Published = false;
                version.Latest = false;
                _session.Save(version);
            }

            ReversedHandlers.Invoke(handler => handler.Removed(context), _logger);
        }

        public async Task DiscardDraftAsync(ContentItem contentItem)
        {
            if (contentItem.Published || !contentItem.Latest)
            {
                throw new InvalidOperationException("Not a draft version.");
            }

            var context = new RemoveContentContext(contentItem);

            Handlers.Invoke(handler => handler.Removing(context), _logger);

            contentItem.Latest = false;
            _session.Save(contentItem);

            ReversedHandlers.Invoke(handler => handler.Removed(context), _logger);

            var publishedItem = await GetAsync(contentItem.ContentItemId, VersionOptions.Published);

            if (publishedItem != null)
            {
                publishedItem.Latest = true;
                _session.Save(publishedItem);
            }
        }
    }
}