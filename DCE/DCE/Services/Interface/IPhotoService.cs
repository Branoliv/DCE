using DCE.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DCE.Services.Interface
{
    public interface IPhotoService : IBaseInterface<Photo>
    {
        Task<Photo> TakeGalleriaPhotoAsync(string directoryName, string fileName, bool isUpdate = false);
        Task<Photo> CapturePhotoCrossMediaAsync(string directoryName, string photoName, bool isUpdate = false);
        Task<List<Photo>> ListPhotoAsync(Guid documentId);
        Task<string> CreateThumbnail(string defaultPhotoPath, string fileName);
        Task<bool> AddAsync(Document document, string directoryName);
        Task<Photo> UpdatePhoto(Photo photo, string directoryName);
    }
}
