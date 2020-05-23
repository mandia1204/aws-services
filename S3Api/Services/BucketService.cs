using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using S3Api.Models;

namespace S3Api.Services
{
    public class BucketService: IBucketService
    {
        private IAmazonS3 s3Client;
        private readonly RegionEndpoint region = RegionEndpoint.USEast2;

        public BucketService()
        {
            s3Client = new AmazonS3Client(region);
        }

        public async Task<IEnumerable<BucketObject>> GetObjects(string bucketName)
        {
            try
            {
                var response = await s3Client.ListObjectsAsync(bucketName);

                return response.S3Objects.Select(o => new BucketObject
                {
                    Name = o.Key,
                    Size = o.Size
                }).Take(2);
            }
            catch(AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
                throw s3Exception;
            }
        }
    }
}
