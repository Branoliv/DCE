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
    public partial class AuthenticationPage : ContentPage
    {
        readonly AuthenticationViewModel _authenticationViewModel;
        public AuthenticationPage()
        {
            InitializeComponent();

            BindingContext = _authenticationViewModel = new AuthenticationViewModel();
        }

        protected override void OnAppearing()
        {
            //if (!App.IsLoggedIn)
            //    _authenticationViewModel.OnAppearing();

            base.OnAppearing();

        }
    }
}