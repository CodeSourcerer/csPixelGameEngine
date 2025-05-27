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
        public void CreatingSprite_WithZeroXorY_IsNotValid(int w, int h)
        {
            // Below should throw ArgumentException
            Sprite testSprite = new Sprite(w, h);
        }

        [DataTestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(10, 10, 100)]
        public void CreatingSprite_WithNonZeroArguments_InitsColorDataToCorrectSize(int w, int h, int expectedSize)
        {
            Sprite testSprite = new Sprite(w, h);

            int actualSize = testSprite.ColorData.Length;

            Assert.AreEqual(expectedSize, actualSize, "Size of sprite ColorData array is invalid");
        }

        //[TestMethod]
        //public void CreatingSprite_WithNonZeroArguments_InitsColorDataToPixels()
        //{
        //    Sprite testSprite = new Sprite(10, 10);

        //    bool hasNullPixel = testSprite.ColorData.Any(p => p == null);

        //    Assert.IsFalse(hasNullPixel, "ColorData not fully initialized with Pixels");
        //}

        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(9, 9)]
        public void GetPixel_WithValidCoordinates_ReturnsPixel(int x, int y)
        {
            Sprite testSprite = new Sprite(10, 10);

            for (int x1 = 0; x1 < 10; x1++)
            {
                for (int y1 = 0; y1 < 10; y1++)
                {
                    // Set blue component based on position of pixel
                    testSprite.SetPixel(x1, y1, new Pixel(0, 0, (byte)(y1 * 10 + x1)));
                }
            }

            Pixel actualPixel = testSprite.GetPixel(x, y);

            Pixel expectedPixel = new Pixel(0, 0, (byte)(y * 10 + x));

            Assert.AreNotEqual(Pixel.BLANK, actualPixel, "Invalid pixel returned");
            Assert.AreEqual(expectedPixel.n, actualPixel.n);
        }

        [DataTestMethod]
        [DataRow(10, 2)]
        [DataRow(1, 50)]
        public void GetPixel_WithCoordsOutsideOfBounds_ReturnsBlankPixel(int x, int y)
        {
            Sprite testSprite = new Sprite(10, 10);

            Pixel actualPixel = testSprite.GetPixel(x, y);

            Assert.AreEqual(Pixel.BLANK, actualPixel);
        }

        [DataTestMethod]
        [DataRow(1, 1, (float)0.0, (float)0.0)]
        [DataRow(1, 1, (float)1.0, (float)1.0)]
        [DataRow(10, 10, (float)0.0, (float)10.01)]
        //[DataRow(10, 10, (float)-20.32, (float)3.14)]
        public void Sample_UsingVariousXYAndSpriteSizes_DoesNotBlowUp(int w, int h, float x, float y)
        {
            Sprite testSprite = new Sprite(w, h);

            Pixel actualPixel = testSprite.Sample(x, y);

            Assert.IsNotNull(actualPixel);
            Assert.AreEqual(Pixel.BLACK, actualPixel);
        }

        [TestMethod]
        public void Fill_SetsAllPixels_ToGivenPixel()
        {
            Sprite testSprite = new Sprite(10, 10);

            testSprite.Fill(Pixel.BLUE);

            Assert.AreEqual(Pixel.BLUE, testSprite.GetPixel(5, 2));
            Assert.AreEqual(Pixel.BLUE, testSprite.GetPixel(0, 0));
            Assert.AreEqual(Pixel.BLUE, testSprite.GetPixel(9, 9));
        }

        [DataTestMethod]
        [DataRow(10, 0, 1, 1)]
        [DataRow(0, 10, 1, 1)]
        [DataRow(0, 0, 20, 1)]
        [DataRow(0, 0, 1, 20)]
        [DataRow(10, 10, 10, 20)]
        public void FillRect_OutsideOfSpriteBounds_DoesNotBlowUp(int x, int y, int width, int height)
        {
            Sprite testSprite = new Sprite(10, 10);

            testSprite.FillRect(x, y, width, height, Pixel.BLUE);

            // Asserting anything useful is difficult - we just don't want exceptions thrown.
        }
    }
}
