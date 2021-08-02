using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.DataAccess;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Simucraft.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            TestStorage();

            Console.ReadLine();
        }

        public async static void TestStorage()
        {
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("PRODUCTION_AZURE_STORAGE_CONNECTION"));
            var assetContainer = blobServiceClient.GetBlobContainerClient("test-container");
            await assetContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);
        }

        public static void RecreateDatabase()
        {
            var cosmosDBConnectionString = Environment.GetEnvironmentVariable("PRODUCTION_AZURE_DATABASE_CONNECTION");
            var split = cosmosDBConnectionString.Split(";");
            var endpoint = Regex.Match(split.First(s => s.Contains("AccountEndpoint")), @"(?<=AccountEndpoint=).*").Value;
            var key = Regex.Match(split.First(s => s.Contains("AccountKey")), @"(?<=AccountKey=).*").Value;

            //services.AddDbContext<SimucraftContext>(c =>
            //     c.UseCosmos(endpoint, key, "Simucraft"));
            var dbContextOptions = new DbContextOptionsBuilder()
                .UseCosmos(endpoint, key, "Simucraft").Options;
            using (var context = new SimucraftContext(dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
