using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aura_OS.IO
{
    public abstract class ioStream
    {
        public int Position;
        public byte[] Data;

        public virtual void Write(byte i)
        {
            if (Data.Length + 1 < Position)
            {
                byte[] newBuff = new byte[Data.Length + 1000];
                for (int p = 0; p < Data.Length; p++)
                {
                    newBuff[p] = i;
                }
                Data = newBuff;
            }
            Data[Position] = i;
            Position++;
        }
        public virtual byte Read()
        {
            Position++;
            return Data[Position - 1];
        }
        public virtual void Flush()
        {
            Data = null;
            Position = 0;
        }
        public virtual void Close()
        {

        }
        bool Resize = true;
        public void init(int size)
        {
            Resize = false;
            this.Data = new byte[size];
        }

    }
    public unsafe class MemoryStream : ioStream
    {
        public override void Close()
        {
            // this.Flush();
        }
        private byte* pointer = null;
        public override void Write(byte i)
        {
            if (pointer == null)
                base.Write(i);
            else
            {
                pointer[Position] = i;
                Position++;
            }
        }
        public override byte Read()
        {
            if (pointer == null)
                return base.Read();
            else
            {
                Position++;
                return pointer[Position - 1];
            }
        }
        public MemoryStream(int size)
        {
            pointer = null;
            this.init(size);
        }
        public MemoryStream(byte[] dat)
        {
            pointer = null;
            this.Data = dat;
        }
        public MemoryStream(byte* ptr)
        {
            pointer = ptr;
        }
    }
}
