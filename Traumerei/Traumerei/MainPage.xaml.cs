using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Traumerei.Algorithme;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Traumerei
{
    public partial class MainPage : ContentPage
    {
        private SKBitmap imgBitmap;
        private int width;
        private int height;
        private ImageGenerator_RandomFunctions generator;
        private bool animation;

        public MainPage()
        {
            InitializeComponent();
            AddHandlers();
            generator = new ImageGenerator_RandomFunctions();
            animation = true;
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
                drawRandomImageAsync();

            };
            tapGestureRecongnizer.NumberOfTapsRequired = 1;
            imgGenerated.GestureRecognizers.Add(tapGestureRecongnizer);

            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }

        private void startAccelerometer()
        {
            try
            {
                if (!Accelerometer.IsMonitoring)
                    Accelerometer.Start(SensorSpeed.Game);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private void stopAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        public void OnAnimationToggledEvent(object sender, EventArgs args)
        {
            animation = !animation;
            if (animation && imgBitmap != null)
            {
                startAccelerometer();
            }
            else
            {
                stopAccelerometer();
            }
        }

        public void OnAnimationAnchorToggledEvent(object sender, EventArgs args)
        {
            generator.toggleAnimationAnchor();
        }

        // source: https://docs.microsoft.com/en-us/xamarin/essentials/accelerometer
        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {

            stopAccelerometer();

            var data = e.Reading;

            // Process Acceleration X, Y, and Z
            imgBitmap = generator.Step(data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z);

            imgGenerated.InvalidateSurface();

            startAccelerometer();

        }

        /// <summary>
        /// Change the background color of an image with a
        /// random color
        /// </summary>
        /// <param name="img">image target</param>
        private static void ChangeBackground(Image img)
        {
            Random r = new Random();
            Color randomColor = new Color(r.NextDouble(), r.NextDouble(), r.NextDouble());
            Debug.Print("Color: " + randomColor.ToString());

            img.BackgroundColor = randomColor;
        }

        private async void drawRandomImageAsync()
        {

            stopAccelerometer();


            ActivityIndicator runningIndicator = FindByName("runningIndicator") as ActivityIndicator;

            runningIndicator.IsRunning = true;
            await Task.Yield();

            if (imgBitmap == null)
            {
                width = (int)imgGenerated.CanvasSize.ToFormsSize().Width;
                height = (int)imgGenerated.CanvasSize.ToFormsSize().Height;
                int size = width;
                if (width > height)
                    size = height;
                imgBitmap = new SKBitmap(size, size);
                generator.SetDimensions(size, size);
            }


            imgBitmap = generator.Generate();
            imgGenerated.InvalidateSurface();

            runningIndicator.IsRunning = false;

            startAccelerometer();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (imgBitmap != null)
            {
                canvas.DrawBitmap(imgBitmap, 0, 0);
            }
            else
            {
                string resourceID = "Traumerei.ressources.tap.bmp";
                Assembly assembly = GetType().GetTypeInfo().Assembly;

                using (Stream stream = assembly.GetManifestResourceStream(resourceID))
                {
                    SKBitmap bitmap = SKBitmap.Decode(stream);
                    canvas.DrawBitmap(bitmap, info.Width / 4, info.Height / 4);
                }
            }
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
