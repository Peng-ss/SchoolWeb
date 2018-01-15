using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Shell;

namespace OrchardCore.Localization
{
    public class ModularPoFileLocationProvider : ILocalizationFileLocationProvider
    {
        private const string PoFileName = "OrchardCore.po";

        private readonly IExtensionManager _extensionsManager;
        private readonly string _root;
        private readonly string _resourcesContainer;
        private readonly string _shellContainer;
        private readonly string _shellName;

        public ModularPoFileLocationProvider(
            IExtensionManager extensionsManager,
            IHostingEnvironment hostingEnvironment,
            IOptions<ShellOptions> shellOptions,
            IOptions<LocalizationOptions> localizationOptions,
            ShellSettings shellSettings)
        {
            _extensionsManager = extensionsManager;

            _root = hostingEnvironment.ContentRootPath;
            _resourcesContainer = localizationOptions.Value.ResourcesPath; // Localization
            _shellContainer = shellOptions.Value.ShellsContainerName;
            _shellName = shellSettings.Name;
        }

        public IEnumerable<string> GetLocations(string cultureName)
        {
            // Load .po files in each extension folder first, based on the extensions order
            foreach (var extension in _extensionsManager.GetExtensions())
            {
                yield return Path.Combine(_root, extension.SubPath, _resourcesContainer, cultureName, PoFileName);
            }

            // Then load global .po file for the applications
            yield return Path.Combine(_root, _resourcesContainer, cultureName, PoFileName);

            // Finally load tenant-specific .po file
            yield return Path.Combine(_root, _shellContainer, _shellName, _resourcesContainer, cultureName, PoFileName);
        }
    }
}
