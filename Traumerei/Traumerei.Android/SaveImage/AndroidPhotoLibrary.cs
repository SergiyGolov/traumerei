
using System.Threading.Tasks;
using Android.Content;
using Android.Media;
using Android.OS;
using Java.IO;
using Java.Lang;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Traumerei.Droid;


namespace Traumerei.SaveImage
{
    //source: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/saving and https://github.com/jamesmontemagno/PermissionsPlugin#in-action
    public class AndroidPhotoLibrary : IPhotoLibrary
    {

        //source: https://github.com/xamarin/xamarin-forms-samples/blob/master/SkiaSharpForms/Demos/Demos/SkiaSharpFormsDemos.Droid/PhotoLibrary.cs
        public async Task<System.IO.Stream> PickPhotoAsync()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Storage))
                        status = results[Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {

                    // Define the Intent for getting images
                    Intent intent = new Intent();
                    intent.SetType("image/*");
                    intent.SetAction(Intent.ActionGetContent);

                    // Start the picture-picker activity (resumes in MainActivity.cs)
                    MainActivity.Instance.StartActivityForResult(
                        Intent.CreateChooser(intent, "Select Picture"),
                        MainActivity.PickImageId);

                    // Save the TaskCompletionSource object as a MainActivity property
                    MainActivity.Instance.PickImageTaskCompletionSource = new TaskCompletionSource<System.IO.Stream>();

                    // Return Task object
                    return await MainActivity.Instance.PickImageTaskCompletionSource.Task;


                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Permission Error: " + ex);

            }
            return null;
        }

        public async Task<bool> SavePhotoAsync(byte[] data, string folder, string filename)
        {

            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Storage))
                        status = results[Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {
                    try
                    {
                        File picturesDirectory = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);
                        File folderDirectory = picturesDirectory;

                        if (!string.IsNullOrEmpty(folder))
                        {
                            folderDirectory = new File(picturesDirectory, folder);
                            folderDirectory.Mkdirs();
                        }

                        using (File bitmapFile = new File(folderDirectory, filename))
                        {
                            bitmapFile.CreateNewFile();

                            using (FileOutputStream outputStream = new FileOutputStream(bitmapFile))
                            {
                                await outputStream.WriteAsync(data);
                            }

                            // Make sure it shows up in the Photos gallery promptly.
                            MediaScannerConnection.ScanFile(MainActivity.Instance,
                                                            new string[] { bitmapFile.Path },
                                                            new string[] { "image/png", "image/jpeg" }, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Save Error: " + ex);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Permission Error: "+ ex);
                return false;
            }

            return true;
        }
    }
}
