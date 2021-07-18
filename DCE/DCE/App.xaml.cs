using DCE.Data;
using DCE.Data.Interface;
using DCE.Services;
using DCE.Services.Interface;
using DCE.Utils;
using DCE.ViewModels;
using Xamarin.Forms;

namespace DCE
{
    public partial class App : Application
    {
        public static bool IsLoggedIn = false;
        public static bool IsAccessOffline = false;


        public App()
        {
            InitializeComponent();

            DependencyService.Register<DocumentService>();
            DependencyService.Register<PhotoService>();
            DependencyService.Register<DialogAlertService>();
            DependencyService.Register<ConnectivityService>();
            DependencyService.Register<IMSGraphService, MSGraphService>();
            DependencyService.Register<ConfigurationService>();

            DependencyService.Register<DocumentRepository>();
            DependencyService.Register<PhotoRepository>();
            DependencyService.Register<ConfigurationRepository>();


            MainPage = new AppShell();
            var auth = new AuthenticationViewModel(true);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
