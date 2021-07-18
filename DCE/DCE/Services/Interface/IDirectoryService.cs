using System.Threading.Tasks;

namespace DCE.Services.Interface
{
    public interface IDirectoryService
    {
        Task<string> CreateFolder(string folderName);
        Task<bool> DeleteFolder(string fullPath);
        Task<string> GetPathFolder(string folderName);
        Task<string> GetDirectoryApplication();
    }
}
