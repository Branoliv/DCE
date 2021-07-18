using DCE.Utils.Interfaces;
using DCE.Views;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DCE
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        readonly IToastService _toastService;
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(AuthenticationPage), typeof(AuthenticationPage));
            Routing.RegisterRoute(nameof(NewDocumentPage), typeof(NewDocumentPage));
            Routing.RegisterRoute(nameof(UploadPage), typeof(UploadPage));
            Routing.RegisterRoute(nameof(DocumentDetailPage), typeof(DocumentDetailPage));
            Routing.RegisterRoute(nameof(UpdateDocumentPage), typeof(UpdateDocumentPage));
            Routing.RegisterRoute(nameof(ImageViewPage), typeof(ImageViewPage));
            Routing.RegisterRoute(nameof(ErrorPage), typeof(ErrorPage));
            Routing.RegisterRoute(nameof(ConfigurationPage), typeof(ConfigurationPage));
            Routing.RegisterRoute(nameof(ListDriveItemPage), typeof(ListDriveItemPage));

            ConfigMenu.IsVisible = App.IsLoggedIn;
            _toastService = DependencyService.Get<IToastService>();
        }

        /// <summary>
        /// Chamado durante a navegação embora não tenha referências
        /// </summary>
        /// <param name="args"></param>
        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (args.Target.Location.OriginalString.Contains("Configuration") && !App.IsLoggedIn)
            {
                args.Cancel();
                _toastService.ToastLong("Login é necessário.");
                Shell.Current.GoToAsync("//Home");
            }
        }

        /// <summary>
        /// Chamado na seleção do menu embora não tenha referências
        /// </summary>
        /// <param name="propertyName"></param>
        protected async override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName.Equals("CurrentItem") && Device.RuntimePlatform == Device.Android)
            {
                FlyoutIsPresented = false;
                await Task.Delay(300);
            }
            base.OnPropertyChanged(propertyName);
        }
    }
}
