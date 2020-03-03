using csPixelGameEngineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCoreTests
{
    [TestClass]
    public class ResourcePackTests
    {
        [TestMethod]
        public void Scrambled_WhenInvokedOnce_ReturnsDifferentString()
        {
            ResourcePack pack = new ResourcePack();

            string actualString = pack.scramble("This is a test", "mykey");

            Assert.IsFalse("This is a test".Equals(actualString));
        }

        [TestMethod]
        public void Scrambled_WhenInvokedTwice_ReturnsSameString()
        {
            ResourcePack pack = new ResourcePack();

            string scrambledString = pack.scramble("This is a test", "mykey");
            string actualString = pack.scramble(scrambledString, "mykey");

            Assert.IsTrue("This is a test".Equals(actualString));
        }
    }
}
