using System.Threading.Tasks;

namespace aws_services.Services
{
    public interface IFileStorage
    {
        Task<string> ReadObject(string bucketName, string keyName);
    }
}