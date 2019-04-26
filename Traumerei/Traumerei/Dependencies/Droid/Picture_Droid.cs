
using Android.Media;
using Android.OS;
using System.IO;
using Xamarin.Essentials;
using SkiaSharp;
using System.Threading.Tasks;
using Traumerei.Dependencies.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(Picture_Droid))]

namespace Traumerei.Dependencies.Droid
{
    class Picture_Droid : IPicture
    {
        public bool SaveBitmapToDisk(string directoryPath, SKBitmap bitmap)
        {
            return true;
            /*Context context = Android.App.Application.Context;
            Android.Graphics.Bitmap droidBitmap = SKBitmap.Enc

            byte[] imageData = bitmap.Bytes;            

            string name = System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
            string filePath = System.IO.Path.Combine(directoryPath, name);
            System.Console.WriteLine("Directory Path is: " + directoryPath);
            try
            {
                // Create the directory
                System.IO.Directory.CreateDirectory(directoryPath);
                
                // Create and write in file
                System.IO.File.WriteAllBytes(filePath, imageData);


                Android.Provider.MediaStore.Images.Media.InsertImage(context.ContentResolver, System.IO.AndroidExtensions.ToBitmap(bitmap), name, "generatedPicture");

                //mediascan adds the saved image into the gallery  
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Uri.FromFile(new File(filePath)));
                Android.App.Application.Context.SendBroadcast(mediaScanIntent, "intent of saving the image");
                

                // https://www.grapecity.com/en/blogs/how-to-save-an-image-to-a-device-using-xuni-and-xamarin-forms
                // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/dependency-service/introduction
                // TODO : Trouver les bons imports pour tester le fonctionnement

                return true;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }*/
        }

        async Task<bool> IPicture.SavePhotoAsync(byte[] data, string folder, string filename)
        {
            if(!Environment.MediaMounted.Equals(Environment.ExternalStorageState))
            {
                System.Console.WriteLine("Not allowed to write on external storage...");
                return false;
            } else
                System.Console.WriteLine("Allowed to write on external storage!");

            // See if it works !
            // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/saving
            try
            {
                var picturesDirectory = Android.App.Application.Context.GetExternalFilesDir(Environment.DirectoryPictures);
                System.Console.WriteLine("Get pictures directory: " + picturesDirectory.Path);
                System.Console.WriteLine("This directory contains: " + picturesDirectory.List());
                System.Console.WriteLine("Allowed to write ? " + picturesDirectory.CanWrite());
                DirectoryInfo folderDirectory = Directory.CreateDirectory(Path.Combine(picturesDirectory.Path, folder));
                System.Console.WriteLine("Has create the folder");

                using (var bitmapStream = File.Create(Path.Combine(folderDirectory.FullName, filename)))
                {
                    System.Console.WriteLine("Has open the stream to " + bitmapStream.Name);
                    bitmapStream.Write(data, 0, data.Length);
                    System.Console.WriteLine("Has write the content");
                    bitmapStream.Flush();
                    System.Console.WriteLine("Has flush the stream");

                    // Make sure it shows up in the Photos gallery promptly.
                    MediaScannerConnection.ScanFile(Android.App.Application.Context,
                                                    new string[] { bitmapStream.Name },
                                                    new string[] { "image/png", "image/jpeg" }, null);
                    System.Console.WriteLine("has write to: ", bitmapStream.Name);
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("An exception has occured: " + e);
                return false;
            }
            return true;
        }
    }
}
