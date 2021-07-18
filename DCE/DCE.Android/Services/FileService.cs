using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Widget;

using DCE.Droid.Services;
using DCE.Services.Interface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace DCE.Droid.Services
{
    public class FileService : IFileService
    {
        public async Task<bool> CompressImage(string path)
        {
            try
            {
                using (Bitmap bitmap = await BitmapFactory.DecodeFileAsync(path))
                {
                    using (System.IO.Stream stream = System.IO.File.Create(path))
                    {
                        return await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 30, stream);
                    };
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<byte[]> CreateThumbnailPhoto(byte[] imageData, float width, float height, string path)
        {
            using Bitmap originalImage = await BitmapFactory.DecodeFileAsync(path);
            float newHeight = 0;
            float newWidth = 0;

            var originalHeight = originalImage.Height;
            var originalWidth = originalImage.Width;

            if (originalHeight > originalWidth)
            {
                newHeight = height;
                float ratio = originalHeight / height;
                newWidth = originalWidth / ratio;
            }
            else
            {
                newWidth = width;
                float ratio = originalWidth / width;
                newHeight = originalHeight / ratio;
            }

            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, true);

            originalImage.Recycle();

            using MemoryStream ms = new MemoryStream();
            resizedImage.Compress(Bitmap.CompressFormat.Png, 100, ms);

            resizedImage.Recycle();

            return await Task.FromResult(ms.ToArray());
        }

        public async Task<bool> DeleteFile(string pathFile)
        {
            bool result = false;

            try
            {
                var exist = File.Exists(pathFile);

                if (exist)
                {
                    File.Delete(pathFile);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message.ToString(), ToastLength.Long).Show();
            }

            return await Task.FromResult(result);
        }

        public async Task<string[]> GetFiles(string folderName)
        {
            string[] files = new string[0];
            Context context = Application.Context;

            try
            {
                var status = await Permissions.RequestAsync<Permissions.StorageWrite>().ConfigureAwait(false);

                if (status.Equals(PermissionStatus.Granted))
                {
                    string path = context.GetExternalFilesDir(string.Empty).AbsolutePath;

                    var rootPath = System.IO.Path.Combine(path, folderName);

                    bool exist = Directory.Exists(rootPath);

                    if (exist)
                    {
                        files = Directory.GetFiles(rootPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message.ToString(), ToastLength.Long).Show();
            }

            return await Task.FromResult(files);
        }

        public async Task<IEnumerable<string>> ListFiles(string pathDirectory)
        {
            var listFiles = Directory.EnumerateFiles(pathDirectory);

            return await Task.FromResult(listFiles);
        }
    }
}