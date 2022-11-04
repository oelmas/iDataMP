using System;
using System.IO;
using System.Security.Cryptography;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public class SymmetricEncryptionAlgorithm : IEncryptionAlgorithm
    {
        private readonly SymmetricEncryptionType encryptionType;
        private readonly IEncryptionKeyGenerator keyGenerator;
        private byte keySize;

        public SymmetricEncryptionAlgorithm(SymmetricEncryptionType encryptionType,
            IEncryptionKeyGenerator keyGenerator)
        {
            this.keyGenerator = keyGenerator;
            this.encryptionType = encryptionType;

            InitializeKeySize();
        }

        /// <inheritdoc />
        public byte[] EncryptBytes(Guid telemetryId, int sequenceNumber, byte[] bytes)
        {
            var key = keyGenerator.GenerateKey(telemetryId, keySize);
            return EncryptBytes(key, sequenceNumber, bytes);
        }

        /// <inheritdoc />
        public byte[] EncryptBytes(byte[] key, int sequenceNumber, byte[] bytes)
        {
            using (var encryption = GetAlgorithm())
            {
                encryption.IV = GenerateIv(sequenceNumber, encryption.BlockSize / 8);
                encryption.Key = key;

                using (var encrypted = new MemoryStream())
                {
                    using (var cs = new CryptoStream(encrypted, encryption.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                    }

                    return encrypted.ToArray();
                }
            }
        }

        /// <inheritdoc />
        public byte[] DecryptBytes(Guid telemetryId, int sequenceNumber, byte[] bytes)
        {
            var key = keyGenerator.GenerateKey(telemetryId, keySize);
            return DecryptBytes(key, sequenceNumber, bytes);
        }

        /// <inheritdoc />
        public byte[] DecryptBytes(byte[] key, int sequenceNumber, byte[] bytes)
        {
            using (var encryption = GetAlgorithm())
            {
                encryption.IV = GenerateIv(sequenceNumber, encryption.BlockSize / 8);
                encryption.Key = key;

                using (var decrypted = new MemoryStream())
                {
                    using (var cs = new CryptoStream(decrypted, encryption.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                    }

                    return decrypted.ToArray();
                }
            }
        }

        private byte[] GenerateIv(int sequenceNumber, int byteSize)
        {
            var bytes = BitConverter.GetBytes(sequenceNumber);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

            var ivBytes = new byte[byteSize];
            Array.Copy(bytes, 0, ivBytes, ivBytes.Length - bytes.Length, bytes.Length);

            return ivBytes;
        }

        private void InitializeKeySize()
        {
            switch (encryptionType)
            {
                case SymmetricEncryptionType.Aes128:
                    keySize = 128 / 8;
                    break;
                case SymmetricEncryptionType.Aes192:
                    keySize = 192 / 8;
                    break;
                case SymmetricEncryptionType.Aes256:
                    keySize = 256 / 8;
                    break;
                default: throw new ArgumentException(nameof(encryptionType));
            }
        }

        private SymmetricAlgorithm GetAlgorithm()
        {
            switch (encryptionType)
            {
                case SymmetricEncryptionType.Aes128:
                case SymmetricEncryptionType.Aes192:
                case SymmetricEncryptionType.Aes256:
                    return Aes.Create();
                default: throw new ArgumentException(nameof(encryptionType));
            }
        }
    }
}