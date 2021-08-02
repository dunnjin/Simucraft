using System;
using System.IO;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IRulesetBlobStorage
    {
        Task<string> SaveImageAsync(Guid rulesetId, Guid assetId, Stream image);

        Task DeleteContainerAsync(Guid rulesetId);

        Task DeleteAssetAsync(Guid rulesetId, Guid assetId);
        Task<Stream> GetBlobAsync(Guid rulesetId, Guid assetId);
    }
}
