using Xamarin.Forms;

namespace DCE.ViewModels
{
    [QueryProperty(nameof(Path), nameof(Path))]
    public class ImageViewViewModel : BaseViewModel
    {
        private string _path;

        public string Path
        {
            get => _path;
            set { SetProperty(ref _path , value); }
        }

        public ImageViewViewModel()
        {

        }

    }
}
