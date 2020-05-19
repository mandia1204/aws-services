using Amazon.KeyManagementService.Model;
using aws_services.client.Models;
using System.Threading.Tasks;

namespace aws_services.client.Services
{
    public interface ICrypto
    {
        Task<string> Encrypt(string data);
        Task<string> Decrypt(string data);
        Task<CryptoData> EncryptWithKey(string data);
        Task<string> DecryptWithKey(CryptoData data);
    }
}