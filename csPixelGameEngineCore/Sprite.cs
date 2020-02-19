﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace csPixelGameEngineCore
{
    /// <summary>
    /// A 2D drawing surface.
    /// </summary>
    public class Sprite
    {
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public Pixel[] ColorData { get; private set; }

        private Sprite()
        {
            // Don't allow creation of sprites without dimensions
        }
        
        public Sprite(uint w, uint h)
        {
            if (w == 0)
                throw new ArgumentException("Argument must be greater than 0", nameof(w));
            if (h == 0)
                throw new ArgumentException("Argument must be greater than 0", nameof(h));

            Width = w;
            Height = h;
            ColorData = new Pixel[Width * Height];

            for (int i = 0; i < Width * Height; i++)
            {
                ColorData[i] = Pixel.BLACK;
            }
        }

        public void Fill(Pixel col)
        {
            Span<Pixel> dest = ColorData;
            dest.Fill(col);
        }

        /// <summary>
        /// Get the pixel value at the given x and y coordinates within this sprite.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The pixel value. If x and y coordinates are outside of the sprite boundaries, Pixel.BLANK is returned.</returns>
        public Pixel GetPixel(uint x, uint y)
        {
            // Not sure if I really want assertions or not....
            Debug.Assert(x < Width, "Attempt to get pixel outside of sprite boundaries");
            Debug.Assert(y < Height, "Attempt to get pixel outside of sprite boundaries");

            if (x < Width && y < Height)
                return ColorData[y * Width + x];
            else
                return Pixel.BLANK;
        }

        /// <summary>
        /// Set a pixel in this sprite to the given pixel value
        /// </summary>
        /// <param name="x">x coordinate in sprite to set</param>
        /// <param name="y">y coordinate in sprite to set</param>
        /// <param name="p">Pixel value to use</param>
        /// <returns>true if set, false if not (outside of sprite boundaries)</returns>
        public bool SetPixel(uint x, uint y, Pixel p)
        {
            // Not sure if I really want assertions or not....
            Debug.Assert(x < Width, "Attempt to set pixel outside of sprite boundaries");
            Debug.Assert(y < Height, "Attempt to set pixel outside of sprite boundaries");

            if (x < Width && y < Height)
            {
                ColorData[y * Width + x] = p;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fills a rect within this sprite with a given pixel.
        /// </summary>
        /// <param name="x">Left x coordinate of rectangle in sprite to start filling at</param>
        /// <param name="y">Upper y coordinate of rectangle in sprite to start filling at</param>
        /// <param name="width">Width of rectangle to fill</param>
        /// <param name="height">Height of rectangle to fill</param>
        /// <param name="p">Pixel value to fill with</param>
        public void FillRect(uint x, uint y, uint width, uint height, Pixel p)
        {
            uint x2 = x + width;
            uint y2 = y + height;

            // TODO: Write unit tests for below...its kinda suspect
            if (x >= Width)
                x = (Width - 1);
            if (y >= Height)
                y = (Height - 1);
            if (x2 >= Width)
                x2 = (Width - 1);
            if (y2 >= Height)
                y2 = (Height - 1);

            for (uint y1 = y; y1 < y2; y1++)
            {
                Span<Pixel> row = new Span<Pixel>(ColorData, (int)(y1 * Width + x), (int)(x2 - x));
                row.Fill(p);
            }

            //for (uint x1 = x; x1 < x2; x1++)
            //{
            //    for (uint y1 = y; y1 < y2; y1++)
            //    {
            //        ColorData[y1 * Width + x1] = p;
            //    }
            //}
        }

        public Pixel Sample(float x, float y)
        {
            uint sx = Math.Min((uint)((x * Width)), Width - 1);
            uint sy = Math.Min((uint)((y * Height)), Height - 1);
            return GetPixel(sx, sy);
        }

        public Pixel SampleBL(float u, float v)
        {
            u = u * Width - 0.5f;
            v = v * Height - 0.5f;
            uint y = (uint)Math.Floor(v); // Thanks @joshinils
            uint x = (uint)Math.Floor(u); // cast to int rounds toward zero, not downward
            float u_ratio = u - x;
            float v_ratio = v - y;
            float u_opposite = 1 - u_ratio;
            float v_opposite = 1 - v_ratio;

            Pixel p1 = GetPixel(Math.Max(x, 0), Math.Max(y, 0));
            Pixel p2 = GetPixel(Math.Min(x + 1, Width - 1), Math.Max(y, 0));
            Pixel p3 = GetPixel(Math.Max(x, 0), Math.Min(y + 1, Height - 1));
            Pixel p4 = GetPixel(Math.Min(x + 1, Width - 1), Math.Min(y + 1, Height - 1));

            return new Pixel(
                (byte)((p1.r * u_opposite + p2.r * u_ratio) * v_opposite + (p3.r * u_opposite + p4.r * u_ratio) * v_ratio),
                (byte)((p1.g * u_opposite + p2.g * u_ratio) * v_opposite + (p3.g * u_opposite + p4.g * u_ratio) * v_ratio),
                (byte)((p1.b * u_opposite + p2.b * u_ratio) * v_opposite + (p3.b * u_opposite + p4.b * u_ratio) * v_ratio));
        }
    }
}
