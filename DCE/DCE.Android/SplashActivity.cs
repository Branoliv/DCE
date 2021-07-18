using Android.App;
using Android.Content.PM;
using Android.OS;

namespace DCE.Droid
{
    [Activity(Theme = "@style/MainTheme.Splash", Label = "DCE", Icon = "@mipmap/ic_launcher", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            StartActivity(typeof(MainActivity));
            Finish();

            OverridePendingTransition(0, 0);
        }
    }
}