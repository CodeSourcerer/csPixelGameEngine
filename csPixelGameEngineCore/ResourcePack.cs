using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace csPixelGameEngineCore
{
    public class ResourcePack
    {
        private Dictionary<string, ResourceFile> _mapFiles;

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
            return false;
        }

		public bool SavePack(string sFile, string sKey)
        {
            return false;
        }

		public BinaryReader GetFileBuffer(string sFile)
        {
            return null;
        }

        public bool Loaded() => _fileReader != null;

  //      std::ifstream baseFile;
  //      const std::string scramble(const std::string& data, const std::string& key);
		private string makeposix(string path) => path.Replace('\\', '/');

        private struct ResourceFile
        {
            public uint nSize;
            public uint nOffset;
        }
    }
}
