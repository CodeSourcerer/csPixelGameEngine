using System;
using System.Diagnostics;
using csPixelGameEngine;

namespace TestApplication
{
    public class PixelTests
    {
        public PixelTests()
        {
        }

        public void ConstructorWithUintPixelValue()
        {
            Console.Write("ConstructorWithUintPixelValue  ");

            Pixel p1 = new Pixel(0x12345678);

            if (p1.r != 0x12) throw new ApplicationException("unexpected red value");
            if (p1.g != 0x34) throw new ApplicationException("unexpected green value");
            if (p1.b != 0x56) throw new ApplicationException("unexpected blue value");
            if (p1.a != 0x78) throw new ApplicationException("unexpected alpha value");

            Console.WriteLine("[Passed]");
        }

        public void ConstructorWithRGBAPixelValue()
        {
            Console.Write("ConstructorWithRGBAPixelValue  ");
            Pixel p1 = new Pixel(0x12, 0x34, 0x56, 0x78);

            if (p1.r != 0x12) throw new ApplicationException("unexpected red value");
            if (p1.g != 0x34) throw new ApplicationException("unexpected green value");
            if (p1.b != 0x56) throw new ApplicationException("unexpected blue value");
            if (p1.a != 0x78) throw new ApplicationException("unexpected alpha value");

            Console.WriteLine("[Passed]");
        }

        public void ConstructorWithRGBPixelValue()
        {
            Console.Write("ConstructorWithRGBPixelValue  ");
            Pixel p1 = new Pixel(0x12, 0x34, 0x56);

            if (p1.r != 0x12) throw new ApplicationException("unexpected red value");
            if (p1.g != 0x34) throw new ApplicationException("unexpected green value");
            if (p1.b != 0x56) throw new ApplicationException("unexpected blue value");
            if (p1.a != 0xFF) throw new ApplicationException("unexpected alpha value");

            Console.WriteLine("[Passed]");
        }

        public void ConstructorWithoutParameters()
        {
            Console.Write("ConstructorWithoutParameters  ");
            Pixel p1 = new Pixel();

            if (p1.r != 0x00) throw new ApplicationException("unexpected red value");
            if (p1.g != 0x00) throw new ApplicationException("unexpected green value");
            if (p1.b != 0x00) throw new ApplicationException("unexpected blue value");
            if (p1.a != 0x00) throw new ApplicationException("unexpected alpha value");

            Console.WriteLine("[Passed]");
        }

        public void SettingRed_LeavesOtherColors()
        {
            Console.Write("SettingRed_LeavesOtherColors  ");
            Pixel p1 = new Pixel(0x12345678);

            p1.r = 0xFF;

            if (p1.r != 0xFF) throw new ApplicationException("unexpected red value");
            if (p1.g != 0x34) throw new ApplicationException("unexpected green value");
            if (p1.b != 0x56) throw new ApplicationException("unexpected blue value");
            if (p1.a != 0x78) throw new ApplicationException("unexpected alpha value");
            if (p1.color != 0xFF345678) throw new ApplicationException("unexpected color value");

            Console.WriteLine("[Passed]");
        }

        public void SettingGreen_LeavesOtherColors()
        {
            Console.Write("SettingGreen_LeavesOtherColors  ");
            Pixel p1 = new Pixel(0x12345678);

            p1.g = 0xFF;

            if (p1.r != 0x12) throw new ApplicationException("unexpected red value");
            if (p1.g != 0xFF) throw new ApplicationException("unexpected green value");
            if (p1.b != 0x56) throw new ApplicationException("unexpected blue value");
            if (p1.a != 0x78) throw new ApplicationException("unexpected alpha value");
            if (p1.color != 0x12FF5678) throw new ApplicationException("unexpected color value");

            Console.WriteLine("[Passed]");
        }

        public void SettingBlue_LeavesOtherColors()
        {
            Console.Write("SettingBlue_LeavesOtherColors  ");
            Pixel p1 = new Pixel(0x12345678);

            p1.b = 0xFF;

            if (p1.r != 0x12) throw new ApplicationException("unexpected red value");
            if (p1.g != 0x34) throw new ApplicationException("unexpected green value");
            if (p1.b != 0xFF) throw new ApplicationException("unexpected blue value");
            if (p1.a != 0x78) throw new ApplicationException("unexpected alpha value");
            if (p1.color != 0x1234FF78) throw new ApplicationException("unexpected color value");

            Console.WriteLine("[Passed]");
        }

        public void SettingAlpha_LeavesOtherColors()
        {
            Console.Write("SettingAlpha_LeavesOtherColors  ");
            Pixel p1 = new Pixel(0x12345678);

            p1.a = 0xFF;

            if (p1.r != 0x12) throw new ApplicationException("unexpected red value");
            if (p1.g != 0x34) throw new ApplicationException("unexpected green value");
            if (p1.b != 0x56) throw new ApplicationException("unexpected blue value");
            if (p1.a != 0xFF) throw new ApplicationException("unexpected alpha value");
            if (p1.color != 0x123456FF) throw new ApplicationException("unexpected color value");

            Console.WriteLine("[Passed]");
        }
    }
}
