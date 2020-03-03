using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace csPixelGameEngineCore
{
    public struct ResourceBuffer
    {
        public Memory<byte> Memory { get; private set; }

        public ResourceBuffer(BinaryReader binReader, uint offset, uint size)
        {
            binReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            Memory = new Memory<byte>(binReader.ReadBytes((int)size));
        }
    }
}
