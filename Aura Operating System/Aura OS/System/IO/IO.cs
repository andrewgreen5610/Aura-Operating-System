﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aura_OS.IO
{

    public class TextReader
    {
        public int pos = 0;
        private char[] dat;
        public int Length;
        public char Read()
        {
            pos++;
            return dat[pos - 1];
        }
        public char Peek()
        {
            if (pos < dat.Length)
            {
                return dat[pos];
            }
            else
            {
                return (char)255;
            }
        }
        public char Peek(int depth)
        {
            if (pos < dat.Length)
            {
                return dat[pos + depth];
            }
            else
            {
                return (char)255;
            }
        }
        public TextReader(string str)
        {
            dat = str.ToCharArray();
            Length = dat.Length;
        }
    }
    public class BinaryReader
    {
        public ioStream BaseStream;
        public BinaryReader(ioStream stream)
        {
            stream.Position = 0;
            this.BaseStream = stream;
        }
        public byte ReadByte()
        {
            BaseStream.Position++;
            return BaseStream.Data[BaseStream.Position - 1];
        }
        public int ReadInt32()
        {
            int val = BitConverter.ToInt32(BaseStream.Data, (int)BaseStream.Position);
            BaseStream.Position += 4;
            return val;
        }
        public uint ReadUInt32()
        {
            uint val = BitConverter.ToUInt32(BaseStream.Data, (int)BaseStream.Position);
            BaseStream.Position += 4;
            return val;
        }
        public string ReadAllText()
        {
            string text = "";
            while (BaseStream.Position < BaseStream.Data.Length)
                text += ((char)BaseStream.Read()).ToString();
            return text;
        }
        public void Close()
        {
            BaseStream.Close();
        }
        public string ReadString()
        {
            byte length = BaseStream.Read();
            string Ret = "";
            for (int i = 0; i < length; i++)
            {
                Ret += ((char)BaseStream.Read()).ToString();
            }
            return Ret;
        }
    }

    class BinaryWriter
    {
        public ioStream BaseStream;
        public void Write(byte data)
        {
            BaseStream.Write(data);
        }
        public void Write(char data)
        {
            BaseStream.Write((byte)data);
        }
        public void WriteBytes(string str)
        {
            for (int i = 0; i < str.Length; i++)
                Write((byte)str[i]);
        }
        public void Write(int data)
        {
            byte[] bits = BitConverter.GetBytes(data);
            foreach (byte b in bits)
            {
                BaseStream.Write(b);
            }
        }
        public void Write(uint data)
        {
            byte[] bits = BitConverter.GetBytes(data);
            foreach (byte b in bits)
            {
                BaseStream.Write(b);
            }
        }
        public void Write(short data)
        {
            byte[] bits = BitConverter.GetBytes(data);
            foreach (byte b in bits)
            {
                BaseStream.Write(b);
            }
        }
        public void Write(ushort data)
        {
            byte[] bits = BitConverter.GetBytes(data);
            foreach (byte b in bits)
            {
                BaseStream.Write(b);
            }
        }
        public void Write(byte[] data)
        {
            foreach (byte b in data)
            {
                BaseStream.Write(b);
            }
        }
        public void Write(string data)
        {
            BaseStream.Write((byte)data.Length);
            foreach (byte b in data)
            {
                BaseStream.Write(b);
            }
        }

        public BinaryWriter(ioStream file)
        {

            BaseStream = file;
        }
    }
}