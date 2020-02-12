using csPixelGameEngineCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csPixelGameEngineCoreTests
{
    [TestClass]
    public class SpriteTests
    {
        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [ExpectedException(typeof(ArgumentException))]
        public void CreatingSprite_WithZeroXorY_IsNotValid(uint w, uint h)
        {
            // Below should throw ArgumentException
            Sprite testSprite = new Sprite(w, h);
        }

        [DataTestMethod]
        [DataRow((uint)1, (uint)1, 1)]
        [DataRow((uint)10, (uint)10, 100)]
        public void CreatingSprite_WithNonZeroArguments_InitsColorDataToCorrectSize(uint w, uint h, int expectedSize)
        {
            Sprite testSprite = new Sprite(w, h);

            int actualSize = testSprite.ColorData.Length;

            Assert.AreEqual(expectedSize, actualSize, "Size of sprite ColorData array is invalid");
        }

        [TestMethod]
        public void CreatingSprite_WithNonZeroArguments_InitsColorDataToPixels()
        {
            Sprite testSprite = new Sprite(10, 10);

            bool hasNullPixel = testSprite.ColorData.Any(p => p == null);

            Assert.IsFalse(hasNullPixel, "ColorData not fully initialized with Pixels");
        }

        [DataTestMethod]
        [DataRow((uint)0, (uint)0)]
        [DataRow((uint)9, (uint)9)]
        public void GetPixel_WithValidCoordinates_ReturnsPixel(uint x, uint y)
        {
            Sprite testSprite = new Sprite(10, 10);

            for (uint x1 = 0; x1 < 10; x1++)
            {
                for (uint y1 = 0; y1 < 10; y1++)
                    testSprite.SetPixel(x1, y1, new Pixel(0, 0, (byte)(y1 * 10 + x1)));
            }

            Pixel actualPixel = testSprite.GetPixel(x, y);

            Assert.AreNotEqual(Pixel.BLANK, actualPixel, "Invalid pixel returned");
        }

        [DataTestMethod]
        [DataRow((uint)1, (uint)1, (float)0.0, (float)0.0)]
        [DataRow((uint)1, (uint)1, (float)1.0, (float)1.0)]
        [DataRow((uint)10, (uint)10, (float)0.0, (float)10.01)]
        [DataRow((uint)10, (uint)10, (float)-20.32, (float)3.14)]
        public void Sample_UsingVariousXYAndSpriteSizes_DoesNotBlowUp(uint w, uint h, float x, float y)
        {
            Sprite testSprite = new Sprite(w, h);

            Pixel actualPixel = testSprite.Sample(x, y);

            Assert.IsNotNull(actualPixel);
            Assert.AreEqual(Pixel.BLACK, actualPixel);
        }
    }
}
