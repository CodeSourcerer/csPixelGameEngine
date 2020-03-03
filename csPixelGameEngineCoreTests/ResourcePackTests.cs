using csPixelGameEngineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [TestMethod]
        public void Scrambled_WhenInvokedOnceWithByteArray_ReturnsDifferentByteArray()
        {
            ResourcePack pack = new ResourcePack();

            byte[] testBytes = (from c in "This is a test"
                                select (byte)c).ToArray();

            byte[] actualBytes = pack.scramble(testBytes, "mykey");

            string actualString = Encoding.Default.GetString(actualBytes);
            string testString = Encoding.Default.GetString(testBytes);
            Assert.IsFalse(testString.Equals(actualString));
        }


        [TestMethod]
        public void Scrambled_WhenInvokedTwiceWithByteArray_ReturnsDifferentByteArray()
        {
            ResourcePack pack = new ResourcePack();

            byte[] testBytes = (from c in "This is a test"
                                select (byte)c).ToArray();

            byte[] scrambledBytes = pack.scramble(testBytes, "mykey");
            byte[] actualBytes = pack.scramble(scrambledBytes, "mykey");

            string actualString = Encoding.Default.GetString(actualBytes);
            string testString = Encoding.Default.GetString(testBytes);
            Assert.IsTrue(testString.Equals(actualString));
        }
    }
}
