using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Simucraft.Server.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Simucraft.Server.DataAccess
{
    public class AzureBlobStorage : IBlobStorage
    {
        public async Task DeleteContainerAsync(string container)
        {
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable(Constants.AZURE_STORAGE_CONNECTION_STRING));
            var assetContainer = blobServiceClient.GetBlobContainerClient(container);
            await assetContainer.DeleteAsync();
        }

        public async Task DeleteBlobAsync(string container, string blob)
        {
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable(Constants.AZURE_STORAGE_CONNECTION_STRING));
            var assetContainer = blobServiceClient.GetBlobContainerClient(container);
            var exists = await assetContainer.ExistsAsync();
            if (!exists)
                return;

            await assetContainer.DeleteBlobAsync(blob, DeleteSnapshotsOption.IncludeSnapshots);
        }

        public async Task<string> SaveImageAsync(string container, string id, Stream stream)
        {
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable(Constants.AZURE_STORAGE_CONNECTION_STRING));
            var assetContainer = blobServiceClient.GetBlobContainerClient(container);
            await assetContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var client = assetContainer.GetBlobClient(id);
            await client.UploadAsync(stream, true);

            return client.Uri.AbsoluteUri;
        }

        public async Task<Stream> GetBlobAsync(string container, string id)
        {
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable(Constants.AZURE_STORAGE_CONNECTION_STRING));
            var assetContainer = blobServiceClient.GetBlobContainerClient(container);
            await assetContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var client = assetContainer.GetBlobClient(id);
            var download = await client.DownloadAsync();

            return download.Value.Content;
        }
    }
}
