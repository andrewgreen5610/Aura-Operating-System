using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.HAL.BlockDevice;
using Aura_OS.IO;

namespace Aura_OS.HAL
{
    class ATA : Driver
    {
        private class MBREntry
        {
            public byte Boot;
            public byte StartingHead;
            public byte StartingSector;
            public ushort StartingCylider;
            public byte SystemID;
            public byte EndingHead;
            public byte EndingSector;
        }
        private MemoryStream MBR = new MemoryStream(1024);
        private char HDlabel = 'a';
        public void ReadMBR(AtaPio dev)
        {
            BinaryReader br = new BinaryReader(MBR);
            for (int i = 0; i < 4;)
            {
                br.BaseStream.Position = 446 + (i * 16) + 8;
                uint relativeSector = br.ReadUInt32();
                uint sectorCount = br.ReadUInt32();
                if ( sectorCount != 0)
                {
                    Partition p = new Partition(dev, relativeSector, sectorCount);
                    Devices.device d = new Devices.device();
                    d.dev = p;
                    d.name = "/dev/sd" + HDlabel.ToString() + (i + 1).ToString();
                    Console.WriteLine("Device " + d.name + " detected");
                    Devices.dev.Add(d);
                }
                i++;
            }
        }
        public override bool Init()
        {
            this.Name = "ATA subsystem";
            Console.WriteLine("Initilizing ATA drivers and devices.");
            for (int i = 0; i < BlockDevice.Devices.Count; i++)
            {
                if (BlockDevice.Devices[i] is AtaPio)
                {
                    AtaPio xAta = (AtaPio)BlockDevice.Devices[i];
                    xAta.ReadBlock(0, 2, MBR.Data);
                    ReadMBR(xAta);
                    Devices.device dev = new Devices.device();
                    dev.name = "/dev/sd" + HDlabel.ToString();
                    dev.dev = xAta;
                    Devices.dev.Add(dev);
                    HDlabel++;
                }
            }
            return true;
        }
    }
}