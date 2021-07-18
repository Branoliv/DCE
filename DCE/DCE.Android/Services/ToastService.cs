using Android.App;
using Android.Widget;
using DCE.Droid.Services;
using DCE.Utils.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ToastService))]
namespace DCE.Droid.Services
{
    public class ToastService : IToastService
    {
        public void ToastLong(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ToastShort(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}