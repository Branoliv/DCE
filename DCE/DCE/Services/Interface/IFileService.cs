using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace DCE.Services.Interface
{
    public interface IFileService
    {
        Task<bool> CompressImage(string path);
        Task<byte[]> CreateThumbnailPhoto(byte[] imageData, float width, float height, string path);
        Task<bool> DeleteFile(string pathFile);
        Task<string[]> GetFiles(string folderName);
        Task<IEnumerable<string>> ListFiles(string pathDirectory);
    }
}
