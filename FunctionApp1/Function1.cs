using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace FunctionApp1
{
    public class Function1
    {
        private readonly BlobServiceClient _blobServiceClient;

        public Function1(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("salesorder", Connection = "ServiceBusConnectionString")] string myQueueItem, ILogger log)
        {
            // Convert myQueueItem to MessageInfo object
            var messageInfo = Utility.ConvertJsonToObject<MessageInfo>(myQueueItem);
            // Log the messageInfo
            log.LogInformation($"\u001b[35mCorrelationID: {messageInfo.CorrelationID}\u001b[0m"); // Purple color
            // Log the blob location
            log.LogInformation($"\u001b[35mBlobLocation: {messageInfo.BlobLocation}\u001b[0m"); // Purple color

            // Example: Access Blob storage
            string blobContainerName = "blobayoub";
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient = containerClient.GetBlobClient(messageInfo.BlobLocation);

            // Download the blob content
            var response = blobClient.Download();
            using (var reader = new System.IO.StreamReader(response.Value.Content))
            {
                string blobContent = reader.ReadToEnd();
                log.LogInformation($"BlobContent: {blobContent}");
            }

            // Example: Access Cosmos DB
            /*
            string cosmosDatabaseName = "Integration";
            string cosmosContainerName = "salesorders";
            var container = _cosmosClient.GetContainer(cosmosDatabaseName, cosmosContainerName);
            */
            // Use container methods to interact with Cosmos DB

            // Perform other processing logic as needed
        }
    }
}
