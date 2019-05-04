using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Traumerei.Algorithme;
using Xamarin.Forms;
using Xamarin.Essentials;
using Traumerei.SaveImage;

namespace Traumerei
{
    public partial class MainPage : ContentPage
    {
        private SKBitmap imgBitmap;
        private int width;
        private int height;
        private ImageGenerator_RandomFunctions generator;
        private bool animation;
        private SKRect skRect;

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
                DrawRandomImageAsync();

            };
            tapGestureRecongnizer.NumberOfTapsRequired = 1;
            imgGenerated.GestureRecognizers.Add(tapGestureRecongnizer);

            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }


        /// <summary>
        /// Starts the accelerometer
        /// </summary>
        private void StartAccelerometer()
        {
            try
            {
                if (!Accelerometer.IsMonitoring)
                    Accelerometer.Start(SensorSpeed.Game);
            }
            catch (FeatureNotSupportedException)
            {
                // Feature not supported on device
            }
            catch (Exception)
            {
                // Other error has occurred.
            }
        }


        /// <summary>
        /// Stops the accelerometer
        /// </summary>
        private void StopAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
            }
            catch (FeatureNotSupportedException)
            {
                // Feature not supported on device
            }
            catch (Exception)
            {
                // Other error has occurred.
            }
        }


        /// <summary>
        /// Toggles the accelerometer, this is a way to toggle the animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnAnimationToggledEvent(object sender, EventArgs args)
        {
            animation = !animation;
            if (animation && imgBitmap != null)
            {
                StartAccelerometer();
            }
            else
            {
                StopAccelerometer();
            }
        }


        /// <summary>
        /// Toggles the animation anchor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnAnimationAnchorToggledEvent(object sender, EventArgs args)
        {
            generator.ToggleAnimationAnchor();
        }

        /// <summary>
        /// Sends the accelerometer data to the generator object to apply the animation effect on the image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // source: https://docs.microsoft.com/en-us/xamarin/essentials/accelerometer
        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {

            StopAccelerometer();

            var data = e.Reading;

            // Process Acceleration X, Y, and Z
            if(animation)
                imgBitmap = generator.Step(data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z);

            imgGenerated.InvalidateSurface();

            StartAccelerometer();

        }


        /// <summary>
        /// Calls the generate method of the generator and display the result in the GUI
        /// </summary>
        private async void DrawRandomImageAsync()
        {

            StopAccelerometer();


            ActivityIndicator runningIndicator = FindByName("runningIndicator") as ActivityIndicator;

            runningIndicator.IsRunning = true;
            await Task.Yield(); //This is a way to see the running indicator animation while the image is generated


            //If first time called, sets the dimensions of the image
            if (imgBitmap == null)
            {
                width = (int)imgGenerated.CanvasSize.ToFormsSize().Width;
                height = (int)imgGenerated.CanvasSize.ToFormsSize().Height;
                int oldSize = width;
                if (width > height)
                    oldSize = height;

                int size = oldSize;
                for (int i = 1; i < oldSize; i <<= 1)
                {
                    size = i;
                }

                width = size;
                height = size;


                generator.SetDimensions(size, size);
                skRect = new SKRect(0, 0, oldSize, oldSize);
                imgBitmap = new SKBitmap(width, height);
            }


            imgBitmap = generator.Generate();

            imgGenerated.InvalidateSurface();

            runningIndicator.IsRunning = false;

            StartAccelerometer();
        }


        /// <summary>
        /// Called to paint the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPainting(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (imgBitmap != null)
            {
                canvas.DrawBitmap(imgBitmap, skRect);
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


        /// <summary>
        /// Save the image on the phone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        //source: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/saving
        private async void saveImage(object sender, EventArgs args)
        {
            if (imgBitmap != null)
            {
                using (SKImage image = SKImage.FromBitmap(imgBitmap))
                {
                    try
                    {
                        SKData data = image.Encode();
                        IPhotoLibrary photoLibrary = DependencyService.Get<IPhotoLibrary>();
                        if (photoLibrary == null)
                        {
                            Console.WriteLine("SAVEIMAGE: photoLibrary null!");
                            return;
                        }

                        bool result = await photoLibrary.SavePhotoAsync(data.ToArray(), "Traumerei", Convert.ToString(Guid.NewGuid()));

                        if (!result)
                        {
                            Console.WriteLine("SAVEIMAGE: SavePhotoAsync return false");
                        }
                        else
                        {
                            Console.WriteLine("SAVEIMAGE: Success!");
                        }
                    }
                    catch (Exception ex)
                    {
                        string err = ex.InnerException.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Load an image from the phone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void LoadImage(object sender, EventArgs args)
        {
            IPhotoLibrary photoLibrary = DependencyService.Get<IPhotoLibrary>();

            using (Stream stream = await photoLibrary.PickPhotoAsync())
            {
                if (stream != null)
                {
                    SKBitmap loaded = SKBitmap.Decode(stream);
                    if (loaded != null)
                    {
                        if(animation)
                            StopAccelerometer();


                        //Set the image dimensions
                        width = (int)imgGenerated.CanvasSize.ToFormsSize().Width;
                        height = (int)imgGenerated.CanvasSize.ToFormsSize().Height;

                        int oldSize = width;
                        if (width > height)
                            oldSize = height;

                        int size = oldSize;
                        for (int i = 1; i < oldSize; i <<= 1)
                        {
                            size = i;
                        }

                        width = size;
                        height = size;

                        generator.SetDimensions(width, height);


                        // Resize the loaded image
                        //source: https://stackoverflow.com/questions/48422724/fastest-way-to-scale-an-skimage-skiasharp
                        SKImageInfo info = new SKImageInfo(width, height, SKColorType.Rgba8888);

                        SKImage output = SKImage.Create(info);

                        loaded.ScalePixels(output.PeekPixels(), SKFilterQuality.None);

                        imgBitmap = SKBitmap.FromImage(output);

                        skRect = new SKRect(0, 0, oldSize, oldSize);

                        generator.Load(imgBitmap);
                        imgGenerated.InvalidateSurface();

                        if (animation)
                            StartAccelerometer();
                    }
                }
            }
        }
    }
}
