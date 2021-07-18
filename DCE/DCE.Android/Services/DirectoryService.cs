using Android.App;
using Android.Content;
using Android.Widget;
using DCE.Droid.Services;
using DCE.Services.Interface;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(DirectoryService))]
namespace DCE.Droid.Services
{
    public class DirectoryService : IDirectoryService
    {
        public async Task<string> CreateFolder(string folderName)
        {
            string rootPath = string.Empty;
            Context context = Android.App.Application.Context;

            try
            {
                var status = await Permissions.RequestAsync<Permissions.StorageWrite>().ConfigureAwait(false);

                if (status.Equals(PermissionStatus.Granted))
                {
                    string path = context.GetExternalFilesDir(string.Empty).AbsolutePath;

                    rootPath = Path.Combine(path, folderName);

                    bool exist = Directory.Exists(rootPath);

                    if (!exist)
                    {
                        Directory.CreateDirectory(rootPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                GC.Collect();
            }

            return await Task.FromResult(rootPath);
        }

        public async Task<bool> DeleteFolder(string pathFolder)
        {
            bool result = false;

            try
            {
                var exist = Directory.Exists(pathFolder);

                if (exist)
                {
                    Directory.Delete(pathFolder);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message.ToString(), ToastLength.Long).Show();
            }

            return await Task.FromResult(result);
        }

        public Task<string> GetDirectoryApplication()
        {
            string path = FileSystem.AppDataDirectory;

            return Task.FromResult(path);
        }

        public async Task<string> GetPathFolder(string folderName)
        {
            string rootPath = string.Empty;
            Context context = Android.App.Application.Context;

            string path = context.GetExternalFilesDir(string.Empty).AbsolutePath;

            rootPath = Path.Combine(path, folderName);

            bool exist = Directory.Exists(rootPath);

            if (!exist)
            {
                return await Task.FromResult(string.Empty);
            }

            return await Task.FromResult(rootPath);
        }
    }
}