using System;
using System.Collections.Generic;
using System.Text;

namespace Aura_OS.System.Security
{
    public static class Crypto
    {
        public static int getCRC(byte[] s)
        {
            int hash = (int)Crc32.Compute(s);
            return hash;
        }
    }
}
