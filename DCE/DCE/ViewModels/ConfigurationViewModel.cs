using DCE.Models;
using DCE.Services.Interface;
using DCE.Views;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DCE.ViewModels
{
    [QueryProperty(nameof(WorkUnitParameter), nameof(WorkUnitParameter))]
    public class ConfigurationViewModel : BaseViewModel
    {
        readonly IConfigurationService _configurationService;

        public ICommand GetUserCommand { get; }
        public ICommand GetGroupCommand { get; }
        public ICommand GetGroupFoldersCommand { get; }
        public ICommand SaveConfigurationCommand { get; }
        public ICommand LogoutCommand { get; }

        string _workUnitParameter;
        public string WorkUnitParameter
        {
            get => _workUnitParameter;
            set
            {
                SetProperty(ref _workUnitParameter, value);
            }
        }

        WorkUnitFolder _workUnitFolder;
        public WorkUnitFolder WorkUnitFolder
        {
            get => _workUnitFolder;
            set
            {
                SetProperty(ref _workUnitFolder, value);
            }
        }

        private string _userLoggedIcon = "outline_edit_white_swipe";
        public string UserLoggedIcon
        {
            get => _userLoggedIcon;
            set
            {
                SetProperty(ref _userLoggedIcon, value);
            }
        }

        private string _groupNameIcon = "outline_edit_white_swipe";
        public string GroupNameIcon
        {
            get => _groupNameIcon;
            set
            {
                SetProperty(ref _groupNameIcon, value);
            }
        }

        private string _workUnitIcon = "outline_edit_white_swipe";
        public string WorkUnitIcon
        {
            get => _workUnitIcon;
            set
            {
                SetProperty(ref _workUnitIcon, value);
            }
        }


        private string _userLogged = "Usuário Logado";
        public string UserLogged
        {
            get => _userLogged;
            set
            {
                SetProperty(ref _userLogged, value);
            }
        }

        private string _groupName = "Grupo";
        public string GroupName
        {
            get => _groupName;
            set
            {
                SetProperty(ref _groupName, value);
            }
        }

        private string _workUnit = "Unidade";
        public string WorkUnit
        {
            get => _workUnit;
            set
            {
                SetProperty(ref _workUnit, value);
            }
        }


        private bool _isAuthenticating = App.IsLoggedIn;
        public bool IsAuthenticating
        {
            get => _isAuthenticating;
            set
            {
                SetProperty(ref _isAuthenticating, value);
            }
        }


        public ConfigurationViewModel()
        {
            _configurationService = DependencyService.Get<IConfigurationService>();

            GetUserCommand = new Command(async () =>
            {
                await GetUserInfo();
            });

            GetGroupCommand = new Command(async () =>
            {
                await GetGroupInfo();
            });

            GetGroupFoldersCommand = new Command(async () =>
            {
                await GetGroupFolders();
            });

            SaveConfigurationCommand = new Command(async () =>
            {
                await SaveConfiguration();
            });

            LogoutCommand = new Command(async () =>
            {
                await Signout();
            });
        }

        private async Task Signout()
        {
            await MSGraphService.Logout();

            UserLogged = "Usuário Logado";
            GroupName = "Grupo";
            WorkUnit = "Unidade";
            App.IsLoggedIn = false;
            ToastService.ToastLong("Logout efetuado com sucesso.");
            App.Current.MainPage = new AppShell();
        }

        async Task GetConfiguration()
        {
            try
            {
                IsBusy = true;

                var configs = await _configurationService.ListAsync();

                if (configs.Count() > 0)
                {
                    var cfg = configs.First();

                    UserLogged = cfg.UserLogged;
                    GroupName = cfg.GroupName;
                    WorkUnit = cfg.WorkUnit;
                }
            }
            catch (Exception)
            {
                ToastService.ToastLong("Aconteceu algo inesperado.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task SaveConfiguration()
        {
            try
            {
                IsBusy = true;

                var groupId = string.Empty;
                var workUnitId = string.Empty;

                if (App.Current.Properties.ContainsKey("userLogged"))
                    UserLogged = App.Current.Properties["userLogged"].ToString();

                if (App.Current.Properties.ContainsKey("groupId"))
                    groupId = App.Current.Properties["groupId"].ToString();

                if (App.Current.Properties.ContainsKey("folderId"))
                    workUnitId = App.Current.Properties["folderId"].ToString();


                var config = new Configuration(UserLogged, GroupName, groupId, WorkUnit, workUnitId);

                var result = await _configurationService.AddAsync(config);

                if (result)
                    ToastService.ToastLong("Salvo com sucesso.");
                else
                    ToastService.ToastLong("Não foi possivel salvar.");

                App.IsLoggedIn = false;

                App.Current.MainPage = new AppShell();
            }
            catch (Exception)
            {
                await DialogAlertService.DialogYesAsync("Erro", "Desculpe! Alguma coisa não saiu conforme esperado.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task GetUserInfo()
        {
            if (!(await MSGraphService.CheckAccountLoggedIn()))
            {
                ToastService.ToastLong("O login é necessário.");
                return;
            }

            try
            {
                var user = await MSGraphService.GetUserInfo();

                App.Current.Properties["userLogged"] = user;

                UserLogged = user;

                UserLoggedIcon = "outline_done_outline_white";
            }
            catch (Exception)
            {
                await DialogAlertService.DialogYesAsync("Aviso", "Aconteceu algo inesperado.");
                return;
            }
        }

        async Task GetGroupInfo()
        {
            if (!(await MSGraphService.CheckAccountLoggedIn()))
            {
                ToastService.ToastLong("O login é necessário.");
                return;
            }

            try
            {
                var groups = await MSGraphService.GetGroupInfo();

                var memberOfGroups = await MSGraphService.GetGroupMemberInfo();

                foreach (var item in groups)
                {
                    if (item.Description.Equals("DCE"))
                    {
                        if (memberOfGroups.Contains(item.Id))
                        {
                            GroupName = item.Description;
                            App.Current.Properties["groupName"] = item.Description;
                            App.Current.Properties["groupId"] = item.Id;

                            GroupNameIcon = "outline_done_outline_white";
                        }
                        else
                        {
                            ToastService.ToastLong("Usuário não tem acesso ao grupo.");
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {
                await DialogAlertService.DialogYesAsync("Aviso", "Aconteceu algo inesperado.");
                return;
            }
        }

        async Task GetGroupFolders()
        {
            try
            {
                if (!(await MSGraphService.CheckAccountLoggedIn()))
                {
                    ToastService.ToastLong("O login é necessário.");
                    return;
                }

                await Shell.Current.GoToAsync(nameof(ListDriveItemPage));

                if (App.Current.Properties.ContainsKey("workUnit"))
                {
                    WorkUnit = App.Current.Properties["workUnit"].ToString();
                    WorkUnitIcon = "outline_done_outline_white";
                }
            }
            catch (Exception)
            {
                ToastService.ToastLong("Aconteceu algo inesperado.");
            }

        }

        public async void OnAppearing()
        {
            if (string.IsNullOrEmpty(WorkUnitParameter))
            {
                await GetConfiguration();
            }
            else
            {
                var workUnitFoler = JsonConvert.DeserializeObject<WorkUnitFolder>(WorkUnitParameter);

                App.Current.Properties["workUnit"] = workUnitFoler.FolderName;
                App.Current.Properties["folderId"] = workUnitFoler.FolderId;
                WorkUnit = workUnitFoler.FolderName;
                WorkUnitIcon = "outline_done_outline_white";
            }
        }
    }
}
