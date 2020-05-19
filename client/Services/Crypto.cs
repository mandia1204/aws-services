using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using aws_services.client.Models;
using Microsoft.AspNetCore.DataProtection;

namespace aws_services.client.Services
{
    public class Crypto : ICrypto
    {
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private readonly IAmazonKeyManagementService client;
        private readonly string keyId = "d08b27f8-7cc7-4cd5-a1b2-314ed34fa71b"; //from console
        public Crypto(IDataProtectionProvider provider)
        {
            client = new AmazonKeyManagementServiceClient(bucketRegion);
            dataProtectionProvider = provider;
        }

        public async Task<string> Encrypt(string data)
        {
            using (var stream = GenerateStreamFromString(data))
            {
                var req = new EncryptRequest
                {
                    Plaintext = stream,
                    KeyId = this.keyId
                };
                var response = await client.EncryptAsync(req);
                var result = Convert.ToBase64String(response.CiphertextBlob.ToArray());
                return result;
            }
        }

        public async Task<string> Decrypt(string data)
        {
            using var stream = GenerateStreamFromBase64String(data);
            var req = new DecryptRequest
            {
                KeyId = this.keyId,
                CiphertextBlob = stream
            };
            var response = await client.DecryptAsync(req);
            var result = GetValueFromStream(response.Plaintext);
            return result;
        }


        public async Task<CryptoData> EncryptWithKey(string data)
        {
            var keyReq = new GenerateDataKeyRequest
            {
                KeyId = this.keyId,
                KeySpec = DataKeySpec.AES_256
            };
            var dataKeyResponse = await client.GenerateDataKeyAsync(keyReq);
            var key = GetBase64StringFromStream(dataKeyResponse.CiphertextBlob);
            var plainTextKey = GetBase64StringFromStream(dataKeyResponse.Plaintext);
            var protector = dataProtectionProvider.CreateProtector(plainTextKey);
            var result = protector.Protect(data);
            return new CryptoData
            { 
                Encrypted = result,
                Key = key
            };
        }

        public async Task<string> DecryptWithKey(CryptoData data)
        {
            var ciphertextBlob = GenerateStreamFromBase64String(data.Key);
            var plainText = await DecryptDataKey(ciphertextBlob);

            var protector = dataProtectionProvider.CreateProtector(GetBase64StringFromStream(plainText));
            return protector.Unprotect(data.Encrypted);
        }

        private async Task<MemoryStream> DecryptDataKey(MemoryStream ciphertextBlob)
        {
            var decryptRequest = new DecryptRequest
            {
                CiphertextBlob = ciphertextBlob,
                KeyId = this.keyId
            };
            var response = await client.DecryptAsync(decryptRequest);
            return response.Plaintext;
        }

        private MemoryStream GenerateStreamFromString(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            return new MemoryStream(bytes, 0, bytes.Length);
        }

        private MemoryStream GenerateStreamFromBase64String(string s)
        {
            var bytes = Convert.FromBase64String(s);
            return new MemoryStream(bytes, 0, bytes.Length);
        }

        private string GetValueFromStream(MemoryStream stream)
        {
            using (stream)
            {

                using (var reader = new StreamReader(stream, Encoding.Default))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string GetBase64StringFromStream(MemoryStream stream)
        {
            var encoded = Convert.ToBase64String(stream.ToArray());
            return encoded;
        }
    }
}
