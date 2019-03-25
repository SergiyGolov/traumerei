using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Traumerei
{
    public partial class MainPage : ContentPage
    {
        private SKBitmap imgBitmap;


        public MainPage()
        {
            InitializeComponent();
            AddHandlers();
           
        }

        /// <summary>
        /// Add handlers to UI components
        /// </summary>
        private void AddHandlers()
        {
            //Add handler on Image imgGenerate
            var tapGestureRecongnizer = new TapGestureRecognizer();
            tapGestureRecongnizer.Tapped += (sender, e) => {
                Debug.WriteLine("taped once");
                Debug.Print("Type of sender: " + sender.GetType().ToString());
                drawRandomImage();
                //Image img = sender as Image;
                //if (img != null)
                //    ChangeBackground(img);
            };
            tapGestureRecongnizer.NumberOfTapsRequired = 1;
            imgGenerated.GestureRecognizers.Add(tapGestureRecongnizer);
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

        private void drawRandomImage()
        {
           if (imgBitmap==null)
                imgBitmap = new SKBitmap((int)imgGenerated.CanvasSize.ToFormsSize().Width, (int)imgGenerated.CanvasSize.ToFormsSize().Height);

            Random rnd = new Random();
            for (int x=0;x< imgBitmap.Width;x++)
            {
                for (int y = 0; y < imgBitmap.Height; y++)
                {
                    SKColor color = new SKColor((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                    imgBitmap.SetPixel(x, y, color);
                }
            }

            imgGenerated.InvalidateSurface();
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (imgBitmap!=null)
                canvas.DrawBitmap(imgBitmap, 0, 0);

            
        }
    }
}
