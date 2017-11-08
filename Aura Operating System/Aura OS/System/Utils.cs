using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura_OS.IO;

namespace Aura_OS.Utils
{
    public class Util
    {
        public static string RemoveNonprintableChars(string str)
        {
            string accum = "";
            foreach (char c in str)
            {
                if ((byte)c >= 32 && (byte)c <= 126)
                {
                    accum += c.ToString();
                }
            }
            return accum;
        }
        public static string Remove(string str, char with)
        {
            string accum = "";
            foreach (char c in str)
            {
                if (c.ToString() != with.ToString())
                {
                    accum += c.ToString();
                }
            }
            return accum;
        }
        public static bool isWhiteSpace(char c)
        {
            if (c == ' ')
            {
                return true;
            }
            else if (c == '\r')
            {
                return true;
            }
            else if (c == '\n')
            {
                return true;
            }
            return false;
        }
        public static bool isLetter(char c)
        {
            byte code = (byte)c;
            if (code >= 65 && code <= 90)
            {
                return true;
            }
            if (code >= 97 && code <= 122)
            {
                return true;
            }
            return false;
        }
        public static bool isLetterOrDigit(char c)
        {
            if (isLetter(c))
            {
                return true;
            }
            byte code = (byte)c;
            if (code >= 48 && code <= 58)
            {
                return true;
            }
            return false;
        }
        public static bool isDigit(char c)
        {

            byte code = (byte)c;
            if (code >= 48 && code <= 58)
            {
                return true;
            }
            return false;
        }
        public static bool Contains(string Str, char c)
        {
            foreach (char ch in Str)
            {
                if (ch == c)
                    return true;
            }
            return false;
        }
        public static int IndexOf(string str, char c)
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
        public static string cleanName(string name)
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

        public static int LastIndexOf(string This, char ch)
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
    }
}
