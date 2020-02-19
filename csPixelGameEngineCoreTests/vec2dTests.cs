using csPixelGameEngineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCoreTests
{
    [TestClass]
    public class vec2dTests
    {
        [TestMethod]
        public void CreatingVec2d_WithoutArgumentsToConstructor_SetsU_and_V_and_W_To_0()
        {
            vec2d actualvec2d = new vec2d();

            Assert.AreEqual(0.0f, actualvec2d.u, "Unexpected u value");
            Assert.AreEqual(0.0f, actualvec2d.v, "Unexpected v value");
            Assert.AreEqual(0.0f, actualvec2d.w, "Unexpected w value");
        }
    }
}
