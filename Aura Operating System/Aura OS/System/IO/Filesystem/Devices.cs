using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.HAL.BlockDevice;

namespace Aura_OS.HAL
{
    public class Devices
    {
        public class device
        {
            public string name;
            public BlockDevice dev;
        }
        public static List<device> dev = new List<device>();
        public static BlockDevice getDevice(string name)
        {
            for (int i = 0; i < dev.Count; i++)
            {
                if (dev[i].name == name)
                    return dev[i].dev;
            }
            Console.WriteLine("Error device not found!");
            return null;
        }

    }
}
