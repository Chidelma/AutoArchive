using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace AUTO_ARCHIVE.Services
{
    public class BlobManager : IBlobManager
    {
        CloudStorageAccount storageAccount = null;

        CloudBlobContainer cloudBlobContainer = null;

        private readonly string storageConnectionString;

        public BlobManager(IConfiguration configuration)
        {
            storageConnectionString = configuration["ConnectionStrings:AccessKey"];
        }
        
        public async Task UploadFile(IFormFile File, string containerName)
        {
            if(CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                    cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

                    await cloudBlobContainer.CreateIfNotExistsAsync();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };

                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(File.FileName);

                    using (var stream = File.OpenReadStream())
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }
            }
        }
    }
}
