using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace csPixelGameEngineCore
{
    public class ResourcePack
    {
        private Dictionary<string, ResourceFile> _mapFiles;
        private BinaryReader _fileReader;

        public ResourcePack(BinaryReader reader)
        {
            _mapFiles = new Dictionary<string, ResourceFile>();
            _fileReader = reader;
        }

        public bool AddFile(string sFile)
        {
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
            uint nSize;
            uint nOffset;
        }
    }
}
