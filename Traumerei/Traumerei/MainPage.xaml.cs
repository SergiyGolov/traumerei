﻿using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
            if(animation)
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

        //source: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/saving
        async void saveImage(object sender, EventArgs args)
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

        async void loadImage(object sender, EventArgs args)
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
                            stopAccelerometer();

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


                        //source: https://stackoverflow.com/questions/48422724/fastest-way-to-scale-an-skimage-skiasharp
                        SKImageInfo info = new SKImageInfo(width, height, SKColorType.Rgba8888);

                        SKImage output = SKImage.Create(info);

                        loaded.ScalePixels(output.PeekPixels(), SKFilterQuality.None);

                        imgBitmap = SKBitmap.FromImage(output);

                        skRect = new SKRect(0, 0, oldSize, oldSize);

                        generator.load(imgBitmap);
                        imgGenerated.InvalidateSurface();

                        if (animation)
                            startAccelerometer();
                    }
                }
            }
        }
    }
}
