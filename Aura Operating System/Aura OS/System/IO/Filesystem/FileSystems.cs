using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.HAL.BlockDevice;
using Aura_OS.IO;

namespace Aura_OS.HAL
{
    public class Memory
    {
        public static List<StorageDevice> devices = new List<StorageDevice>();
    }
    public abstract class StorageDevice
    {
        public char Label;
        public abstract string[] ListFiles(string dir);
        public abstract byte[] readFile(string name);
        public abstract void saveFile(byte[] data, string name, string owner);
        public abstract void makeDir(string name, string owner);
        public abstract string[] ListDirectories(string dir);
        public abstract string[] ListJustFiles(string dir);
        public abstract void Move(string s1, string s2);
        public abstract HAL.fsEntry[] getLongList(string dir);
        public abstract void Delete(string Path);
        public abstract void Chmod(string f, string perms);
        public abstract void Chown(string f, string perms);
        public string DriveLabel = "";
    }
    public class VirtualFile
    {
        public string name;
        public byte[] data;
    }
    public class FileSystem
    {
        public static RootFilesystem Root;
    }
    public class fsEntry
    {
        public string Name;
        public byte Attributes;
        public int Pointer;
        public int Length;
        public byte User = 6, Group = 4, Global = 4;
        public string Owner;
        public string Time;
        public int Checksum;
    }

    public class DevFS : StorageDevice
    {
        public override void Chmod(string f, string perms)
        {
            throw new NotImplementedException();
        }
        public override void Chown(string f, string perms)
        {
            throw new NotImplementedException();
        }
        public override void Delete(string Path)
        {
            throw new NotImplementedException();
        }
        public override fsEntry[] getLongList(string dir)
        {
            string[] str = ListFiles(dir);
            fsEntry[] ent = new fsEntry[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                ent[i].Name = str[i];
                ent[i].Owner = "sys";
                ent[i].Time = "07/10/1999"; //TODO!!!!
            }
            return ent;
        }
        public override string[] ListDirectories(string dir)
        {
            throw new NotImplementedException();
        }
        public override string[] ListJustFiles(string dir)
        {
            return ListFiles(dir);
        }
        public override void makeDir(string name, string owner)
        {
            throw new NotImplementedException();
        }
        public override byte[] readFile(string name)
        {
            throw new NotImplementedException();
        }
        public override void saveFile(byte[] data, string name, string owner)
        {
            throw new NotImplementedException();
        }
        public override void Move(string s1, string s2)
        {
            throw new NotImplementedException();
        }
        public override string[] ListFiles(string dir)
        {
            string[] ret = new string[Devices.dev.Count];
            for (int i = 0; i < Devices.dev.Count; i++)
            {
                ret[i] = Utils.Util.cleanName(Devices.dev[i].name).Substring(Utils.Util.IndexOf(Devices.dev[i].name, '/') + 1);
            }
            return ret;
        }
    }

    public class MountPoint
    {
        public string Path;
        public StorageDevice device;
    }
    public class RootFilesystem : StorageDevice
    {

        public List<MountPoint> Mountpoints = new List<MountPoint>();
        public void Mount(string dir, StorageDevice sd)
        {
            MountPoint mp = new MountPoint();
            dir = Utils.Util.cleanName(dir);
            mp.Path = dir;
            mp.device = sd;
            Mountpoints.Add(mp);
        }
        public bool isMountPoint(string dir)
        {
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Utils.Util.cleanName(Mountpoints[i].Path) == Utils.Util.cleanName(dir))
                    return true;
            }
            return false;
        }
        public void saveMtab()
        {
            //Kernel.Shell.PushPrivilages();
            string mtab = "";
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                mtab += "rw " + Mountpoints[i].Path + " ";
            }
        }
        public void Unmount(string device)
        {
            List<MountPoint> mounts = new List<MountPoint>();

            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Utils.Util.cleanName(Mountpoints[i].Path) != Utils.Util.cleanName(device))
                    mounts.Add(Mountpoints[i]);
            }
            Mountpoints = mounts;
        }
        public override void Chmod(string dir, string perms)
        {
            dir = Utils.Util.cleanName(dir);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path && Mountpoints[i].Path != "")
                    {
                        ((GLNFS)Mountpoints[i].device).Chmod(dir.Substring(Mountpoints[i].Path.Length), perms);
                        return;
                    }
                }

            }
            ((GLNFS)Mountpoints[0].device).Chmod(dir, perms);
        }
        public override void Chown(string dir, string perms)
        {
            dir = Utils.Util.cleanName(dir);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path && Mountpoints[i].Path != "")
                    {
                        ((GLNFS)Mountpoints[i].device).Chown(dir.Substring(Mountpoints[i].Path.Length), perms);
                        return;
                    }
                }

            }
            ((GLNFS)Mountpoints[0].device).Chown(dir, perms);
        }
        public override string[] ListJustFiles(string dir)
        {
            dir = Utils.Util.cleanName(dir);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path)
                    {
                        return Mountpoints[i].device.ListJustFiles(dir.Substring(Mountpoints[i].Path.Length));
                    }
                }

            }
            return Mountpoints[0].device.ListJustFiles(dir);
            throw new Exception("File not found!");
        }
        public override fsEntry[] getLongList(string dir)
        {

            //dir = Utils.Util.cleanName(dir);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path)
                    {
                        return Mountpoints[i].device.getLongList(dir.Substring(Mountpoints[i].Path.Length));
                    }
                }

            }
            return Mountpoints[0].device.getLongList(dir);
            throw new Exception("File not found!");
        }
        public override void Move(string dir, string dir2)
        {

            dir = Utils.Util.cleanName(dir);

            dir2 = Utils.Util.cleanName(dir2);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path)
                    {
                        Mountpoints[i].device.Move(dir.Substring(Mountpoints[i].Path.Length), dir2.Substring(Mountpoints[i].Path.Length));
                        return;
                    }
                }

            }
            Mountpoints[0].device.Move(dir, dir2);
            return;
        }
        public override void makeDir(string dir, string owner)
        {
            Console.WriteLine("STEP01");
            Console.ReadKey();
            dir = Utils.Util.cleanName(dir);
            Console.WriteLine("STEP02");
            Console.ReadKey();
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                Console.WriteLine("STEP03");
                Console.ReadKey();
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {
                    Console.WriteLine("STEP04");
                    Console.ReadKey();
                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path)
                    {
                        Console.WriteLine("STEP05");
                        Console.ReadKey();
                        Mountpoints[i].device.makeDir(dir.Substring(Mountpoints[i].Path.Length), owner);
                        Console.WriteLine("STEP06");
                        Console.ReadKey();
                        return;
                    }
                }
            }
            Console.WriteLine("STEP07");
            Console.ReadKey();
            Mountpoints[0].device.makeDir(dir, owner);
            Console.WriteLine("STEP08");
            Console.ReadKey();
            return;
        }
        public override byte[] readFile(string dir)
        {

            dir = Utils.Util.cleanName(dir);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path)
                    {
                        return Mountpoints[i].device.readFile(dir.Substring(Mountpoints[i].Path.Length));
                    }
                }
            }
            return Mountpoints[0].device.readFile(dir);
            throw new Exception("File not found!");
        }
        public override void saveFile(byte[] data, string dir, string owner)
        {

            dir = Utils.Util.cleanName(dir);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path)
                    {
                        Mountpoints[i].device.saveFile(data, dir.Substring(Mountpoints[i].Path.Length), owner);
                        return;
                    }
                }

            }
            Mountpoints[0].device.saveFile(data, dir, owner);
            return;
        }
        public override void Delete(string dir)
        {

            dir = Utils.Util.cleanName(dir);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path)
                    {
                        Mountpoints[i].device.Delete(dir.Substring(Mountpoints[i].Path.Length));
                        return;
                    }
                }

            }
            Mountpoints[0].device.Delete(dir);
            return;
        }
        public override string[] ListDirectories(string dir)
        {

            dir = Utils.Util.cleanName(dir);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path)
                    {
                        return Mountpoints[i].device.ListDirectories(dir.Substring(Mountpoints[i].Path.Length));
                    }
                }
                else if (Mountpoints[i].Path == "")
                {

                }
            }
            return Mountpoints[0].device.ListDirectories(dir);
            throw new Exception("File not found!");
        }
        public override string[] ListFiles(string dir)
        {

            dir = Utils.Util.cleanName(dir);
            for (int i = 0; i < Mountpoints.Count; i++)
            {
                if (Mountpoints[i].Path.Length <= dir.Length && Mountpoints[i].Path != "")
                {

                    if (dir.Substring(0, Mountpoints[i].Path.Length) == Mountpoints[i].Path)
                    {
                        return Mountpoints[i].device.ListFiles(dir.Substring(Mountpoints[i].Path.Length));
                    }
                }
                else if (Mountpoints[i].Path == "")
                {

                }
            }
            return Mountpoints[0].device.ListFiles(dir);
            throw new Exception("File not found!");
        }

    }

    public class GLNFS : StorageDevice
    {

        private Partition part = null;
        public int ID = 0;

        public override void makeDir(string name, string owner)
        {
            name = cleanName(name);
            if (!Contains(name, '/'))
                //if (Kernel.Shell.Privilages != 0 && DriveLabel == "GruntyOS")
                //    throw new Exception("Can not make files here");
                //else
                    createNode(name, 2, owner);
            else
                if (CanWrite(name.Substring(0, Utils.Util.LastIndexOf(name, '/'))))
                createNode(name, 2, owner);
            else
                throw new Exception("Access denied!");
        }
        public void Format(string VolumeLABEL)
        {

            byte[] test = new byte[512];
            MemoryStream ms = new MemoryStream(512);
            for (int i = 0; i < 512; i++)
            {

                test[i] = 0;
            }
            for (int ii = 0; ii < 100; ii++)
            {
                part.WriteBlock((ulong)ii, 1, test);
            }

            ms.Data = test;
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write("GFS SC");
            bw.Write(VolumeLABEL);
            bw.Write(4);
            bw.BaseStream.Close();
            part.WriteBlock(1, 1, bw.BaseStream.Data);


        }
        public GLNFS(Partition p)
        {
            part = p;
            byte[] start = new byte[512];
            part.ReadBlock(1, 1, start);
            BinaryReader br = new BinaryReader(new MemoryStream(512));
            br.BaseStream.Data = start;
            if (br.ReadString() != "GFS SC")
            {
                //  Terminal.WriteError("Invalid Filesystem!");
                return;
            }
            this.DriveLabel = br.ReadString();
        }
        public override void Delete(string Path)
        {
            Unlink(Path);
        }
        bool Contains(string Str, char c)
        {
            foreach (char ch in Str)
            {
                if (ch == c)
                    return true;
            }
            return false;
        }

        public static bool isGFS(Partition part)
        {
            byte[] start = new byte[512];
            part.ReadBlock(1, 1, start);
            BinaryReader br = new BinaryReader(new MemoryStream(512));
            br.BaseStream.Data = start;
            if (br.ReadString() == "GFS SC")
            {
                return true;
            }
            return false;
            br.ReadString();
        }
        public byte[] GetFile(int index)
        {
            return null;
        }
        byte[] ReadData(int loc, int count)
        {
            if (count < 512)
            {
                byte[] b = new byte[512];
                part.ReadBlock((ulong)loc, 1, b);
                byte[] ret = new byte[count];
                for (int i = 4; i < count; i++)
                {
                    ret[i] = b[i];
                }
                return ret;
            }
            else
            {
                int pos = 0;

                byte[] ret = new byte[count];
                int s = 4;
                while (pos < count)
                {
                    byte[] b = new byte[512];
                    part.ReadBlock((ulong)loc, 1, b);
                    for (int i = s; i < 512; i++)
                    {

                        ret[pos] = b[i];
                        if (pos == count)
                        {
                            return ret;
                        }
                        pos++;

                    }
                    s = 0;
                    loc++;
                }
            }
            Console.WriteLine("lawl wut?");
            return null;
        }
        private int IndexOf(string str, char c)
        {
            int i = 0;
            foreach (char ch in str)
            {
                if (ch == c)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        string sname = null;
        private int last_addr = 0;
        public int prevLoc = 0;
        private bool GetBit(byte b, int bitNumber)
        {
            int bit = b & (1 << bitNumber - 1);
            if (bit == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool CanWrite(string file)
        {
            return true;
        }
        public bool CanRead(string file)
        {
            return true;
        }
        public bool CanExecute(string file)
        {
            return true;
        }
        private void insertEntry(fsEntry ent, int Node_block)
        {
            byte[] root = new byte[1024];
            part.ReadBlock((ulong)Node_block, 2, root);
            BinaryReader br = new BinaryReader(new MemoryStream(1024));
            br.BaseStream.Data = root;
            int files = br.ReadInt32() + 1;

            for (int i = 0; i < files - 1; i++)
            {
                string fname = br.ReadString();
                int ptr = br.ReadInt32();
                int len = br.ReadInt32();
                byte attr = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string owner = br.ReadString();
                int Checksum = br.ReadInt32();
            }
            int np = br.BaseStream.Position;
            BinaryWriter bw = new BinaryWriter(new MemoryStream(1024));
            bw.BaseStream.Data = root;

            bw.Write(files);
            bw.BaseStream.Position = np;
            bw.Write(ent.Name);
            bw.Write(ent.Pointer);
            bw.Write(ent.Length);
            bw.BaseStream.Write((byte)ent.Attributes);
            bw.Write((byte)ent.User);
            bw.Write((byte)ent.Group);
            bw.Write((byte)ent.Global);
            bw.Write("07/10/1999"); //TODO!!!
            bw.Write(ent.Owner);
            bw.Write((int)ent.Checksum);
            bw.BaseStream.Close();
            part.WriteBlock((ulong)Node_block, 2, bw.BaseStream.Data);
        }
        public int getWriteAddress()
        {
            byte[] start = new byte[512];
            part.ReadBlock(1, 1, start);
            BinaryReader br = new BinaryReader(new MemoryStream(512));
            br.BaseStream.Data = start;
            br.ReadString();
            br.ReadString();
            return br.ReadInt32();
        }
        private void increaseWriteAddress(int amount)
        {
            byte[] start = new byte[512];
            part.ReadBlock(1, 1, start);
            BinaryReader br = new BinaryReader(new MemoryStream(512));
            br.BaseStream.Data = start;
            br.ReadString();
            br.ReadString();
            BinaryWriter bw = new BinaryWriter(br.BaseStream);
            bw.BaseStream.Position = br.BaseStream.Position;
            bw.Write(getWriteAddress() + amount);
            bw.BaseStream.Close();
            part.WriteBlock(1, 1, bw.BaseStream.Data);
        }
        public void setWriteAddress(int amount)
        {
            byte[] start = new byte[512];
            part.ReadBlock(1, 1, start);
            BinaryReader br = new BinaryReader(new MemoryStream(512));
            br.BaseStream.Data = start;
            br.ReadString();
            br.ReadString();
            BinaryWriter bw = new BinaryWriter(br.BaseStream);
            bw.BaseStream.Position = br.BaseStream.Position;
            bw.Write(amount);
            bw.BaseStream.Close();
            part.WriteBlock(1, 1, bw.BaseStream.Data);
        }
        private void createNode(string name, int Node_block, string owner)
        {

            name = cleanName(name);
            byte[] root = new byte[1024];
            part.ReadBlock((ulong)Node_block, 2, root);
            BinaryReader br = new BinaryReader(new MemoryStream(1024));
            br.BaseStream.Data = root;
            prevLoc = Node_block;
            int files = br.ReadInt32();
            string item = "";
            if (Contains(name, '/'))
            {
                item = name.Substring(0, IndexOf(name, '/'));
            }
            else
            {
                fsEntry dir = new fsEntry();
                dir.Name = name;
                dir.Attributes = 2;
                dir.Length = 2;
                dir.Pointer = getWriteAddress();
                byte[] nulls = new byte[1024];
                for (int i = 0; i < 1024; i++)
                {
                    nulls[i] = 0;
                }
                part.WriteBlock((ulong)getWriteAddress(), 2, nulls);
                dir.Owner = owner;
                increaseWriteAddress(2);
                insertEntry(dir, prevLoc);
                return;
            }
            for (int i = 0; i < files; i++)
            {
                string fname = br.ReadString();
                if (fname == name && !Contains(name, '/'))
                {
                    return;
                }

                int ptr = br.ReadInt32();
                int len = br.ReadInt32();
                if (item == fname)
                {
                    Node_block = ptr;
                    prevLoc = ptr;
                    createNode(name.Substring(IndexOf(name, '/') + 1), Node_block, owner);
                    return;
                }

                byte attr = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string Owner = br.ReadString();

                int Checksum = br.ReadInt32();
            }

        }
        int node_faddr;


        private int LastIndexOf(string This, char ch)
        {
            int ret = -1;
            int i = 0;
            foreach (char c in This)
            {
                if (c == ch)
                {
                    ret = i;
                }

                i++;
            }
            return ret;
        }
        private void Unlink(string item)
        {
            if (!CanWrite(item))
            {
                Console.WriteLine("Error: Access Denied!");
                return;
            }

            string Name = item.Substring(Utils.Util.LastIndexOf(item, '/') + 1);

            item = item.Substring(0, Utils.Util.LastIndexOf(item, '/'));
            cleanName(item);
            item = "/" + item;
            int address = 2;

            if (Utils.Util.Contains(item, '/'))
                address = getNodeAddress(item);

            byte[] root = new byte[1024];
            part.ReadBlock((ulong)(uint)address, 2, root);
            BinaryReader br = new BinaryReader(new MemoryStream(1024));
            BinaryWriter bw = new BinaryWriter(new MemoryStream(1024));
            br.BaseStream.Data = root;
            int fcount = br.ReadInt32();
            bool deleted = false;
            bw.Write(fcount - 1);
            for (int i = 0; i < fcount; i++)
            {
                string name = br.ReadString();
                int ptr = br.ReadInt32();
                int loc = br.ReadInt32();
                byte attr = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string owner = br.ReadString();
                int Checksum = br.ReadInt32();
                if (name == Name)
                {

                    deleted = true;
                }
                else
                {
                    bw.Write(name);
                    bw.Write(ptr);
                    bw.Write(loc);
                    bw.Write((byte)attr);
                    bw.Write(Read);
                    bw.Write(Write);
                    bw.Write(Execute);
                    bw.Write(time);
                    bw.Write(owner);

                    bw.Write(Checksum);
                }
            }
            if (!deleted)
                return;
            bw.BaseStream.Close();
            part.WriteBlock((ulong)address, 2, bw.BaseStream.Data);

        }
        public override void Chown(string item, string mod)
        {
            if (!CanWrite(item))
            {
                Console.WriteLine("Error: Access Denied!");
                return;
            }

            cleanName(item);
            string Name = item.Substring(Utils.Util.LastIndexOf(item, '/') + 1);
            item = item.Substring(0, Utils.Util.LastIndexOf(item, '/'));
            int address = getNodeAddress(item);
            byte[] root = new byte[1024];
            part.ReadBlock((ulong)address, 2, root);
            BinaryReader br = new BinaryReader(new MemoryStream(1024));
            BinaryWriter bw = new BinaryWriter(new MemoryStream(1024));
            br.BaseStream.Data = root;
            int fcount = br.ReadInt32();
            bw.Write(fcount);
            for (int i = 0; i < fcount; i++)
            {
                string name = br.ReadString();
                int ptr = br.ReadInt32();
                int loc = br.ReadInt32();
                byte attr = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string owner = br.ReadString();

                int Checksum = br.ReadInt32();
                if (name == Name)
                {
                    bw.Write(name);
                    bw.Write(ptr);
                    bw.Write(loc);
                    bw.Write((byte)attr);
                    bw.Write(Read);
                    bw.Write(Write);
                    bw.Write(Execute);
                    bw.Write(time);
                    bw.Write(mod);
                    bw.Write(Checksum);
                }
                else
                {
                    bw.Write(name);
                    bw.Write(ptr);
                    bw.Write(loc);
                    bw.Write((byte)attr);
                    bw.Write(Read);
                    bw.Write(Write);
                    bw.Write(Execute);
                    bw.Write(time);
                    bw.Write(owner);
                    bw.Write(Checksum);
                }
            }
            bw.BaseStream.Close();
            part.WriteBlock((ulong)address, 2, bw.BaseStream.Data);

        }
        public override void Chmod(string item, string mod)
        {
            if (!CanWrite(item))
            {
                Console.WriteLine("Error: Access Denied!");
                return;
            }
            byte user, group, global;
            user = (byte)Conversions.StringToInt(mod.Substring(0, 1));
            group = (byte)Conversions.StringToInt(mod.Substring(1, 1));
            global = (byte)Conversions.StringToInt(mod.Substring(2, 1));
            cleanName(item);
            string Name = item.Substring(Utils.Util.LastIndexOf(item, '/') + 1);
            item = item.Substring(0, Utils.Util.LastIndexOf(item, '/'));
            int address = getNodeAddress(item);
            byte[] root = new byte[1024];
            part.ReadBlock((ulong)address, 2, root);
            BinaryReader br = new BinaryReader(new MemoryStream(1024));
            BinaryWriter bw = new BinaryWriter(new MemoryStream(1024));
            br.BaseStream.Data = root;
            int fcount = br.ReadInt32();
            bw.Write(fcount);
            for (int i = 0; i < fcount; i++)
            {
                string name = br.ReadString();
                int ptr = br.ReadInt32();
                int loc = br.ReadInt32();
                byte attr = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string owner = br.ReadString();

                int Checksum = br.ReadInt32();
                if (name == Name)
                {
                    bw.Write(name);
                    bw.Write(ptr);
                    bw.Write(loc);
                    bw.Write((byte)attr);
                    bw.Write(user);
                    bw.Write(group);
                    bw.Write(global);
                    bw.Write(time);
                    bw.Write(owner);
                    bw.Write(Checksum);
                }
                else
                {
                    bw.Write(name);
                    bw.Write(ptr);
                    bw.Write(loc);
                    bw.Write((byte)attr);
                    bw.Write(Read);
                    bw.Write(Write);
                    bw.Write(Execute);
                    bw.Write(time);
                    bw.Write(owner);
                    bw.Write(Checksum);
                }
            }
            bw.BaseStream.Close();
            part.WriteBlock((ulong)address, 2, bw.BaseStream.Data);

        }
        public override fsEntry[] getLongList(string dir)
        {
            if (dir != "" && dir != "/" && !CanRead(dir))
                throw new Exception("Access Denied!");
            if (dir == "/")
                dir = "";
            string[] files = ListFiles(dir);

            dir = cleanName(dir);
            fsEntry[] ret = new fsEntry[files.Length];
            for (int i = 0; i < files.Length; i++)
            {

                ret[i] = readFromNode(Utils.Util.cleanName(dir + "/" + files[i]));
            }
            return ret;
        }
        public override void Move(string f, string dest)
        {
            fsEntry fs = readFromNode(f);
            Unlink(f);
            fs.Name = dest.Substring(Utils.Util.LastIndexOf(dest, '/') + 1);
            int fptr = 0;
            if (Utils.Util.Contains(dest, '/'))
                fptr = getNodeAddress(dest.Substring(0, LastIndexOf(dest, '/')), 2);
            else
                fptr = getNodeAddress(f.Substring(0, LastIndexOf(f, '/')), 2);
            insertEntry(fs, fptr);
        }
        public fsEntry readFromNode(string name)
        {
            name = cleanName(name);
            int addr = 2;
            if (Contains(name, '/'))
            {
                addr = getNodeAddress(name.Substring(0, LastIndexOf(name, '/')));
            }
            else
            {
                addr = 2;
            }
            byte[] root = new byte[1024];
            part.ReadBlock((ulong)addr, 2, root);
            BinaryReader br = new BinaryReader(new MemoryStream(1024));
            br.BaseStream.Data = root;
            int files = br.ReadInt32();
            for (int i = 0; i < files; i++)
            {
                string fname = br.ReadString();
                int ptr = br.ReadInt32();
                int len = br.ReadInt32();
                byte attr = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string owner = br.ReadString();
                int Checksum = br.ReadInt32();
                if (fname == name.Substring(LastIndexOf(name, '/') + 1) || fname == name)
                {
                    fsEntry ent = new fsEntry();
                    ent.Name = fname;
                    ent.Attributes = attr;
                    ent.Length = len;
                    ent.Pointer = ptr;
                    ent.User = Read;
                    ent.Group = Write;
                    ent.Global = Execute;
                    ent.Owner = owner;
                    ent.Time = time;
                    ent.Checksum = Checksum;
                    return ent;
                }
            }
            throw new Exception("File not found!!!");
        }
        public int getNodeAddress(string name, int Node_block = 2)
        {

            name = cleanName(name);
            if (name == "" || name == null)
            {
                return 2;
            }
            if (Node_block == 0) { Node_block = 2; };
            string item = "";
            byte[] root = new byte[1024];
            part.ReadBlock((ulong)Node_block, 2, root);
            BinaryReader br = new BinaryReader(new MemoryStream(1024));
            br.BaseStream.Data = root;
            br.BaseStream.Position = 0;
            int files = br.ReadInt32();
            if (Contains(name, '/'))
            {
                item = name.Substring(0, IndexOf(name, '/'));
            }
            for (int i = 0; i < files; i++)
            {
                string fname = br.ReadString();
                int ptr = br.ReadInt32();
                int len = br.ReadInt32();
                if (name.Substring(IndexOf(name, '/') + 1) == fname && !Contains(name, '/'))
                {
                    return (int)ptr;
                }
                else if (item == fname && Contains(name, '/'))
                {
                    Node_block = ptr;
                    prevLoc = ptr;
                    name = name.Substring(IndexOf(name, '/') + 1);
                    return getNodeAddress(name, ptr);
                }

                byte attr = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string owner = br.ReadString();
                int Checksum = br.ReadInt32();
            }
            throw new Exception("File not found :(");
        }
        public override byte[] readFile(string name)
        {
            if (!CanRead(name))
                throw new Exception("Access Denied!");
            name = cleanName(name);
            fsEntry f = readFromNode(name);
            byte[] dat = new byte[f.Length * 512];
            part.ReadBlock((ulong)f.Pointer, (uint)f.Length, dat);

            MemoryStream ms = new MemoryStream(dat.Length);
            ms.Data = dat;
            BinaryReader br = new BinaryReader(ms);
            int flen = br.ReadInt32();
            byte[] real = new byte[flen];
            for (int i = 0; i < flen; i++)
            {
                real[i] = br.BaseStream.Read();
            }
            int CheckSum = System.Security.Crypto.getCRC(real);
            if (CheckSum != f.Checksum)
            {
                return readFile(name);
            }
            return real;
            return null;

        }
        public override string[] ListJustFiles(string dir)
        {
            if (dir != "" && dir != "/" && !CanRead(dir))
                throw new Exception("Access Denied!");
            dir = cleanName(dir);
            List<string> ret = new List<string>();
            byte[] FileTable = new byte[1024];
            part.ReadBlock((ulong)getNodeAddress(dir), 2, FileTable);
            MemoryStream ms = new MemoryStream(1024);
            ms.Data = FileTable;
            BinaryReader br = new BinaryReader(ms);
            int Files = br.ReadInt32();
            for (int i = 0; i < Files; i++)
            {
                string fname = br.ReadString();
                int faddr = br.ReadInt32();
                int flen = br.ReadInt32();
                byte Attributes = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string owner = br.ReadString();
                int Checksum = br.ReadInt32();
                if (Attributes != 2)
                    ret.Add(fname);
            }
            return ret.ToArray();
        }
        public override string[] ListDirectories(string dir)
        {
            if (dir != "" && dir != "/" && !CanRead(dir))
                throw new Exception("Access Denied!");
            dir = cleanName(dir);
            List<string> ret = new List<string>();
            byte[] FileTable = new byte[1024];
            part.ReadBlock((ulong)getNodeAddress(dir), 2, FileTable);
            MemoryStream ms = new MemoryStream(1024);
            ms.Data = FileTable;
            BinaryReader br = new BinaryReader(ms);
            int Files = br.ReadInt32();
            for (int i = 0; i < Files; i++)
            {
                string fname = br.ReadString();
                int faddr = br.ReadInt32();
                int flen = br.ReadInt32();
                byte Attributes = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string owner = br.ReadString();
                int Checksum = br.ReadInt32();
                if (Attributes == 2)
                    ret.Add(fname);
            }
            return ret.ToArray();
        }
        public override string[] ListFiles(string dir)
        {
            if (dir != "" && dir != "/" && !CanRead(dir))
                throw new Exception("Access Denied!");

            List<string> ret = new List<string>();
            byte[] FileTable = new byte[1024];
            part.ReadBlock((ulong)(uint)getNodeAddress(dir), 2, FileTable);
            dir = cleanName(dir);
            MemoryStream ms = new MemoryStream(1024);
            ms.Data = FileTable;
            BinaryReader br = new BinaryReader(ms);
            int Files = br.ReadInt32();
            for (int i = 0; i < Files; i++)
            {
                string fname = br.ReadString();
                int faddr = br.ReadInt32();
                int flen = br.ReadInt32();
                byte Attributes = br.BaseStream.Read();
                byte Read = br.BaseStream.Read();
                byte Write = br.BaseStream.Read();
                byte Execute = br.BaseStream.Read();
                string time = br.ReadString();
                string owner = br.ReadString();
                int Checksum = br.ReadInt32();
                ret.Add(fname);
            }
            return ret.ToArray();
        }
        private string cleanName(string name)
        {
            if (name.Substring(0, 1) == "/")
            {
                name = name.Substring(1, name.Length - 1);
            }
            if (name.Substring(name.Length - 1, 1) == "/")
            {
                name = name.Substring(0, name.Length - 1);
            }
            return name;
        }
        public override void saveFile(byte[] data, string name, string owner)
        {

            name = cleanName(name);
            if (Utils.Util.Contains(name, '/'))
            {
                if (!CanWrite(name.Substring(0, Utils.Util.LastIndexOf(name, '/'))))
                {
                    throw new Exception("Access denied");
                    return;
                }
            }
            fsEntry fs = new fsEntry();

            fs.Checksum = System.Security.Crypto.getCRC(data);
            fs.Attributes = 1;
            byte[] New = new byte[data.Length + 4];
            MemoryStream ms = new MemoryStream(New.Length);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                bw.BaseStream.Write(data[i]);
            }
            bw.BaseStream.Close();
            data = bw.BaseStream.Data;
            fs.Name = name;
            int p = 0;
            while (p < data.Length)
            {
                byte[] newf = new byte[512];
                for (int i = 0; i < 512; i++)
                {
                }
                p++;
            }
            fs.Length = p;
            fs.Pointer = getWriteAddress();
            fs.Attributes = 1;
            fs.Owner = owner;
            int fptr = 2;
            if (Utils.Util.Contains(fs.Name, '/'))
                fptr = getNodeAddress(fs.Name.Substring(0, LastIndexOf(fs.Name, '/')), 2);
            if (Contains(name, '/'))
            {
                fs.Name = name.Substring(LastIndexOf(name, '/') + 1);
            }

            string[] files = ListFiles(name.Substring(0, LastIndexOf(name, '/')));

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] == name.Substring(Utils.Util.LastIndexOf(name, '/') + 1))
                {
                    if (fs.Length * 512 <= p)
                    {
                        fs = readFromNode(name);

                        Unlink(name);
                        if (data.Length < 512)
                        {
                            ms = new MemoryStream(512);
                            for (int ii = 0; ii < data.Length; ii++)
                            {
                                ms.Write(data[ii]);
                            }
                            part.WriteBlock((ulong)fs.Pointer, 1, ms.Data);
                            insertEntry(fs, fptr);

                            return;
                        }
                        int post = 0;
                        int lastpos = fs.Pointer;

                        while (post < data.Length)
                        {
                            byte[] newf = new byte[512];
                            for (int ii = 0; ii < 512; ii++)
                            {
                                newf[ii] = data[post];
                                post++;
                            }
                            part.WriteBlock((ulong)lastpos, 1, newf);
                            lastpos++;
                        }

                        insertEntry(fs, fptr);
                        return;
                    }
                    else
                    {
                        fs.Owner = readFromNode(name).Owner;
                        fs.Group = readFromNode(name).Group;
                        fs.Global = readFromNode(name).Global;
                        fs.User = readFromNode(name).User;
                        Unlink(name);
                        break;
                    }

                }

            }
            if (data.Length < 512)
            {
                ms = new MemoryStream(512);
                for (int i = 0; i < data.Length; i++)
                {
                    ms.Write(data[i]);
                }
                part.WriteBlock((ulong)fs.Pointer, 1, ms.Data);
                insertEntry(fs, fptr);
                increaseWriteAddress(p);
                return;
            }
            int pos = 0;
            int lastp = getWriteAddress();

            increaseWriteAddress(p);
            while (pos < data.Length)
            {
                byte[] newf = new byte[512];
                for (int i = 0; i < 512; i++)
                {
                    newf[i] = data[pos];
                    pos++;
                }
                part.WriteBlock((ulong)lastp, 1, newf);
                lastp++;
            }

            insertEntry(fs, fptr);
        }
    }

}
