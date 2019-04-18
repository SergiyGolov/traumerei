using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Traumerei.Algorithme;
using Xamarin.Forms;


namespace Traumerei
{
    public partial class MainPage : ContentPage
    {
        private SKBitmap imgBitmap;
        private int width;
        private int height;
        private IImageGenerator generator;

        public MainPage()
        {
            InitializeComponent();
            AddHandlers();
            generator = ImageGenerator_Random.GetInstance();
        }

        /// <summary>
        /// Add handlers to UI components
        /// </summary>
        private void AddHandlers()
        {
            //Add handler on Image imgGenerate
            var tapGestureRecongnizer = new TapGestureRecognizer();
            tapGestureRecongnizer.Tapped += (sender, e) =>
            {
                Debug.WriteLine("taped once");
                Debug.Print("Type of sender: " + sender.GetType().ToString());
                DrawRandomImage();
            };
            tapGestureRecongnizer.NumberOfTapsRequired = 1;
            imgGenerated.GestureRecognizers.Add(tapGestureRecongnizer);

            // Add on click to saveButton
            btnSave.Clicked += (sender, e) =>
            {
                Console.WriteLine("has clicked button save");
                Task<bool> task = SaveBitmapToGallery(imgBitmap);
                Console.WriteLine("Has start the task. Wait for it to end");
                task.Wait();
                if (task.Result)
                    Debug.WriteLine("The image has been saved successfully");
                else
                    Debug.WriteLine("The image hasn't been saved. Please, refer to the debug log.");
            };
        }

        private void DrawRandomImage()
        {
            if (imgBitmap == null)
            {
                width = (int)imgGenerated.CanvasSize.ToFormsSize().Width;
                height = (int)imgGenerated.CanvasSize.ToFormsSize().Height;
                imgBitmap = new SKBitmap(width, height);
                generator.SetDimensions(width, height);
            }

            imgBitmap = generator.Generate();

            imgGenerated.InvalidateSurface();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (imgBitmap != null)
                canvas.DrawBitmap(imgBitmap, 0, 0);
        }

        private async Task<bool> SaveBitmapToGallery(SKBitmap bitmap)
        {
            // https://forums.xamarin.com/discussion/75958/using-skiasharp-how-to-save-a-skbitmap
            // https://www.c-sharpcorner.com/article/local-file-storage-using-xamarin-form/

            string directoryName = "Traumerei";

            String localStorage = PCLStorage.FileSystem.Current.LocalStorage.Path;
            String[] paths = { localStorage, directoryName };
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            try
            {
                var folder = new PCLStorage.FileSystemFolder(PCLStorage.PortablePath.Combine(paths));
                var file = await folder.CreateFileAsync("testImage.png", PCLStorage.CreationCollisionOption.ReplaceExisting, token);
                Console.WriteLine("has create folder and file");

                using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    Console.WriteLine("Has open the file");
                    SKData data = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
                    Console.WriteLine("Has encode the data");
                    data.SaveTo(stream);
                    Console.WriteLine("Has write in the stream");
                }
            } catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e);
                return false;
            }
            return true;
        }
    }
}
