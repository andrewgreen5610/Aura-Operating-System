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

    public class FileStream : ioStream
    {
        private string fname = "";
        string fmode = "";
        public FileStream(string fname, string mode)
        {
            this.fname = fname;
            this.init(7000);
            fmode = mode;
            if (mode == "r")
            {
                this.init(HAL.FileSystem.Root.readFile(fname).Length);
                this.Data = HAL.FileSystem.Root.readFile(fname);
                return;
            }
        }

        public override void Flush()
        {
            base.Flush();
        }
        public override void Write(byte i)
        {
            base.Write(i);
        }
        public override byte Read()
        {
            return base.Read();
        }
        public override void Close()
        {
            if (fmode == "w")
            {
                MemoryStream ms = new MemoryStream(this.Position);

                for (int i = 0; i < this.Position; i++)
                {
                    ms.Write(this.Data[i]);
                }
                this.Data = ms.Data;
                HAL.FileSystem.Root.saveFile(this.Data, fname, "user");

            }


        }
    }
}
