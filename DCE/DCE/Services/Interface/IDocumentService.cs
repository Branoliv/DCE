using DCE.Models;
using System.Threading.Tasks;

namespace DCE.Services.Interface
{
    public interface IDocumentService : IBaseInterface<Document>
    {
        Task<Document> AddDocAsync(string containerNumber, string controlNumber, string sealNumber);
        Task<bool> ExistingContainerAndControlRegister(string ContainerNumber, string ControlNumber);
        Task DeleteThumbnails(string directoryName);
        Task DeleteDirectoryThumbnails(string directoryName);
        Task DeletePhotos(string folderName);
        Task DeleteDirectoryPhotos(string folderName);
        Task DeletePhoto(Photo photo);
        Task<bool> AddAsync(string containerNumber, string controlNumber, string sealNumber);
        Task<Document> GetDocumentByContainerNumberAndControlNumber(string containerNumber, string controlNumber);
    }
}
