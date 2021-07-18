using DCE.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCE.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UploadPage : ContentPage
    {
        readonly UploadViewModel UploadViewModel;
        public UploadPage()
        {
            InitializeComponent();
            BindingContext = UploadViewModel = new UploadViewModel();
        }

        protected override void OnAppearing()
        {
            refreshView.IsRefreshing = true;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UploadViewModel.OnDisappearing().Wait();
        }
    }
}