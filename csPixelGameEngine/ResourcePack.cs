using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPixelGameEngine.enums;
using csPixelGameEngine.util;

namespace csPixelGameEngine
{
    public class ResourcePack
    {
        private Dictionary<string, Entry> mapFiles;

        public ResourcePack()
        {
            this.mapFiles = new Dictionary<string, Entry>();
        }

        public struct Entry
        {
            public uint     ID;
            public long     FileOffset;
            public long     FileSize;
            public byte[]   data;
        }

        public rcode AddToPack(string file)
        {
            try 
            {
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite);

                Entry e = new Entry();
                e.FileSize = fs.Length;
                e.data = new byte[e.FileSize];

                int bytesRead = -1;
                int offset = 0;
                do
                {
                    bytesRead = fs.Read(e.data, offset, 64 * 1024);
                    offset += bytesRead;
                } while (bytesRead > 0);

                fs.Close();
                fs.Dispose();

                // Add to map
                this.mapFiles[file] = e;
            }
            catch (IOException)
            {
                return rcode.FAIL;
            }

            return rcode.OK;
        }

        public rcode SavePack(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.CreateNew, FileAccess.ReadWrite);

                int mapSize = this.mapFiles.Count;

                // write header
                fs.Write(BitConverter.GetBytes(mapSize), 0, sizeof(int));

                // write headers for each entry
                foreach(var e in this.mapFiles)
                {
                    byte[] filenameBytes = Encoding.ASCII.GetBytes(e.Key);
                    fs.Write(BitConverter.GetBytes(filenameBytes.Length), 0, sizeof(int));
                    fs.Write(filenameBytes, 0, filenameBytes.Length);
                    fs.Write(BitConverter.GetBytes(e.Value.ID), 0, sizeof(uint));
                    fs.Write(BitConverter.GetBytes(e.Value.FileSize), 0, sizeof(long));
                    fs.Write(BitConverter.GetBytes(e.Value.FileOffset), 0, sizeof(long));
                }

                // Write all data
                var offset = fs.Position;
                var mapFileNames = this.mapFiles.Keys;
                foreach (var filename in mapFileNames)
                {
                    Entry e = mapFiles[filename];
                    e.FileOffset = offset;
                    mapFiles[filename] = e;
                    fs.Write(e.data, 0, e.data.Length);
                    offset += e.data.Length;
                }

                // Update header with offsets
                fs.Seek(sizeof(int), SeekOrigin.Begin);
                foreach (var e in this.mapFiles)
                {
                    fs.Write(BitConverter.GetBytes(e.Key.Length), 0, sizeof(int));
                    fs.Write(BitConverter.GetBytes(e.Value.ID), 0, sizeof(uint));
                    fs.Write(BitConverter.GetBytes(e.Value.FileSize), 0, sizeof(long));
                    fs.Write(BitConverter.GetBytes(e.Value.FileOffset), 0, sizeof(long));
                }

                fs.Close();
                fs.Dispose();
            }
            catch (IOException)
            {
                return rcode.FAIL;
            }

            return rcode.OK;
        }

        public rcode LoadPack(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);

                int mapEntries = fs.ReadInt();

                for (int i = 0; i < mapEntries; i++)
                {
                    int filenameSize = fs.ReadInt();
                    string filename = fs.ReadString(filenameSize);

                    Entry e = new Entry();
                    e.ID            = fs.ReadUInt();
                    e.FileSize      = fs.ReadLong();
                    e.FileOffset    = fs.ReadLong();

                    mapFiles[filename] = e;
                }

                // Read data
                foreach(var key in mapFiles.Keys)
                {
                    var entry = mapFiles[key];
                    entry.data = new byte[entry.FileSize];
                    fs.Read(entry.data, 0, entry.data.Length);
                    this.mapFiles[key] = entry;
                }

                fs.Close();
                fs.Dispose();
            }
            catch (IOException)
            {
                return rcode.FAIL;
            }

            return rcode.OK;
        }
    }
}
