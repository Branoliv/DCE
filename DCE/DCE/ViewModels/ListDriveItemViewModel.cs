using DCE.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DCE.ViewModels
{
    public class ListDriveItemViewModel : BaseViewModel
    {
        public ICommand LoadCommand { get; }
        public ICommand FolderTapped { get; }


        private WorkUnitFolder _selectedFolder;
        public WorkUnitFolder SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                SetProperty(ref _selectedFolder, value);
                OnFolderSelectec(value);
            }
        }

       
        public ObservableCollection<WorkUnitFolder> WorkUnitFolders { get; set; }

        public ListDriveItemViewModel()
        {
            WorkUnitFolders = new ObservableCollection<WorkUnitFolder>();

            LoadCommand = new Command(async () =>
            {
                await GetGroupFolders();
            });

            FolderTapped = new Command<WorkUnitFolder>(OnFolderSelectec);
        }

        async Task GetGroupFolders()
        {
            try
            {
                IsBusy = true;

                var checkAccoutLoggedIn = await MSGraphService.CheckAccountLoggedIn();

                if (!checkAccoutLoggedIn)
                {
                    ToastService.ToastLong("O login é necessário.");
                    return;
                }

                string groupId = string.Empty;

                if (App.Current.Properties.ContainsKey("groupId"))
                {
                    groupId = App.Current.Properties["groupId"].ToString();
                }

                if (!string.IsNullOrEmpty(groupId))
                {
                    var folders = await MSGraphService.GetGroupFolders(groupId);

                    if (folders.Count() > 0)
                    {
                        foreach (var item in folders)
                        {
                            var folder = new WorkUnitFolder(item.Id, item.Name);

                            WorkUnitFolders.Add(folder);
                        }
                    }
                    else
                    {
                        ToastService.ToastLong("Não foi encotrada nenuma pasta.");
                    }
                }
            }
            catch (Exception)
            {
                ToastService.ToastLong("Não foi possível carregar a lista.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async void OnFolderSelectec(WorkUnitFolder workUnitFolder)
        {
            var workUnitParameter = JsonConvert.SerializeObject(workUnitFolder);
            await Shell.Current.GoToAsync($"..?{nameof(ConfigurationViewModel.WorkUnitParameter)}={workUnitParameter}");
        }
    }
}
