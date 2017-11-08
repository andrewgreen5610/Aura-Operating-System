using System;
using System.Collections.Generic;
using System.Text;

namespace Aura_OS
{
    public static class Akernel
    {
        static Cosmos.Core.IOPort io = new Cosmos.Core.IOPort(0);
        static int PP = 0, D = 0;
        public static void sendSignal(string process, int signal)
        {
        }
        public static void Outb(ushort port, byte data)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            io.Byte = data;
            PP = port;
            D = data;

        }
        public static void Outw(ushort port, ushort data)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            io.Word = data;
            PP = port;
            D = data;

        }
        public static byte Inb(ushort port)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            return io.Byte;

        }
        public static ushort Inw(ushort port)
        {
            if (io.Port != port)
                io = new Cosmos.Core.IOPort(port);
            return io.Word;

        }
    }
}
