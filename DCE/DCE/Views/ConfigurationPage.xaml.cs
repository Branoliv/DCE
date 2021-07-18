using DCE.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCE.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage : ContentPage
    {
        readonly ConfigurationViewModel _configurationViewModel;
        public ConfigurationPage()
        {
            InitializeComponent();

            BindingContext = _configurationViewModel = new ConfigurationViewModel();
        }

        protected override void OnAppearing()
        {
            _configurationViewModel.OnAppearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            App.IsLoggedIn = false;
            base.OnDisappearing();
        }
    }
}