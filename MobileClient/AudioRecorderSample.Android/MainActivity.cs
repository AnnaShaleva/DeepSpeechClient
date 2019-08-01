using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android;

namespace AudioRecorderSample.Droid
{
    [Activity(Label = "AudioRecorderSample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            if ((CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted) ||
                (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Android.Content.PM.Permission.Granted) ||
                (CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted) ||
                (CheckSelfPermission(Manifest.Permission.Internet) != Android.Content.PM.Permission.Granted))
            {
                RequestPermissions(new string[] { Android.Manifest.Permission.ReadExternalStorage, Android.Manifest.Permission.WriteExternalStorage, Android.Manifest.Permission.RecordAudio, Android.Manifest.Permission.Internet }, 1000);
            }

            LoadApplication(new App());
        }
    }
}

