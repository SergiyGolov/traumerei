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
                    Command = new Command(() => Debug.WriteLine("taped image once")),
                    NumberOfTapsRequired = 1,
                }
            );
        }
    }
}
