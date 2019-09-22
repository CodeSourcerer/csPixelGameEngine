using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngine.util
{
    public static class FileStreamExtensions
    {
        public static int ReadInt(this FileStream fs)
        {
            byte[] intToRead = new byte[sizeof(int)];
            fs.Read(intToRead, 0, intToRead.Length);
            return BitConverter.ToInt32(intToRead, 0);
        }

        public static uint ReadUInt(this FileStream fs)
        {
            byte[] uintToRead = new byte[sizeof(int)];
            fs.Read(uintToRead, 0, uintToRead.Length);
            return BitConverter.ToUInt32(uintToRead, 0);
        }

        public static long ReadLong(this FileStream fs)
        {
            byte[] longToRead = new byte[sizeof(long)];
            fs.Read(longToRead, 0, longToRead.Length);
            return BitConverter.ToInt64(longToRead, 0);
        }

        public static string ReadString(this FileStream fs, int stringSize)
        {
            byte[] stringToRead = new byte[stringSize];
            fs.Read(stringToRead, 0, stringSize);
            return Encoding.ASCII.GetString(stringToRead);
        }
    }
}
