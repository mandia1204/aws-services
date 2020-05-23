using S3Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3Api.Services
{
    public interface IBucketService
    {
        Task<IEnumerable<BucketObject>> GetObjects(string bucketName);
    }
}
