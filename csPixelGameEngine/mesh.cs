using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngine
{
    public class Mesh
    {
        public List<Triangle> triangles { get; set; }

        public Mesh()
        {
            this.triangles = new List<Triangle>();
        }

        public bool LoadFromObjectFile(string filename, bool hasTexture)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            // implement later
            fs.Close();

            return false;
        }
    }
}
