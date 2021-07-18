using DCE.Data.Interface;
using DCE.Models;
using DCE.Services.Interface;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DCE.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepository _photoRepository;
        readonly IFileService _fileService;
        readonly IDirectoryService _directoryService;

        public PhotoService()
        {
            _photoRepository = DependencyService.Get<IPhotoRepository>();
            _directoryService = DependencyService.Get<IDirectoryService>();
            _fileService = DependencyService.Get<IFileService>();
        }
        public async Task<bool> AddAsync(Photo obj)
        {
            await _photoRepository.AddAsync(obj);
            return await Task.FromResult(true);
        }

        public async Task<bool> AddAsync(Document document, string directoryName)
        {
            try
            {
                var pathFolder = await _directoryService.CreateFolder(Path.Combine("Pictures", directoryName));
                var photosDirectory = await _fileService.ListFiles(pathFolder);

                foreach (var file in photosDirectory)
                {
                    var photoName = file.Replace($"{pathFolder}/", "");
                    var thumbPath = Path.Combine(pathFolder, "thumb", photoName);

                    var photo = new Photo(photoName, file, thumbPath, document.Id, DateTime.Now);

                    await _photoRepository.AddAsync(photo);
                }

                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Photo> CapturePhotoCrossMediaAsync(string directoryName, string fileName, bool isUpdate)
        {
            await _directoryService.CreateFolder(Path.Combine("Pictures", directoryName));
            await _directoryService.CreateFolder(Path.Combine("Pictures", directoryName, "thumb"));

            var mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                Directory = directoryName,
                PhotoSize = PhotoSize.Medium,
                Name = fileName,
                CompressionQuality = 40,
            });

            if (mediaFile == null)
                return await Task.FromResult<Photo>(null);

            var pathPhoto = mediaFile.Path;

            var thumb = await CreateThumbnail(pathPhoto, fileName);
            var photo = new Photo(fileName, pathPhoto, thumb);

            if (isUpdate)
                photo = await UpdatePhoto(photo, directoryName);

            mediaFile.Dispose();
            return photo;
        }

        public async Task<string> CreateThumbnail(string defaultPhotoPath, string fileName)
        {
            var defaultDirectoryPath = defaultPhotoPath.Replace(fileName, "");
            var pathThumbnail = Path.Combine(defaultDirectoryPath, "thumb", fileName);

            using (FileStream fs = new FileStream(defaultPhotoPath, FileMode.Open))
            {
                byte[] vs = new byte[fs.Length];
                var thumbBytes = await _fileService.CreateThumbnailPhoto(vs, 112, 150, defaultPhotoPath);

                using (var nfs = new FileStream(pathThumbnail, FileMode.Create, FileAccess.Write))
                {
                    nfs.Write(thumbBytes, 0, thumbBytes.Length);
                }
            }

            return await Task.FromResult(pathThumbnail);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _photoRepository.DeleteAsync(id);
        }

        public async Task<Photo> GetByIdAsync(Guid id)
        {
            return await _photoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Photo>> ListAsync()
        {
            return await _photoRepository.ListAsync();
        }

        public async Task<List<Photo>> ListPhotoAsync(Guid documentId)
        {
            return await _photoRepository.ListPhotoAsync(documentId);
        }

        public async Task<Photo> TakeGalleriaPhotoAsync(string directoryName, string fileName, bool isUpdate)
        {
            var directoryPathDefault = await _directoryService.CreateFolder(Path.Combine("Pictures", directoryName));
            await _directoryService.CreateFolder(Path.Combine("Pictures", directoryName, "thumb"));

            var mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium,
                CompressionQuality = 40
            });

            if (mediaFile == null)
                return await Task.FromResult<Photo>(null);

            var pathPhoto = mediaFile.Path;
            var newNamePhoto = Path.Combine(directoryPathDefault, fileName);
              
            File.Move(pathPhoto, newNamePhoto);

            var thumb = await CreateThumbnail(Path.Combine(directoryPathDefault, fileName), fileName);

            var photo = new Photo(fileName, newNamePhoto, thumb);

            if (isUpdate)
                photo = await UpdatePhoto(photo, directoryName);

            mediaFile.Dispose();
            return photo;
        }

        public async Task<bool> UpdateAsync(Photo obj)
        {
            return await _photoRepository.AddAsync(obj);
        }

        public async Task<Photo> UpdatePhoto(Photo photo, string directoryName)
        {
            var directoryPathDefault = await _directoryService.CreateFolder(Path.Combine("Pictures",  directoryName));
            var directoryPathThumbnail = await _directoryService.CreateFolder(Path.Combine("Pictures", directoryName, "thumb"));

            var newPhoto = Path.Combine(directoryPathDefault, photo.Name);
            var newThumb = Path.Combine(directoryPathThumbnail, photo.Name);

            File.Move(photo.Path, newPhoto);
            File.Move(photo.PathThumbnail, newThumb);

            return new Photo(photo.Name, newPhoto, newThumb);
        }
    }
}
