using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace Assignment3VasylMilchevskyi.Services
{
    public class ImageUploader
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public ImageUploader()
        {
            _storageClient = StorageClient.Create();
            // [END storageclient]
        }

        // [START uploadimage]
        public async Task<String> UploadFile(IFormFile file, long id, string bucket)
        {
            var imageAcl = PredefinedObjectAcl.PublicRead;

            var imageObject = await _storageClient.UploadObjectAsync(
                bucket: bucket,
                objectName: id.ToString(),
                contentType: file.ContentType,
                source: file.OpenReadStream(),
                options: new UploadObjectOptions { PredefinedAcl = imageAcl }
            );
            return imageObject.MediaLink;
        }

        public async Task DeleteUploadedImage(long id)
        {
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, id.ToString());
            }
            catch (Google.GoogleApiException exception)
            {
                if (exception.Error.Code != 404)
                    throw;
            }
        }
    }
}
