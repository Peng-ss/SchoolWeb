﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;

namespace OrchardCore.Settings.Recipes
{
    /// <summary>
    /// This recipe step updates the site settings.
    /// </summary>
    public class SettingsStep : IRecipeStepHandler
    {
        private readonly ISiteService _siteService;

        public SettingsStep(ISiteService siteService)
        {
            _siteService = siteService;
        }

        public async Task ExecuteAsync(RecipeExecutionContext context)
        {
            if (!String.Equals(context.Name, "Settings", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var model = context.Step;
            var site = await _siteService.GetSiteSettingsAsync();

            foreach(JProperty property in model.Properties())
            {
                switch (property.Name)
                {
                    case "BaseUrl":
                        site.BaseUrl = property.Value.ToString();
                        break;

                    case "Calendar":
                        site.Calendar = property.Value.ToString();
                        break;

                    case "Culture":
                        site.Culture = property.Value.ToString();
                        break;

                    case "MaxPagedCount":
                        site.MaxPagedCount = property.Value<int>();
                        break;

                    case "MaxPageSize":
                        site.MaxPageSize = property.Value<int>();
                        break;

                    case "PageSize":
                        site.PageSize = property.Value<int>();
                        break;

                    case "ResourceDebugMode":
                        site.ResourceDebugMode = property.Value<ResourceDebugMode>();
                        break;

                    case "SiteName":
                        site.SiteName = property.ToString();
                        break;

                    case "SiteSalt":
                        site.SiteSalt = property.ToString();
                        break;

                    case "SuperUser":
                        site.SuperUser = property.ToString();
                        break;

                    case "TimeZone":
                        site.TimeZone = property.ToString();
                        break;

                    case "UseCdn":
                        site.UseCdn = property.Value<bool>();
                        break;

                    case "HomeRoute":
                        site.HomeRoute = property.Value.ToObject<RouteValueDictionary>();
                        break;

                    default:
                        site.Properties.Add(property);
                        break;
                }
            }            

            await _siteService.UpdateSiteSettingsAsync(site);
        }
    }
}