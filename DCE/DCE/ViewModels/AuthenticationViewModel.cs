using DCE.Security;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DCE.ViewModels
{
    public class AuthenticationViewModel : BaseViewModel
    {
        public static object AuthUIParent = null;
        public static string iOSKeychainSecurityGroup = null;
        public static IPublicClientApplication PCA;
        public static GraphServiceClient GraphClient;

        private readonly string[] Scopes = OAuthSettings.Scopes.Split(' ');

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                SetProperty(ref _isRunning, value);
            }
        }

        private bool isSignedIn;
        public bool IsSignedIn
        {
            get { return isSignedIn; }
            set
            {
                isSignedIn = value;
                OnPropertyChanged("IsSignedIn");
                OnPropertyChanged("IsSignedOut");
            }
        }


        public AuthenticationViewModel(bool config = false)
        {
            var builder = PublicClientApplicationBuilder
              .Create(OAuthSettings.ApplicationId)
              .WithRedirectUri(OAuthSettings.RedirectUri);
            //.WithTenantId(OAuthSettings.TenantId)

            if (!string.IsNullOrEmpty(iOSKeychainSecurityGroup))
            {
                builder = builder.WithIosKeychainSecurityGroup(iOSKeychainSecurityGroup);
            }

            PCA = builder.Build();

            AuthenticationAsync(config).ConfigureAwait(false);
        }

        public async Task SignOutAsync()
        {
            var accounts = await PCA.GetAccountsAsync();
            while (accounts.Any())
            {
                await PCA.RemoveAsync(accounts.First());
                accounts = await PCA.GetAccountsAsync();
            }
        }

        private async Task InitializeGraphClientAsync()
        {
            var currentAccounts = await PCA.GetAccountsAsync();

            try
            {
                if (currentAccounts.Count() > 0)
                {

                    if (!ConnectivityService.IsNetworkAccess())
                        await DialogAlertService.DialogYesAsync("Alerta", "Não conexão disponivel com a internet");


                    GraphClient = await MSGraphService.GetGraphServiceClient(Scopes, PCA);

                    IsSignedIn = true;
                }
                else
                {
                    IsSignedIn = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(" ERRO =====> Failed to initialized graph client.");
                Debug.WriteLine($"ERRO =====> Accounts in the msal cache: {currentAccounts.Count()}.");
                Debug.WriteLine($"ERRO =====> See exception message for details: {ex.Message}");
            }
        }


        public async Task AuthenticationAsync(bool config)
        {
            if (!config)
            {
                await PromptAuthenticationAsync();
            }
            else
            {
                await SilentAuthenticationAsync();
            }
        }

        async Task PromptAuthenticationAsync()
        {
            try
            {
                IsRunning = true;
                SignOutAsync().Wait();

                //var interactiveRequest = PCA.AcquireTokenInteractive(Scopes);
                var interactiveRequest = PCA.AcquireTokenInteractive(Scopes).WithPrompt(Microsoft.Identity.Client.Prompt.ForceLogin);

                if (AuthUIParent != null)
                {
                    interactiveRequest = interactiveRequest
                        .WithParentActivityOrWindow(AuthUIParent);
                }

                var interactiveAuthResult = await interactiveRequest
                        .WithUseEmbeddedWebView(true)
                        .ExecuteAsync();

                await InitializeGraphClientAsync();

                App.IsLoggedIn = await VerificationMemberGroupAsync();

                if (!App.IsLoggedIn)
                {
                    await MSGraphService.Logout();
                    ToastService.ToastLong("Usuário não é membro do grupo DCE.\nEntre em contato com administrador.");

                }
            }
            catch (MsalException mEx)
            {
                App.IsLoggedIn = false;
                if (mEx.ErrorCode.Equals("access_denied") || mEx.ErrorCode.Equals("authentication_canceled"))
                {
                    ToastService.ToastShort("Não foi possível realizar o login.");
                }
            }
            catch (Exception ex)
            {
                App.IsLoggedIn = false;
                Debug.WriteLine("ERRO ===========> Authentication failed. See exception messsage for more details: " + ex.Message);
            }
            finally
            {
                IsRunning = false;
                App.Current.MainPage = new AppShell();
            }
        }

        async Task SilentAuthenticationAsync()
        {
            try
            {
                var accounts = await PCA.GetAccountsAsync();
                var silentAuthResult = await PCA
                .AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                .ExecuteAsync();

                await InitializeGraphClientAsync();
            }
            catch (MsalException mEx)
            {
                App.IsLoggedIn = false;

                if (mEx.ErrorCode.Equals("access_denied") || mEx.ErrorCode.Equals("authentication_canceled"))
                {
                    ToastService.ToastShort("Não foi possível realizar o login.");
                }
            }
            catch (Exception ex)
            {
                App.IsLoggedIn = false;

                Debug.WriteLine("ERRO ===========> Authentication failed. See exception messsage for more details: " + ex.Message);
            }
            finally
            {
                IsRunning = false;
                App.Current.MainPage = new AppShell();
            }
        }

        async Task<bool> VerificationMemberGroupAsync()
        {
            bool IsMemberGroup = false;
            var groups = await MSGraphService.GetGroupInfo();
            var memberOfGroups = await MSGraphService.GetGroupMemberInfo();

            foreach (var group in groups)
            {
                if (group.Description.Equals("DCE"))
                {
                    if (memberOfGroups.Contains(group.Id))
                        IsMemberGroup = true;
                }
            }

            return IsMemberGroup;
        }
    }
}
