using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OrchardCore.Modules;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrchardCore.Environment.Extensions;
using OrchardCore.Recipes.Models;

namespace OrchardCore.Recipes.Services
{
    public class RecipeHarvester : IRecipeHarvester
    {
        private readonly IExtensionManager _extensionManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IOptions<RecipeHarvestingOptions> _recipeOptions;

        public RecipeHarvester(
            IExtensionManager extensionManager,
            IHostingEnvironment hostingEnvironment,
            IOptions<RecipeHarvestingOptions> recipeOptions,
            ILogger<RecipeHarvester> logger)
        {
            _extensionManager = extensionManager;
            _hostingEnvironment = hostingEnvironment;
            _recipeOptions = recipeOptions;

            Logger = logger;
        }

        public ILogger Logger { get; set; }

        public Task<IEnumerable<RecipeDescriptor>> HarvestRecipesAsync()
        {
            return _extensionManager.GetExtensions().InvokeAsync(descriptor =>HarvestRecipes(descriptor), Logger);
        }
        
        private Task<IEnumerable<RecipeDescriptor>> HarvestRecipes(IExtensionInfo extension)
        {
            var folderSubPath = Path.Combine(extension.SubPath, "Recipes");
            return HarvestRecipesAsync(folderSubPath, _recipeOptions.Value, _hostingEnvironment);
        }

        /// <summary>
        /// Returns a list of recipes for a content path.
        /// </summary>
        /// <param name="path">A path string relative to the content root of the application.</param>
        /// <returns>The list of <see cref="RecipeDescriptor"/> instances.</returns>
        public static Task<IEnumerable<RecipeDescriptor>> HarvestRecipesAsync(string path, RecipeHarvestingOptions options, IHostingEnvironment hostingEnvironment)
        {
            var recipeContainerFileInfo = hostingEnvironment
                .ContentRootFileProvider
                .GetFileInfo(path);

            var recipeDescriptors = new List<RecipeDescriptor>();

            var matcher = new Matcher(System.StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude("*.recipe.json");

            var matches = matcher
                .Execute(new DirectoryInfoWrapper(new DirectoryInfo(recipeContainerFileInfo.PhysicalPath)))
                .Files;

            if (matches.Any())
            {
                var result = matches
                    .Select(match => hostingEnvironment
                        .ContentRootFileProvider
                        .GetFileInfo(Path.Combine(path, match.Path))).ToArray();

                recipeDescriptors.AddRange(result.Select(recipeFile =>
                {
                    // TODO: Try to optimize by only reading the required metadata instead of the whole file
                    using (StreamReader file = File.OpenText(recipeFile.PhysicalPath))
                    {
                        using (var reader = new JsonTextReader(file))
                        {
                            var serializer = new JsonSerializer();
                            var recipeDescriptor = serializer.Deserialize<RecipeDescriptor>(reader);
                            recipeDescriptor.RecipeFileInfo = recipeFile;
                            return recipeDescriptor;
                        }
                    }
                }));
            }

            return Task.FromResult<IEnumerable<RecipeDescriptor>>(recipeDescriptors);
        }
    }
}