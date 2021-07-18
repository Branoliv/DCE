using DCE.Data.Interface;
using DCE.Models;
using DCE.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DCE.Services
{
    public class DocumentService : IDocumentService
    {
        readonly IDocumentRepository _documentRepository;
        readonly IDirectoryService _directoryService;
        readonly IFileService _fileService;

        public DocumentService()
        {
            _documentRepository = DependencyService.Get<IDocumentRepository>();
            _directoryService = DependencyService.Get<IDirectoryService>();
            _fileService = DependencyService.Get<IFileService>();
        }


        public async Task<Document> AddDocAsync(string containerNumber, string controlNumber, string sealNumber)
        {
            var fileName = $"{containerNumber.ToUpper()}_{controlNumber.ToUpper()}";
            var pathFolder = await _directoryService.CreateFolder(Path.Combine("Pictures", fileName));

            var photosDirectory = await _fileService.ListFiles(pathFolder);

            var document = new Document(
                controlNumber: controlNumber,
                containerNumber: containerNumber.ToUpper(),
                sealNumber: sealNumber,
                inclusionDate: DateTime.Now,
                photoCounter: photosDirectory.Count(),
                copied: false);

            return await _documentRepository.AddDocumentAsync(document);

        }

        public async Task<bool> AddAsync(Document obj)
        {
            await _documentRepository.AddAsync(obj);

            return await Task.FromResult(true);
        }

        public async Task<bool> AddAsync(string containerNumber, string controlNumber, string sealNumber)
        {
            var fileName = $"{containerNumber.ToUpper()}_{controlNumber.ToUpper()}";
            var pathFolder = await _directoryService.CreateFolder(Path.Combine("Pictures", fileName));

            var photosDirectory = await _fileService.ListFiles(pathFolder);

            var document = new Document(
                containerNumber: containerNumber.ToUpper(),
                controlNumber: controlNumber,
                sealNumber: sealNumber,
                inclusionDate: DateTime.Now,
                photoCounter: photosDirectory.Count(),
                copied: false
            );

            return await _documentRepository.AddAsync(document);

        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _documentRepository.DeleteAsync(id);

            return await Task.FromResult(true);
        }

        public async Task DeleteDirectoryPhotos(string directoryName)
        {
            var pathDirectoryPhotos = await _directoryService.GetPathFolder(Path.Combine("Pictures", directoryName));
            await _directoryService.DeleteFolder(pathDirectoryPhotos);
        }

        public async Task DeleteDirectoryThumbnails(string directoryName)
        {
            var directoryPathThumb = await _directoryService.GetPathFolder(Path.Combine("Pictures", directoryName, "thumb"));
            await _directoryService.DeleteFolder(directoryPathThumb);
        }

        public async Task DeletePhoto(Photo photo)
        {
            await _fileService.DeleteFile(photo.PathThumbnail);
            await _fileService.DeleteFile(photo.Path);
        }

        public async Task DeletePhotos(string directoryName)
        {
            var pathDirectoryPhotos = await _directoryService.GetPathFolder(Path.Combine("Pictures", directoryName));

            if (string.IsNullOrEmpty(pathDirectoryPhotos))
            {
                return;
            }

            var filesPath = await _fileService.GetFiles(pathDirectoryPhotos);

            foreach (var file in filesPath)
            {
                await _fileService.DeleteFile(file);
            }
        }

        public async Task DeleteThumbnails(string folderName)
        {
            var directoryPathThumb = await _directoryService.GetPathFolder(Path.Combine("Pictures", folderName, "thumb"));

            if (string.IsNullOrEmpty(directoryPathThumb))
            {
                return;
            }

            var filesThumb = await _fileService.GetFiles(directoryPathThumb);

            foreach (var file in filesThumb)
            {
                await _fileService.DeleteFile(file);
            }
        }

        public async Task<bool> ExistingContainerAndControlRegister(string containerNumber, string controlNumber)
        {
            return await _documentRepository.ExistingContainerAndControlRegister(containerNumber, controlNumber);
        }

        public async Task<Document> GetByIdAsync(Guid id)
        {
            var doc = await _documentRepository.GetByIdAsync(id);
            return doc;
        }

        public async Task<Document> GetDocumentByContainerNumberAndControlNumber(string containerNumber, string controlNumber)
        {
            return await _documentRepository.GetDocumentByContainerNumberAndControlNumber(containerNumber, controlNumber);
        }

        public async Task<IEnumerable<Document>> ListAsync()
        {
            return await _documentRepository.ListAsync();
        }

        public async Task<bool> UpdateAsync(Document obj)
        {
            var doc = await _documentRepository.UpdateAsync(obj);

            if (!doc)
                return await Task.FromResult(false);
            else
                return await Task.FromResult(true);
        }

    }
}
