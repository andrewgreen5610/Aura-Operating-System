/*
* PROJECT:          Aura Operating System Development
* CONTENT:          Command Interpreter - CommandManager
* PROGRAMMER(S):    John Welsh <djlw78@gmail.com>
*/

using Aura_OS.Apps.User;
using System;
using Cosmos.HAL.BlockDevice;
using System.Collections.Generic;
using Aura_OS.HAL;
using Aura_OS.IO;

namespace Aura_OS.Shell.cmdIntr
{
    class CommandManager
    {
        //TO-DO: Do for all commands:
        //       Windows like command, Linux like command, Aura original command (optional for the last one)
        //Example: else if ((cmd.Equals("ipconfig")) || (cmd.Equals("ifconfig")) || (cmd.Equals("netconf"))) {

        /// <summary>
        /// Empty constructor. (Good for debug)
        /// </summary>
        public CommandManager() { }
        /// <summary>
        /// Shell Interpreter
        /// </summary>
        /// <param name="cmd">Command</param>
        public static void _CommandManger(string cmd)
        {

        #region Power

            if (cmd.Equals("shutdown"))
            {//NOTE: Why isn't it just the constructor? This leaves more room for <package>.<class>.HelpInfo;
                Power.Shutdown.c_Shutdown();
            }
            else if (cmd.Equals("reboot"))
            {
                Power.Reboot.c_Reboot();
            }

        #endregion Power

        #region Console

            else if ((cmd.Equals("clear")) || (cmd.Equals("cls")))
            {
                c_Console.Clear.c_Clear();
            }
            else if (cmd.StartsWith("echo "))
            {
                c_Console.Echo.c_Echo(cmd);
            }
            else if (cmd.Equals("help"))
            {
                System.Translation.Help._Help();
            }

        #endregion Console

        #region FileSystem

            else if (cmd.StartsWith("cd "))
            {
                FileSystem.CD.c_CD(cmd);
            }
            else if (cmd.Equals("cp"))
            {
                FileSystem.CP.c_CP();
            }
            else if (cmd.StartsWith("cp "))
            {
                FileSystem.CP.c_CP();
            }
            else if ((cmd.Equals("dir")) || (cmd.Equals("ls")))
            {
                FileSystem.Dir.c_Dir();
            }
            else if ((cmd.StartsWith("dir ")) || (cmd.StartsWith("ls ")))
            {
                FileSystem.Dir.c_Dir(cmd);
            }
            else if (cmd.Equals("mkdir"))
            {
                FileSystem.Mkdir.c_Mkdir();
            }
            else if (cmd.StartsWith("mkdir "))
            {
                FileSystem.Mkdir.c_Mkdir(cmd);
            }
            else if (cmd.StartsWith("rmdir "))
            {
                FileSystem.Rmdir.c_Rmdir(cmd);
            }//TODO: orgainize
            else if (cmd.StartsWith("rmfil "))
            {
                FileSystem.Rmfil.c_Rmfil(cmd);
            }
            else if (cmd.Equals("mkfil"))
            {
                FileSystem.Mkfil.c_mkfil();
            }
            else if (cmd.StartsWith("mkfil "))
            {
                FileSystem.Mkfil.c_mkfil(cmd);
            }
            else if (cmd.StartsWith("edit "))
            {
                FileSystem.Edit.c_Edit(cmd);
            }
            else if (cmd.Equals("vol"))
            {
                FileSystem.Vol.c_Vol();
            }
            else if (cmd.StartsWith("run "))
            {
                FileSystem.Run.c_Run(cmd);
            }

        #endregion FileSystem

        #region Settings

            else if (cmd.Equals("logout"))
            {
                Settings.Logout.c_Logout();
            }
            else if (cmd.Equals("settings"))
            {
                Settings.Settings.c_Settings();
            }
            else if (cmd.StartsWith("settings "))
            {
                Settings.Settings.c_Settings(cmd);
            }

        #endregion Settings

        #region System Infomation

            else if (cmd.Equals("systeminfo"))
            {
                SystemInfomation.SystemInfomation.c_SystemInfomation();
            }
            else if ((cmd.Equals("ver")) || (cmd.Equals("version")))
            {
                SystemInfomation.Version.c_Version();
            }
            else if ((cmd.Equals("ipconfig")) || (cmd.Equals("ifconfig")) || (cmd.Equals("netconf")))
            {
                SystemInfomation.IPConfig.c_IPConfig();
            }
            else if ((cmd.Equals("time")) || (cmd.Equals("date")))
            {
                SystemInfomation.Time.c_Time();
            }

            #endregion System Infomation

        #region Tests

            else if (cmd.Equals("crash"))
            {
                Tests.Crash.c_Crash();
            }

            else if (cmd.Equals("fs"))
            {
                HAL.ATA ata = new HAL.ATA();
                Akernel.Init();

                HAL.FileSystem.Root = new HAL.RootFilesystem();
                HAL.GLNFS fd;

                bool installation_detected = false;

                for (int i = 0; i < HAL.Devices.dev.Count; i++)
                {
                    if (HAL.Devices.dev[i].dev is Partition)
                    {
                        if (HAL.GLNFS.isGFS((Partition)HAL.Devices.dev[i].dev))
                        {
                            fd = new HAL.GLNFS((Partition)HAL.Devices.dev[i].dev);
                            if (fd.DriveLabel == "GruntyOS")
                            {
                                Console.WriteLine("Installation found, mounted partition (" + HAL.Devices.dev[i].name + ")");
                                installation_detected = true;
                                break;
                            }
                        }
                    }
                }
                if (!installation_detected)
                    Console.WriteLine("Installation not detected!");
                Console.ReadKey();
                Setup.Init();
            }

            #endregion Tests

            #region Tools

            else if (cmd.Equals("snake"))
            {
                Tools.Snake.c_Snake();
            }
            else if (cmd.StartsWith("md5"))
            {
                Tools.MD5.c_MD5(cmd);
            }

            #endregion

            #region Util           

            else
            {
                Util.CmdNotFound.c_CmdNotFound();
            }

        #endregion Util

        }


    }

    class Setup
    {

        public static string CurrentDirectory;

        public static void Init()
        {

            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.CursorTop = 0;
            bool trial = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("GruntyOS Setup Wizard - Step 1: Pick a partition.                               ");
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            List<string> parts = new List<string>();
            List<Partition> partitions = new List<Partition>();
            int d = 1;
            //Kernel.Shell.User = "sys";
            CurrentDirectory = "/";

            for (int i = 0; i < BlockDevice.Devices.Count; i++)
            {
                BlockDevice device = BlockDevice.Devices[i];
                if (device is Partition)
                {
                    parts.Add("/dev/sda" + d.ToString());
                    partitions.Add((Partition)device);
                    d++;
                }
            }
            RAMDisk rd = new RAMDisk(400);
            Partition p = new Partition(rd, 0, 400);
            Devices.device dev = new Devices.device();
            dev.name = "/dev/rdz";
            dev.dev = p;
            Devices.dev.Add(dev);
            partitions.Add(p);

            parts.Add("Try GruntyOS");
            parts.Add("Create/Edit Partitions");
            fill(ConsoleColor.Gray, 15, 40, 4, 13, true);
            Console.CursorLeft = 16;
            Console.CursorTop = 5;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Please choose a partition");
            //uint selected = Menu(parts.ToArray(), 16, 6);
            //fill(ConsoleColor.Gray, 15, 40, 4, 13, true);

            foreach (string part in parts)
            {
                Console.WriteLine(part + " " + parts.Count);
            }

            Console.Write("Select partition: ");
            string partiti = Console.ReadLine();
            int selected = Int32.Parse(partiti);
            if (selected == parts.Count - 1)
            {
                Console.WriteLine("FORMAT");
                Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.Clear();
                commandes.Format("");
                Cosmos.System.Power.Reboot();
            }
            else if (selected == parts.Count - 2)
            {
                Console.WriteLine("TRIAL");
                Console.ReadKey();
                parts[(int)selected] = "/dev/rdz";
                trial = true;
            }

            Console.WriteLine("OTHER?");
            Console.ReadKey();

            create_acc:
            Console.CursorTop = 0;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("GruntyOS Setup Wizard - Step 2: Create an account                               ");

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.CursorLeft = 16;
            Console.CursorTop = 5;
            Console.WriteLine("Please create your GruntyOS account");
            Console.CursorLeft = 16;
            Console.CursorTop = 8;
            Console.WriteLine("Password:");
            Console.CursorTop = 7;
            Console.CursorLeft = 16;
            Console.Write("Username:");
            string User = Console.ReadLine();
            Console.CursorLeft = 16;
            Console.Write("Password:");
            string Pass = Console.ReadLine();
            //GLNFS fd = new GLNFS(partitions.ToArray()[selected]); TODO FIX THAT
            GLNFS fd = new GLNFS(partitions.ToArray()[0]);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.CursorTop = 0;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("GruntyOS Setup Wizard - Step 3: Installing Grunty OS                            ");

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            doagain:
            try
            {
                Console.WriteLine("STEP1");
                Console.ReadKey();

                Console.WriteLine("Formating selected partition to GLNFS..");
                commandes.Umount("-a");

                Console.WriteLine("STEP2");
                Console.ReadKey();

                string[] args = new string[]
        {
            parts[0],
            "GruntyOS"
        };

                Console.WriteLine("STEP3");
                Console.ReadKey();

                commandes.Mkfs(args);

                Console.WriteLine("STEP4");
                Console.ReadKey();

                string[] arg2 = new string[]
                {
                     parts[0]
                };

                Console.WriteLine("STEP5");
                Console.ReadKey();

                commandes.Mount(arg2);

                Console.WriteLine("STEP6");
                Console.ReadKey();

                // File.Chmod("/etc/passwd", "600");

                HAL.FileSystem.Root.makeDir(getFullPath("etc"), "sys");


                Console.WriteLine("STEP12");
                Console.ReadKey();

                File.Save("/etc/motd", @"Welcome to Grunty OS v 3.0.1 codenamed 'Infinity' Beta!


*  For full documentation visit:

   http://infinity.gruntxproductions.net/wiki/


*  More documentation can be found in /usr/doc
   


You can change this message by modifiying /etc/motd

");


                Console.WriteLine("STEP13");
                Console.ReadKey();

                Console.WriteLine(parts[0]);

                Console.WriteLine("STEP14");
                Console.ReadKey();
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                Console.WriteLine("FAILURE: Retrying.... (This might happen randomly due to unknown bug. Just reboot and I recommend VMWare)");
                Console.ReadLine();
                goto doagain;
            }
            Console.ReadLine();
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            ////Cosmos.Core.Global.CPU.Reboot();
            //if (!trial)
            //{
            //    Console.WriteLine("Setup complete");
            //    Console.WriteLine("Please reboot...");
            //   Cosmos.System.Power.Reboot();
            //}

            LS();
            Console.ReadKey();
            Cosmos.System.Power.Reboot();

        }

        public static void LS()
        {
            int p = 0;
            ConsoleColor fg = Console.ForegroundColor;
            HAL.fsEntry[] ent = HAL.FileSystem.Root.getLongList(CurrentDirectory);

            int longest_length = 0;
            int longest_length2 = 0;
            int longest_length3 = 0;

            for (int i = 0; i < ent.Length; i++)
            {
                if (ent[i].Name.Length > longest_length)
                    longest_length = ent[i].Name.Length;
                if (ent[i].Owner.Length > longest_length2)
                    longest_length2 = ent[i].Owner.Length;
                if (ent[i].Time.Length > longest_length3)
                    longest_length3 = ent[i].Time.Length;
            }
            longest_length = longest_length + 2;
            longest_length2 += 2;
            longest_length3 += 2;

                    HAL.fsEntry[] ents = HAL.FileSystem.Root.getLongList(CurrentDirectory);

                    for (int i = 0; i < ents.Length; i++)
                    {
                        Console.Write(ents[i].Name);
                        Console.CursorLeft = longest_length;
                        Console.Write(ents[i].Owner);
                        Console.CursorLeft = longest_length + longest_length2;
                        Console.Write(ents[i].Time);
                        Console.CursorLeft = longest_length + longest_length2 + longest_length3;
                        Console.WriteLine(ents[i].User.ToString() + ents[i].Group.ToString() + ents[i].Global.ToString());
                    }
                   
        }


        public static string getFullPath(string s)
        {

            //return "/" + Utils.Util.cleanName(CurrentDirectory) + "/" + Utils.Util.cleanName(s);
            return "/" + CurrentDirectory + "/" + s;
        }

        public static void fill(ConsoleColor c, int x, int width, int y, int height, bool border = false)
        {
            ConsoleColor prevColor = Console.BackgroundColor;

            Console.BackgroundColor = c;
            int left = Console.CursorLeft, top = Console.CursorTop;
            for (int X = 0; X < width; X++)
            {
                for (int Y = 0; Y < height; Y++)
                {
                    Console.CursorLeft = X + x;
                    Console.CursorTop = Y + y;
                    Console.Write(" ");
                }
            }
            Console.CursorLeft = left;
            Console.CursorTop = top;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (border)
                //Terminal.Rect(x, y, width, height);
            Console.BackgroundColor = prevColor;

        }
        public static uint CenteredMenu(string[] items, int x, int y)
        {
            int lastLength = 0;
            int maxLength = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Length > lastLength)
                {
                    maxLength = items[i].Length;
                }
                lastLength = items[i].Length;
            }
            int room = 80;
            int half = room / 2;
            int halfl = maxLength / 2;
            int start = half - halfl;

            int room2 = 25;
            int half2 = room2 / 2;
            int halfl2 = items.Length / 2;
            int start2 = half2 - halfl2;
            uint choice = Menu(items, start, start2);
            return choice;
        }
        public static uint Menu(string[] items, int x, int y)
        {
            int curseleft = x;
            uint selc = 0;
            int cursetop = y;

            redraw:
            Console.CursorTop = y;
            for (int i = 0; i < items.Length; i++)
            {
                Console.CursorLeft = curseleft;
                if (i == selc)
                {
                    ConsoleColor c1 = Console.ForegroundColor;
                    ConsoleColor c2 = Console.BackgroundColor;
                    Console.BackgroundColor = c1;
                    Console.ForegroundColor = c2;
                    Console.WriteLine(items[i]);
                    Console.ForegroundColor = c1;
                    Console.BackgroundColor = c2;
                }
                else
                {

                    Console.CursorLeft = curseleft;
                    Console.WriteLine(items[i]);
                }
            }
            byte c = (byte)Console.Read();
            if (c == 145)
            {
                if (selc == 0) selc = (uint)items.Length;
                selc--;

                goto redraw;
            }
            else if (c == (byte)147)
            {

                selc++;
                if (selc == (uint)items.Length)
                    selc = 0;
                goto redraw;
            }
            else if (c == (byte)10)
            {
                return selc;
            }
            else
            {
                goto redraw;
            }
            Console.WriteLine(c.ToString());

        }


    }

    class commandes
    {
        public static void Umount(string arg)
        {
            if (arg == "-a")
                HAL.FileSystem.Root.Mountpoints.Clear();
            else
                HAL.FileSystem.Root.Unmount(arg);
        }

        public static void Mkfs(string[] args)
        {
            string name = args[0];
            GLNFS fs = new GLNFS((Partition)Devices.getDevice(name));
            string label = "Filesystem";
            if (args.Length == 3)
                label = args[1];
            fs.Format(label);
        }

        public static void Mount(string[] args)
        {
            bool temp = false;
            int size = 100;
            string loc = "";
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "-t")
                    temp = true;
                if (args[i] == "-size")
                {
                    size = Conversions.StringToInt(args[++i]);
                }
                if (args[i] == "tempfs")
                {
                    loc = args[++i];
                }
            }

            int fs = 1;
            for (int i = 0; i < Devices.dev.Count; i++)
            {
                BlockDevice device = (BlockDevice)(Devices.dev[i].dev);
                if (Devices.dev[i].name == args[0])
                {
                    GLNFS fd = new GLNFS((Partition)device);
                    if (HAL.GLNFS.isGFS((Partition)device))
                    {
                        //if (args.Length > 2)
                            ((HAL.RootFilesystem)HAL.FileSystem.Root).Mount("/dev/sd", fd);
                        //else
                        //    ((HAL.RootFilesystem)HAL.FileSystem.Root).Mount("", fd);
                        Console.WriteLine("Mounted!");
                    }
                }
            }
        }

        public string getFullPath(string s)
        {
                return "/" + Utils.Util.cleanName("/dev/sd") + " / " + Utils.Util.cleanName(s);
        }

        static BinaryWriter bw = new BinaryWriter(new MemoryStream(512));

        public static void Format(string s)
        {
            string devn = "/dev/sda";

            AtaPio bd = (AtaPio)Devices.getDevice(devn);
            bd.ReadBlock(0, 1, bw.BaseStream.Data);
            Console.WriteLine("Grunty OS Fixed disk utility");
            Console.WriteLine("Enter command (m for help)");
            while (true)
            {
                Console.Write("> ");
                string command = Console.ReadLine();
                if (command == "n")
                {
                    Console.Write("Partition(1-4):");
                    string part = Console.ReadLine();
                    Console.Write("Blocks:");
                    int size = Conversions.StringToInt(Console.ReadLine());
                    bool greater_than_one = false;

                    int address = 446;
                    byte head_start = 1;
                    ushort start_sect = 1;
                    byte start_cyln = 1;
                    uint rel_sect = 1;
                    if (part == "1")
                    {
                        address = 446;
                    }
                    else if (part == "2")
                        address = 462;
                    else if (part == "3")
                        address = 478;
                    else if (part == "4")
                        address = 494;
                    if (address != 446)
                    {
                        BinaryReader br = new BinaryReader(bw.BaseStream);
                        br.BaseStream.Position = address - 16;
                        br.BaseStream.Position++;
                        br.BaseStream.Position += 3;
                        head_start = br.BaseStream.Read();
                        byte b1 = br.BaseStream.Read();
                        byte b2 = br.BaseStream.Read();
                        ushort us = (ushort)BitConverter.ToInt16(new byte[] { b1, b2 }, 0);
                        start_sect = getShort1(us);
                        start_cyln = (byte)getShort2(us);
                        rel_sect = br.ReadUInt32();
                        greater_than_one = true;
                    }
                    bw.BaseStream.Position = address;
                    bw.Write((byte)0);

                    bw.Write(head_start);
                    bw.Write((ushort)getShort(start_sect, start_cyln));
                    bw.Write((byte)1);

                    uint c = (uint)(size / 12);
                    if (greater_than_one)
                        c = (uint)((rel_sect + size) / 12);
                    bw.Write((byte)(c * 6));

                    bw.Write((ushort)getShort(12, (byte)c));
                    bw.Write((uint)rel_sect);
                    bw.Write((uint)size);



                }
                else if (command == "a")
                {
                    Console.Write("Partition(1-4):");
                    string part = Console.ReadLine();
                    int address = 462;
                    if (part == "1")
                    {
                        address = 446;
                    }
                    else if (part == "2")
                        address = 462;
                    else if (part == "3")
                        address = 478;
                    else if (part == "4")
                        address = 494;

                    if (bw.BaseStream.Data[address] == 0)
                        bw.BaseStream.Data[address] = (byte)1;
                    else
                        bw.BaseStream.Data[address] = (byte)0;
                }
                else if (command == "t")
                {
                    Console.Write("Partition(1-4):");

                    string part = Console.ReadLine();
                    Console.Write("System Label: ");
                    int t = Conversions.StringToInt(Console.ReadLine());
                    int address = 462;

                    if (part == "1")
                    {
                        address = 446;
                    }
                    else if (part == "2")
                        address = 462;
                    else if (part == "3")
                        address = 478;
                    else if (part == "4")
                        address = 494;
                    address += 5;
                    bw.BaseStream.Position = address;
                    bw.Write((byte)(uint)t);
                }
                else if (command == "q")
                    break;
                else if (command == "w")
                {
                    Console.WriteLine("Writing to partition table...");
                    bw.BaseStream.Close();
                    bd.WriteBlock(0, 1, bw.BaseStream.Data);
                    Console.WriteLine("Changes saved!");
                    break;
                }
                else if (command == "p")
                {
                    BinaryReader br = new BinaryReader(bw.BaseStream);
                    br.BaseStream.Position = 446;
                    Console.WriteLine(" Starting Head     SystemID     Ending Head     Size");
                    byte boot = br.BaseStream.Read();
                    if (boot == 1)
                        Console.Write("*");
                    else
                        Console.WriteLine(" ");


                }
                else if (command == "d")
                {
                    Console.Write("Partition(1-4):");
                    string part = Console.ReadLine();
                    int address = 446;

                    if (part == "1")
                    {
                        address = 446;
                    }
                    else if (part == "2")
                        address = 462;
                    else if (part == "3")
                        address = 478;
                    else if (part == "4")
                        address = 494;
                    bw.BaseStream.Position = address;
                    for (int i = 1; i < 16; i++)
                    {
                        bw.Write((byte)0);
                    }
                }

                else if (command == "help" || command == "m")
                {
                    Console.WriteLine(@"Command (m for help): m
Command action
   a   toggle a bootable flag
   d   delete a partition
   m   print this menu
   n   add a new partition
   p   print the partition table
   q   quit
   t   change a partition's system id
   w   write table to disk and exit");
                }
            }

        }

        public static ushort getShort(ushort first, byte second)
        {
            ushort n = (ushort)(second << 10 | first);
            return n;
        }
        public static ushort getShort1(ushort first)
        {
            ushort y = (ushort)(int)(first & 0x3F);
            ushort x = (ushort)((first << 10) & 63); // the last 6 bits
            return y;
        }
        public static ushort getShort2(ushort first)
        {
            return (ushort)(int)(first >> 10);

        }

    }
}

