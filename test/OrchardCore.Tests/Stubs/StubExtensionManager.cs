﻿using OrchardCore.Environment.Extensions;
using System;
using OrchardCore.Environment.Extensions.Loaders;
using OrchardCore.Environment.Extensions.Features;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrchardCore.Tests.Stubs
{
    public class StubExtensionManager : IExtensionManager
    {
        public IEnumerable<IFeatureInfo> GetDependentFeatures(string featureId)
        {
            throw new NotImplementedException();
        }

        public IExtensionInfo GetExtension(string extensionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IExtensionInfo> GetExtensions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFeatureInfo> GetFeatureDependencies(string featureId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFeatureInfo> GetFeatures()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFeatureInfo> GetFeatures(string[] featureIdsToLoad)
        {
            throw new NotImplementedException();
        }

        public Task<ExtensionEntry> LoadExtensionAsync(IExtensionInfo extensionInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FeatureEntry>> LoadFeaturesAsync(string[] featureIdsToLoad)
        {
            throw new NotImplementedException();
        }
    }
}