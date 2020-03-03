using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace csPixelGameEngineCore
{
    public class ResourcePack : IDisposable
    {
        private Dictionary<string, ResourceFile> _mapFiles;
        private BinaryReader _baseFile;

        public ResourcePack()
        {
            _mapFiles = new Dictionary<string, ResourceFile>();
        }

        public bool AddFile(string sFile)
        {
            if (File.Exists(sFile))
            {
                FileInfo fi = new FileInfo(sFile);

                //BinaryReader br = new BinaryReader(File.OpenRead(sFile));

                ResourceFile rf = new ResourceFile
                {
                    nSize = (uint)fi.Length,
                    nOffset = 0
                };

                return true;
            }

            return false;
        }

        public bool LoadPack(string sFile, string sKey)
        {
            try
            {
                _baseFile = new BinaryReader(File.OpenRead(sFile));

                // 1. Read scrambled index
                uint nIndexSize = 0;
                nIndexSize = _baseFile.ReadUInt32();
                char[] buffer = _baseFile.ReadChars((int)nIndexSize);

                string decoded = scramble(new string(buffer), sKey);

                using (StringReader iss = new StringReader(decoded))
                {
                    int streamPtr = 0;
                    // 2. Read map
                    uint nMapEntries = 0;
                    char[] chMapEntries = new char[4];
                    streamPtr += iss.Read(chMapEntries, 0, 4);

                    //nMapEntries = uint.Parse()
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

		public bool SavePack(string sFile, string sKey)
        {
            try
            {
                using(var binWriter = new BinaryWriter(File.Open(sFile, FileMode.Open)))
                {
                    uint nIndexSize = 0;
                    binWriter.Write(nIndexSize);
                    uint nMapSize = (uint)_mapFiles.Count;
                    binWriter.Write(nMapSize);
                    foreach (var mapFile in _mapFiles)
                    {
                        // Write the path of the file
                        binWriter.Write(mapFile.Key.Length);
                        binWriter.Write(mapFile.Key);

                        // Write the file entry properties
                        binWriter.Write(mapFile.Value.nSize);
                        binWriter.Write(mapFile.Value.nOffset);
                    }

                    // 2. Write the data
                    var offset = binWriter.BaseStream.Position;
                    nIndexSize = (uint)offset;
                    foreach (var mapFilename in _mapFiles.Keys)
                    {
                        // Store beginning of file offset within resource pack file
                        var mapData = _mapFiles[mapFilename];
                        mapData.nOffset = (uint)offset;
                        _mapFiles[mapFilename] = mapData;

                        // Load the file to be added
                        using (var binReader = new BinaryReader(File.Open(mapFilename, FileMode.Open)))
                        {
                            binWriter.Write(binReader.ReadBytes((int)_mapFiles[mapFilename].nSize));
                        }
                        offset += _mapFiles[mapFilename].nSize;
                    }

                    byte[] scrambledBytes;
                    // 3. Scramble crap for fun and profit
                    using (var binWriterMangled = new BinaryWriter(new MemoryStream(512)))
                    {
                        binWriterMangled.Write(nMapSize);
                        foreach (var mapFile in _mapFiles)
                        {
                            // Write the path of the file
                            binWriterMangled.Write(mapFile.Key.Length);
                            binWriterMangled.Write(mapFile.Key);

                            // Write the file entry properties
                            binWriterMangled.Write(mapFile.Value.nSize);
                            binWriterMangled.Write(mapFile.Value.nOffset);
                        }
                        scrambledBytes = scramble(((MemoryStream)binWriterMangled.BaseStream).ToArray(), sKey);
                    }

                    // 4. Write out the scrambly bits
                    binWriter.Seek(0, SeekOrigin.Begin);
                    binWriter.Write(scrambledBytes);
                    binWriter.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

		public BinaryReader GetFileBuffer(string sFile)
        {
            return null;
        }

        public bool Loaded() => false;

  //      std::ifstream baseFile;
  //      const std::string scramble(const std::string& data, const std::string& key);
		private string makeposix(string path) => path.Replace('\\', '/');

        public string scramble(string data, string key)
        {
            uint c = 0;
            char[] o = new char[data.Length];
            foreach(var s in data)
            {
                o[c] = (char)(s ^ key[(int)((c++) % key.Length)]);
            }

            return new string(o);
        }

        public byte[] scramble(byte[] data, string key)
        {
            uint c = 0;
            byte[] o = new byte[data.Length];
            foreach (var s in data)
            {
                o[c] = (byte)(s ^ key[(int)((c++) % key.Length)]);
            }

            return o;
        }

        private struct ResourceFile
        {
            public uint nSize;
            public uint nOffset;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_baseFile != null) _baseFile.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
