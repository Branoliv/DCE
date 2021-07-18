using DCE.ViewModels;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCE.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        readonly HomeViewModel _viewModel;
        public HomePage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new HomeViewModel();
        }

        protected override void OnAppearing()
        {
            refreshView.IsRefreshing = true;
            base.OnAppearing();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = searchBar.Text;
            if (keyword.Length >= 1)
            {
                var filter = _viewModel.Documents.Where(d =>
                d.ContainerNumber.ToUpper().Contains(keyword.ToUpper()) ||
                d.ControlNumber.ToUpper().Contains(keyword.ToUpper()));

                documentsListView.ItemsSource = filter;
            }
            else
            {
                documentsListView.ItemsSource = _viewModel.Documents;
            }
        }
    }
}