using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Traumerei
{
    public interface IPicture
    {
        /// <summary>
        /// Save a Bitmap to the filesystem of the device
        /// </summary>
        /// <param name="directoryPath">path of the directory</param>
        /// <param name="bitmap">bitmap to save</param>
        /// <returns>if succed or not</returns>
        bool SaveBitmapToDisk(String directoryPath, SKBitmap bitmap);

        Task<bool> SavePhotoAsync(byte[] data, string folder, string filename);
    }
}
