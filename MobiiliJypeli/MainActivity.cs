using Android.App;
using Android.Content.PM;
using Android.OS;
using Silk.NET.Windowing.Sdl.Android;

namespace Net6Android
{
    // Tästä viimeisenä olevaa ScreenOrientation parametria muokkaamalla voit asettaa pelin olemaan vaaka-, tai pystyasennossa.
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : SilkActivity
    {
        // Näihin ei tarvitse koskea.
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ActionBar?.Hide();
            base.OnCreate(savedInstanceState);
        }
        protected override void OnRun()
        {
            Jypeli.Game.AssetManager = Assets;
            using (var game = new Pongaus())
                game.Run();
        }
    }
}