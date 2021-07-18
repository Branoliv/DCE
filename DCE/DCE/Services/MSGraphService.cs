using DCE.Services;
using DCE.Services.Interface;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(MSGraphService))]
namespace DCE.Services
{
    public class MSGraphService : IMSGraphService
    {
        static GraphServiceClient _graphClient;

        static IPublicClientApplication _pCA;

        readonly IConfigurationService _configurationService;

        public MSGraphService()
        {
            _configurationService = DependencyService.Get<IConfigurationService>();
        }

        public async Task<bool> CheckAccountLoggedIn()
        {
            if (_pCA != null)
            {
                var accounts = await _pCA.GetAccountsAsync();
                return accounts.Count() != 0;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CheckExistConfig()
        {
            var configs = await _configurationService.ListAsync();
            return configs.Count() != 0;
        }

        public async Task<DriveItem> CreateFolderOneDrive(string folderName)
        {
            var driveItem = new DriveItem
            {
                Name = folderName,
                Folder = new Folder { },
                AdditionalData = new Dictionary<string, object>()
                {
                    {"@microsoft.graph.conflictBehavior","rename" }
                }
            };

            var groupId = App.Current.Properties["groupId"].ToString();
            var folderid = App.Current.Properties["folderId"].ToString();

            return await _graphClient.Groups[groupId]
                .Drive
                .Items[folderid].Children.Request().AddAsync(driveItem);
        }

        public async Task<IDriveItemChildrenCollectionPage> ExistFolderOneDrive(string folderName)
        {
            var groupId = App.Current.Properties["groupId"].ToString();
            var folderid = App.Current.Properties["folderId"].ToString();

            return await _graphClient.Groups[groupId]
                 .Drive
                 .Items[folderid]
                 .Children
                 .Request()
                 .GetAsync();
        }

        public async Task<DriveItem> LoadOneDrive(string path, string fileName)
        {
            var groupId = App.Current.Properties["groupId"].ToString();
            var folderid = App.Current.Properties["folderId"].ToString();

            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                return await _graphClient.Groups[groupId]
                    .Drive
                    .Items[folderid]
                    .ItemWithPath(fileName)
                    .Content
                    .Request()
                    .PutAsync<DriveItem>(fileStream);
            };
        }

        public async Task<IGraphServiceGroupsCollectionPage> GetUser()
        {
            return await _graphClient.Groups
                .Request()
                .GetAsync();
        }

        public async Task<string> GetUserInfo()
        {
            var user = await _graphClient.Me
                .Request()
                .Select(u => new
                {
                    u.Id,
                    u.DisplayName,
                    u.Mail,
                    u.UserPrincipalName
                })
                .GetAsync();

            return user.DisplayName;
        }

        public async Task<IGraphServiceGroupsCollectionPage> GetGroupInfo()
        {
            return await _graphClient.Groups
                .Request()
                .GetAsync();
        }

        public async Task<IDriveItemChildrenCollectionPage> GetGroupFolders(string groupId)
        {
            return await _graphClient.Groups[groupId]
                 .Drive
                 .Root
                 .Children
                 .Request()
                 .GetAsync();
        }

        public async Task Logout()
        {
            var configs = await _configurationService.ListAsync();

            if (configs.Count() > 0)
            {
                foreach (var cfg in configs)
                {
                    await _configurationService.DeleteAsync(cfg.Id);
                }
            }

            var accounts = await _pCA.GetAccountsAsync();
            while (accounts.Any())
            {
                await _pCA.RemoveAsync(accounts.First());
                accounts = await _pCA.GetAccountsAsync();
            }
        }

        public async Task<GraphServiceClient> GetGraphServiceClient(string[] scopes, IPublicClientApplication pca)
        {
            _pCA = pca;
            var currentAccounts = await pca.GetAccountsAsync();

            // Initialize Graph client
            return _graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                  async (requestMessage) =>
                  {
                      var result = await pca.AcquireTokenSilent(scopes, currentAccounts.FirstOrDefault())
                          .ExecuteAsync();

                      requestMessage.Headers.Authorization =
                          new AuthenticationHeaderValue("Bearer", result.AccessToken);
                  }));
        }

        public async Task<IDirectoryObjectGetMemberGroupsCollectionPage> GetGroupMemberInfo()
        {
            return await _graphClient.Me
               .GetMemberGroups(false)
               .Request()
               .PostAsync();
        }
    }
}
