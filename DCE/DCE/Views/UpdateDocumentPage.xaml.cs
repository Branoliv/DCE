
using DCE.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCE.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateDocumentPage : ContentPage
    {
        UpdateDocumentViewModel _viewModel;
        public UpdateDocumentPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new UpdateDocumentViewModel();
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