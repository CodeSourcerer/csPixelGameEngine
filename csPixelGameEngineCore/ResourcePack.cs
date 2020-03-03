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

                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
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
