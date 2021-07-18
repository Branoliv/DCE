using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCE.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCE.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListDriveItemPage : ContentPage
    {
       
        public ListDriveItemPage()
        {
            InitializeComponent();
            //BindingContext =  new ListDriveItemViewModel();
        }

        protected override void OnAppearing()
        {
            refreshView.IsRefreshing = true;
            base.OnAppearing();

        }
    }
}