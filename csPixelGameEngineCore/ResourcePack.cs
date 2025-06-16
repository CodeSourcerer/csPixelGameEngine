using System;
using System.Collections.Generic;
using System.IO;

namespace csPixelGameEngineCore;

/// <summary>
/// Allows you to store files in one large scrambled file.
/// </summary>
public class ResourcePack : IDisposable
{
    private Dictionary<string, ResourceFile> _mapFiles;
    private BinaryReader _baseFile;

    public ResourcePack()
    {
        _mapFiles = new Dictionary<string, ResourceFile>();
    }

    /// <summary>
    /// This adds a file to the pack to be savd by SavePack later.
    /// </summary>
    /// <param name="sFile"></param>
    /// <returns></returns>
    public bool AddFile(string sFile)
    {
        if (File.Exists(sFile))
        {
            string file = makeposix(sFile);
            FileInfo fi = new FileInfo(sFile);

            ResourceFile rf = new ResourceFile
            {
                nSize = (uint)fi.Length,
                nOffset = 0                 // We'll figure this out in SavePack()
            };

            _mapFiles[file] = rf;

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
            byte[] scramblyBytes = _baseFile.ReadBytes((int)nIndexSize);
            byte[] mapData = scramble(scramblyBytes, sKey);

            using BinaryReader binReader = new BinaryReader(new MemoryStream(mapData));

            // 2. Read map
            uint nMapEntries = binReader.ReadUInt32();

            // Run through all the map entries reading in paths and offsets
            for (uint i = 0; i < nMapEntries; i++)
            {
                uint nFilePathSize = binReader.ReadUInt32();
                string fileName = new string(binReader.ReadChars((int)nFilePathSize));

                ResourceFile resourceFile = new ResourceFile
                {
                    nSize = binReader.ReadUInt32(),
                    nOffset = binReader.ReadUInt32()
                };
                _mapFiles[fileName] = resourceFile;
            }

            // We're going to leave _baseFile open just dangling its file handle around so we can
            // use it later
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
            using var binWriter = new BinaryWriter(File.Open(sFile, FileMode.Open));

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
            // 3. Scramble the image meta-data for fun and profit
            using var binWriterMangled = new BinaryWriter(new MemoryStream(512));

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

            // 4. Write out the scrambly bits
            binWriter.Seek(0, SeekOrigin.Begin);
            binWriter.Write(scrambledBytes);
            binWriter.Close();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public ResourceBuffer GetFileBuffer(string sFile)
    {
        string file = makeposix(sFile);
        return new ResourceBuffer(_baseFile, _mapFiles[file].nOffset, _mapFiles[file].nSize);
    }

    public bool Loaded() => _baseFile != null;

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
                if (_baseFile != null)
                {
                    _baseFile.Close();
                    _baseFile.Dispose();
                    _baseFile = null;
                }
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
