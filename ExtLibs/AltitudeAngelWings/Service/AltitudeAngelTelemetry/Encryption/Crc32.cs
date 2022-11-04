namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public static class Crc32
    {
        // GZip compatible CRC32 Polynomial.
        private const uint Polynomial = 0xEDB88320;

        private static readonly uint[] CrcTable;

        static Crc32()
        {
            CrcTable = BuildTable(Polynomial);
        }

        /// <summary>
        ///     Calculates a CRC32 value given an array of bytes.
        /// </summary>
        /// <remarks>
        ///     See here for algorithm description https://en.wikipedia.org/wiki/Cyclic_redundancy_check#CRC-32_algorithm
        /// </remarks>
        /// <param name="bytes">Byte array</param>
        /// <returns>32 bit crc value</returns>
        public static uint CalculateHash(byte[] bytes)
        {
            var crc = 0xFFFFFFFF;
            foreach (var b in bytes)
            {
                var index = (crc ^ b) & 0xff;
                crc = (crc >> 8) ^ CrcTable[index];
            }

            return ~crc;
        }

        private static uint[] BuildTable(uint polynomial)
        {
            var table = new uint[256];

            for (var i = 0; i < 256; i++)
            {
                var crc = (uint)i;
                for (var j = 0; j < 8; j++)
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;

                table[i] = crc;
            }

            return table;
        }
    }
}