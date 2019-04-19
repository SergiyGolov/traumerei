using System;

using Android.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Permissions;
using Android.Content.PM;

namespace Traumerei.Droid
{
    [Activity(Label = "Traumerei", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        internal static MainActivity Instance { get; private set; }

        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Instance = this;

            //source: https://github.com/jamesmontemagno/PermissionsPlugin/issues/95
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Forms.DependencyService.Register<Traumerei.SaveImage.AndroidPhotoLibrary>();
            LoadApplication(new App());
        }

        //source: https://github.com/jamesmontemagno/PermissionsPlugin#android-specific-in-your-baseactivity-or-mainactivity-for-xamarinforms-add-this-code
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }



}