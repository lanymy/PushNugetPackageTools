﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace PushNugetPackageTools.Models
{


    public class NuGetServerManipulater
    {

        public string NugetServerUrl { get; }
        public bool IsV3 { get; }

        private readonly string _ApiKey;

        private readonly SourceRepository _CurrentRepository;


        public NuGetServerManipulater(string apiKey, string nugetServerUrl = "https://api.nuget.org/v3/index.json", bool isV3 = true)
        {

            _ApiKey = apiKey;
            NugetServerUrl = nugetServerUrl;
            IsV3 = isV3;
            _CurrentRepository = CreateRepository(NugetServerUrl);

        }




        private SourceRepository CreateRepository(string source)
        {
            return PackageRepositoryFactory.CreateRepository(source);
            //return CreateRepository(new PackageSource(source), null);
        }

        //private SourceRepository CreateRepository(PackageSource packageSource, IEnumerable<Lazy<INuGetResourceProvider>> additionalProviders)
        //{

        //    //var providers = Repository.Provider.GetCoreV3();
        //    var providers = Repository.Provider.GetCoreV3();

        //    if (additionalProviders != null)
        //    {
        //        providers = providers.Concat(additionalProviders);
        //    }

        //    return Repository.CreateSource(providers, packageSource);

        //}


        public async Task<IPackageSearchMetadata> GetNuGetPackageMetadataByIdAsync(string packageID)
        {

            var findPackageByIdResource = await _CurrentRepository.GetResourceAsync<PackageMetadataResource>();

            var metadata = await findPackageByIdResource.GetMetadataAsync(packageID, false, false, NullSourceCacheContext.Instance, NullLogger.Instance, new CancellationTokenSource().Token);

            var result = metadata.OrderByDescending(m => m.Identity.Version).Take(1).FirstOrDefault();

            return result;

        }


        public async Task PublishNuGetPackage(string packageFileFullPath)
        {

            var packageResource = await _CurrentRepository.GetResourceAsync<PackageUpdateResource>();

            await packageResource.Push(packageFileFullPath, null, 999, false, s => _ApiKey, s => _ApiKey, IsV3, false, null, NullLogger.Instance);

        }



    }


}
