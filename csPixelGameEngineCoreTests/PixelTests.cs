using csPixelGameEngineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csPixelGameEngineCoreTests
{
    [TestClass]
    public class PixelTests
    {
        [TestMethod]
        public void WhenConstructed_WithIntValue_RGBASetCorrectly()
        {
            Pixel p1 = new Pixel(0x12345678);

            Assert.AreEqual<byte>(0x12, p1.r, "unexpected red value");
            Assert.AreEqual<byte>(0x34, p1.g, "unexpected green value");
            Assert.AreEqual<byte>(0x56, p1.b, "unexpected blue value");
            Assert.AreEqual<byte>(0x78, p1.a, "unexpected alpha value");
        }

        [TestMethod]
        public void WhenConstructed_WithNoParams_RGBIsZeroAndAlphaIs255()
        {
            Pixel p1 = new Pixel();

            Assert.AreEqual<byte>(0, p1.r, "unexpected red value");
            Assert.AreEqual<byte>(0, p1.g, "unexpected green value");
            Assert.AreEqual<byte>(0, p1.b, "unexpected blue value");
            Assert.AreEqual<byte>(0xFF, p1.a, "unexpected alpha value");
        }
    }
}
