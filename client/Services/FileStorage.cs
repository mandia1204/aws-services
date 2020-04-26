using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace aws_services.Services
{
    public class FileStorage : IFileStorage
    {
        private IAmazonS3 client;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        public FileStorage()
        {
            client = new AmazonS3Client(bucketRegion);
        }

        public async Task<string> ReadObject(string bucketName, string keyName)
        {
            string responseBody = "";
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                using (var response = await client.GetObjectAsync(request))
                using (var responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    responseBody = reader.ReadToEnd();
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading an object", e.Message);
            }
            return responseBody;
        }
    }
}
