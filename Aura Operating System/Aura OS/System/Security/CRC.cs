using System;
using System.Collections.Generic;
using System.Text;

namespace Aura_OS.System.Security
{
    public abstract class HashAlgorithm
    {
        public byte[] HashValue;
        public string getString(byte[] data)
        {
            string digest = "";
            byte[] dat = Calculate(data);
            foreach (byte b in dat)
            {
                digest += Conversions.ByteToHex(b);
            }
            return digest;
        }
        public byte[] Calculate(string str)
        {
            byte[] dat = new byte[str.Length];
            int pos = 0;
            foreach (byte b in str)
            {
                dat[pos] = b;
                pos++;
            }
            return dat;
        }
        public virtual byte[] Calculate(byte[] data)
        { return null; }

        public virtual void Calculate(byte[] data, ref uint val) { }
    }
    public class Crc32 : HashAlgorithm
    {
        public const UInt32 DefaultPolynomial = 0xedb88320;
        public const UInt32 DefaultSeed = 0xffffffff;

        private UInt32 hash;
        private UInt32 seed;
        private UInt32[] table;
        private static UInt32[] defaultTable;

        public Crc32()
        {
            table = InitializeTable(DefaultPolynomial);
            seed = DefaultSeed;
            hash = seed;
        }

        public Crc32(UInt32 polynomial, UInt32 seed)
        {
            table = InitializeTable(polynomial);
            this.seed = seed;
            hash = seed;
        }


        public override void Calculate(byte[] data, ref uint val)
        {
            val = Compute(data);
        }
        void HashCore(byte[] buffer, int start, int length)
        {
            hash = CalculateHash(table, hash, buffer, start, length);
        }

        byte[] HashFinal()
        {
            byte[] hashBuffer = UInt32ToBigEndianBytes(~hash);
            this.HashValue = hashBuffer;
            return hashBuffer;
        }

        public static UInt32 Compute(byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
        }

        public static UInt32 Compute(UInt32 seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
        }

        public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        private static UInt32[] InitializeTable(UInt32 polynomial)
        {
            if (polynomial == DefaultPolynomial && defaultTable != null)
                return defaultTable;

            UInt32[] createTable = new UInt32[256];
            for (int i = 0; i < 256; i++)
            {
                UInt32 entry = (UInt32)i;
                for (int j = 0; j < 8; j++)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry = entry >> 1;
                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial)
                defaultTable = createTable;

            return createTable;
        }

        private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size)
        {
            UInt32 crc = seed;
            for (int i = start; i < size; i++)
                unchecked
                {
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                }
            return crc;
        }

        private byte[] UInt32ToBigEndianBytes(UInt32 x)
        {
            return new byte[] {
            (byte)((x >> 24) & 0xff),
            (byte)((x >> 16) & 0xff),
            (byte)((x >> 8) & 0xff),
            (byte)(x & 0xff)
        };
        }
    }

    public class Hash
    {
        public static int GHash(string s)
        {
            // I know this is a horrible hash algol , I need to implement MD5!!!!
            int hash = 23;
            int pos = 0;
            foreach (char c in s.ToCharArray())
            {
                hash = hash ^ pos ^ c;
            }
            return hash ^ s.Length;
        }
        
    }
}
