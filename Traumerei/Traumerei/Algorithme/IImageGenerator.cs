using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Traumerei.Algorithme
{
    interface IImageGenerator
    {
        /// <summary>
        /// Set the dimensions of the generated image
        /// </summary>
        /// <param name="width">width of the image</param>
        /// <param name="height">height of the image</param>
        void SetDimensions(int width, int height);

        /// <summary>
        /// Generate one image as a 3d float array.
        /// </summary>
        /// <returns>
        /// Returns a bitmap
        /// </returns>
        SKBitmap Generate();

        /// <summary>
        /// Modify a little bit the previous generated image
        /// to allow animations.
        /// </summary>
        /// <returns>
        /// Returns a bitmap
        /// </returns>
        SKBitmap Step();
    }
}
