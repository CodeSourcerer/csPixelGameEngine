using csPixelGameEngineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCoreTests
{
    [TestClass]
    public class vec3dTests
    {
        [TestMethod]
        public void CreatingVec3d_WithoutArgumentsToConstructor_SetsX_and_Y_and_Z_To_0_and_W_To_1()
        {
            vec3d actualVec3d = new vec3d();

            Assert.AreEqual(0.0f, actualVec3d.x, "Unexpected x value");
            Assert.AreEqual(0.0f, actualVec3d.y, "Unexpected y value");
            Assert.AreEqual(0.0f, actualVec3d.z, "Unexpected z value");
            Assert.AreEqual(1.0f, actualVec3d.w, "Unexpected w value");
        }
    }
}
