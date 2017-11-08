using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aura_OS.HAL
{
    public abstract class Driver
    {
        public string Name;
        public void outb(ushort port, byte data)
        {
            Akernel.Outb(port, data);
        }
        public byte inb(ushort port)
        {
            return Akernel.Inb(port);
        }
        public ushort inw(ushort port)
        {
            return Akernel.Inw(port);
        }
        public void irq_wait(ushort IRQ)
        {
            Cosmos.Core.Global.CPU.Halt();
        }
        public void outw(ushort port, ushort dat)
        {
            Akernel.Outw(port, dat);
        }
        public Driver()
        {
            Akernel.Drivers.Add(this);
        }
        public abstract bool Init();
    }
}
