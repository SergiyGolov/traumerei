﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Traumerei.Algorithme
{
    class ImageGenerator_Random : IImageGenerator
    {
        protected static ImageGenerator_Random instance = null;
        protected static readonly float _delta = 0.001f;
        protected Random _random;

        protected SKBitmap imgBitmap;

        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;

        public ImageGenerator_Random()
        {
            _random = new Random();
        }

        /// <summary>
        /// Return the RGB value for one pixel at
        /// th x, y coordonates
        /// </summary>
        /// <param name="x">x coordonate</param>
        /// <param name="y">y coordonate</param>
        /// <returns>RGB value as array</returns>
        protected float[] F(int x, int y, float delta = 0)
        {
            return new float[] { (float) _random.NextDouble(),
                (float) _random.NextDouble(),
                (float) _random.NextDouble()
            };
        }

        /// 
        /// Implementation of IImageGenerator
        /// 

        public void SetDimensions(int width, int height)
        {
            Width = width;
            Height = height;
            imgBitmap = new SKBitmap(Width, Height);
        }

        public SKBitmap Generate()
        {
            SKColor color = new SKColor(0, 0, 0);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float[] RGBvalues = F(x, y);
                    imgBitmap.SetPixel(x, y, color
                       .WithRed((byte)(RGBvalues[0] * 255))
                       .WithGreen((byte)(RGBvalues[1] * 255))
                       .WithBlue((byte)(RGBvalues[2] * 255))
                   );
                }
            }
            return imgBitmap;
        }

        public SKBitmap Step()
        {
            float[,,] image = new float[Width, Height, 3];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float[] RGBvalues = F(x, y, _delta);
                    image[x, y, 0] = RGBvalues[0];
                    image[x, y, 1] = RGBvalues[1];
                    image[x, y, 2] = RGBvalues[2];
                }
            }
            return imgBitmap;
        }
    }
}
