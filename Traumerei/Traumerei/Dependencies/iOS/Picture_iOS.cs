using System;
using System.Threading.Tasks;
using Foundation;
using SkiaSharp;
using Traumerei.Dependencies.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Picture_iOS))]

namespace Traumerei.Dependencies.iOS
{
    class Picture_iOS : IPicture
    {
        public bool SaveBitmapToDisk(string directoryPath, SKBitmap bitmap)
        {
            bool success = true;
            UIImage image = new UIImage(NSData.FromArray(bitmap.Bytes));
            image.SaveToPhotosAlbum((img, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine(error.ToString());
                    success = false;
                }
            });
            return success;
        }

        public Task<bool> SavePhotoAsync(byte[] data, string folder, string filename)
        {
            NSData nsData = NSData.FromArray(data);
            UIImage image = new UIImage(nsData);
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            image.SaveToPhotosAlbum((UIImage img, NSError error) =>
            {
                taskCompletionSource.SetResult(error == null);
            });

            return taskCompletionSource.Task;
        }
    }
}
