using Simucraft.Server.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Simucraft.Client.Common;

namespace Simucraft.Server.Services
{
    public class RulesetBlobStorage : IRulesetBlobStorage
    {
        private readonly IBlobStorage _blogStorage;

        public RulesetBlobStorage(IBlobStorage blobStorage)
        {
            _blogStorage = blobStorage;
        }

        public async Task<string> SaveImageAsync(Guid rulesetId, Guid entityId, Stream image)
        {
            return await _blogStorage.SaveImageAsync(rulesetId.ToString(), entityId.ToString(), image);
        }

        public Task DeleteContainerAsync(Guid rulesetId) =>
            _blogStorage.DeleteContainerAsync(rulesetId.ToString());

        public Task DeleteAssetAsync(Guid rulesetId, Guid entityId) =>
            _blogStorage.DeleteBlobAsync(rulesetId.ToString(), entityId.ToString());

        public Task<Stream> GetBlobAsync(Guid rulesetId, Guid assetId) =>
            _blogStorage.GetBlobAsync(rulesetId.ToString(), assetId.ToString());
    }
}
