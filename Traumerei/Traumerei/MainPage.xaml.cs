using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Traumerei
{
    public partial class MainPage : ContentPage
    {
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
                Image img = sender as Image;
                if (img != null)
                    ChangeBackground(img);
            };
            tapGestureRecongnizer.NumberOfTapsRequired = 1;
            imgGenerate.GestureRecognizers.Add(tapGestureRecongnizer);
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
    }
}
