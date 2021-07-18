using DCE.Models;
using System.Threading.Tasks;

namespace DCE.Data.Interface
{
    public interface IDocumentRepository : IBaseRepository<Document>
    {
        Task<Document> AddDocumentAsync(Document document);
        Task<bool> ExistingContainerAndControlRegister(string containerNumber, string controlNumber);
        Task<Document> GetDocumentByContainerNumberAndControlNumber(string containerNumber, string controlNumber);
    }
}
