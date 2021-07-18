using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace DCE.Services.Interface
{
    public interface IMSGraphService
    {
        Task<GraphServiceClient> GetGraphServiceClient(string[] scopes, IPublicClientApplication PCA);
        Task<bool> CheckExistConfig();
        Task<bool> CheckAccountLoggedIn();
        Task<IGraphServiceGroupsCollectionPage> GetUser();
        Task<DriveItem> CreateFolderOneDrive(string folderName);
        Task<IDriveItemChildrenCollectionPage> ExistFolderOneDrive(string folderName);
        Task<DriveItem> LoadOneDrive(string path, string fileName);
        Task<string> GetUserInfo();
        Task<IGraphServiceGroupsCollectionPage> GetGroupInfo();
        Task<IDirectoryObjectGetMemberGroupsCollectionPage> GetGroupMemberInfo();
        Task<IDriveItemChildrenCollectionPage> GetGroupFolders(string groupId);
        Task Logout();
    }
}
