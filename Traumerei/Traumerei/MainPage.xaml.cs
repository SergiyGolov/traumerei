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

        private void AddHandlers()
        {
            imgGenerate.GestureRecognizers.Add(new TapGestureRecognizer {
                Command = new Command((image) => {
                    Debug.WriteLine("taped image once");
                    ChangeBackground(image);
                }),
                    NumberOfTapsRequired = 1,
                }
            );
        }

        private static void ChangeBackground(Object view)
        {
            // Change background color
        }
    }
}
