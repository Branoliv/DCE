
using DCE.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCE.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewDocumentPage : ContentPage
    {
        NewDocumentViewModel _viewModel;
        public NewDocumentPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new NewDocumentViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearingCommand.Execute(null);

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.OnDisappearingCommand.Execute(null);
        }
    }
}