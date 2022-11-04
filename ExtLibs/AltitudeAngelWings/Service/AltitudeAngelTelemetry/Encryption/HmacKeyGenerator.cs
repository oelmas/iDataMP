using System;
using System.Text;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public class HmacKeyGenerator : IEncryptionKeyGenerator
    {
        private readonly Hkdf hkdf;
        private readonly byte[] keyBytes;


        public HmacKeyGenerator(HashType hashType, string encryptionKeySecret)
        {
            hkdf = new Hkdf(hashType);

            keyBytes = Encoding.UTF8.GetBytes(encryptionKeySecret);
        }

        /// <inheritdoc />
        public byte[] GenerateKey(Guid telemetryId, byte keySize)
        {
            var salt = telemetryId.ToByteArray();
            return hkdf.DeriveKey(salt, keyBytes, keySize);
        }
    }
}