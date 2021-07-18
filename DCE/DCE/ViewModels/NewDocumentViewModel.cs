using DCE.Models;
using DCE.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DCE.ViewModels
{
    public class NewDocumentViewModel : BaseViewModel
    {
        public ICommand SaveDocumentCommand { get; }
        public ICommand CapturePhotoCommand { get; }
        public ICommand TakeGalleriaPhotoCommand { get; }
        public ICommand ImageViewCommand { get; }
        public ICommand DeleteImageCommand { get; }
        public ICommand ResetNewDocumentCommand { get; }
        public Command OnAppearingCommand { get; set; }
        public Command OnDisappearingCommand { get; set; }
        private Photo Photo { get; set; }


        private bool _finishedContainerStuffing = false;
        public bool FinishedContainerStuffing
        {
            get => _finishedContainerStuffing;
            set
            {
                SetProperty(ref _finishedContainerStuffing, value);
            }
        }

        private bool _savedDocument = false;
        public bool SavedDocument
        {
            get => _savedDocument;
            set
            {
                SetProperty(ref _savedDocument, value);
            }
        }

        private string _navigationDirection;
        public string NavigationDirection
        {
            get => _navigationDirection;
            set
            {
                SetProperty(ref _navigationDirection, value);
            }
        }

        private string _containerNumber;
        public string ContainerNumber
        {
            get => _containerNumber;
            set
            {
                string valueWithoutSpace = value;
                _containerNumber = valueWithoutSpace.Replace(" ", string.Empty);
                SetProperty(ref _containerNumber, _containerNumber);
            }
        }

        private string _controlNumber;
        public string ControlNumber
        {
            get => _controlNumber;
            set
            {
                string valueWithoutSpace = value;
                _controlNumber = valueWithoutSpace.Replace(" ", string.Empty);
                SetProperty(ref _controlNumber, _controlNumber);
            }
        }

        private string _sealNumber;
        public string SealNumber
        {
            get => _sealNumber;
            set
            {
                string valueWithoutSpace = value;
                _sealNumber = valueWithoutSpace.Replace(" ", string.Empty);
                SetProperty(ref _sealNumber, _sealNumber);
            }
        }

        private int _photoCounter;
        public int PhotoCounter
        {
            get => _photoCounter;
            set
            {
                SetProperty(ref _photoCounter, value);
            }
        }

        private bool _enableEntry = true;
        public bool EnableEntry
        {
            get => _enableEntry;
            set
            {
                SetProperty(ref _enableEntry, value);
            }
        }

        private ObservableCollection<Photo> _photos;
        public ObservableCollection<Photo> Photos
        {
            get => _photos;
            set
            {
                SetProperty(ref _photos, value);
                InputIsUpgradeable();
            }
        }


        public NewDocumentViewModel()
        {
            Photos = new ObservableCollection<Photo>();
            SaveDocumentCommand = new Command(async () => await SaveDocument());
            CapturePhotoCommand = new Command(async () => await CapturePhotoCrossMedia());
            ResetNewDocumentCommand = new Command(async () => await ResetNewDocument());
            TakeGalleriaPhotoCommand = new Command(async () => await TakeGalleriaPhoto());
            DeleteImageCommand = new Command<Photo>(async (photo) => await DeleteImage(photo));

            ImageViewCommand = new Command<Photo>(async (img) =>
            {
                await ImageView(img);
            });

            OnDisappearingCommand = new Command(() => OnDisappearing());
            OnAppearingCommand = new Command(() => OnAppearing());
        }


        async Task DeleteImage(Photo img)
        {
            try
            {
                IsEnable = false;
                var result = await DialogAlertService.DialogYesNoAsync("Alerta!", "Deseja apagar a foto?");

                if (result)
                {
                    await DocumentService.DeletePhoto(img);
                    Photos.Remove(img);
                    PhotoCounter = Photos.Count;
                }
            }
            catch
            {
                await DialogAlertService.DialogYesAsync("Erro", "Desculpe alguma coisa saiu errada.");
            }
            finally
            {
                IsEnable = true;
            }
        }

        async Task SaveDocument()
        {
            try
            {
                IsBusy = true;
                IsEnable = true;

                if (Photos.Count() <= 0)
                {
                    await DialogAlertService.DialogYesAsync("Alerta!", "É necessário pelo menos uma foto.");
                    return;
                }

                var documentExit = await DocumentService.ExistingContainerAndControlRegister(ContainerNumber, ControlNumber);

                if (documentExit)
                {
                    ToastService.ToastLong("Documento já existe.");
                    return;
                }

                var responseAddDocument = await DocumentService.AddDocAsync(ContainerNumber, ControlNumber, SealNumber);

                if (responseAddDocument != null)
                {
                    var responseAddPhotos = await PhotoService.AddAsync(responseAddDocument, GetFolderName());

                    if (responseAddPhotos)
                    {
                        SavedDocument = true;
                        NavigationDirection = "Save";
                    }

                    if (responseAddPhotos && FinishedContainerStuffing)
                    {
                        if (await MSGraphService.CheckAccountLoggedIn() && await MSGraphService.CheckExistConfig())
                            await UploadDocument(responseAddDocument);
                    }
                }

                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception)
            {
                IsBusy = false;
                await DialogAlertService.DialogYesAsync("Erro", "Alguma coisa não saiu conforme esperado.");

                if (SavedDocument)
                    await Shell.Current.Navigation.PopAsync();
            }
            finally
            {
                InputIsUpgradeable();
                IsBusy = false;
                IsEnable = false;
            }
        }

        async Task CapturePhotoCrossMedia()
        {
            try
            {
                IsEnable = false;
                var exist = await DocumentService.ExistingContainerAndControlRegister(ContainerNumber, ControlNumber);

                if (exist)
                {
                    await DialogAlertService.DialogYesAsync("Informação", "Existe um registro com o número de container e controle informado.");
                    return;
                }

                if (Photos.Count >= 12)
                {
                    await DialogAlertService.DialogYesAsync("Informação", "Limite de 12 fotos atingido.");
                    return;
                }

                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (!status.Equals(PermissionStatus.Granted))
                {
                    await DialogAlertService.DialogYesAsync("Alerta", "A autorização é necessária para utilizar a camêra.");
                    return;
                }

                if (!ValidateButton())
                {
                    ToastService.ToastLong("O número de container e controle são obrigatórios.");
                    return;
                }

                var fileName = CreateFileName();

                var photo = await PhotoService.CapturePhotoCrossMediaAsync(GetFolderName(), fileName);

                if (photo == null)
                {
                    ToastService.ToastShort("Foto não tirada.");
                    return;
                }

                Photos.Add(photo);
                PhotoCounter = Photos.Count;
            }
            catch (Exception ex)
            {
                await DialogAlertService.DialogYesAsync("Alerta", ex.Message);
            }
            finally
            {
                InputIsUpgradeable();
                GC.Collect();
                IsEnable = true;
            }
        }

        async Task TakeGalleriaPhoto()
        {
            try
            {
                IsEnable = false;
                var exist = await DocumentService.ExistingContainerAndControlRegister(ContainerNumber, ControlNumber);

                if (exist)
                {
                    await DialogAlertService.DialogYesAsync("Informação", "Existe um registro com o número de container e controle informado.");
                    return;
                }

                if (Photos.Count >= 12)
                {
                    await DialogAlertService.DialogYesAsync("Informação", "Limite de 12 fotos atingido.");
                    return;
                }

                if (!ValidateButton())
                {
                    ToastService.ToastLong("O número de container e controle são obrigatórios.");
                    return;
                }

                var fileName = CreateFileName();

                var photo = await PhotoService.TakeGalleriaPhotoAsync(GetFolderName(), fileName);

                if (photo == null)
                {
                    ToastService.ToastShort("Foto não selecionada.");
                    return;
                }

                Photos.Add(photo);
                PhotoCounter = Photos.Count;
            }
            catch (Exception ex)
            {
                await DialogAlertService.DialogYesAsync("Alerta", ex.Message);
            }
            finally
            {
                InputIsUpgradeable();
                IsEnable = true;
            }
        }

        string GetFolderName()
        {
            return $"{ContainerNumber.ToUpper()}_{ControlNumber.ToUpper()}";
        }

        string CreateFileName()
        {
            var date = DateTime.Now;

            var index = $"{date.Year}{date.Month}{date.Day}{date.Hour}{date.Minute}{date.Second}";
            return $"{ContainerNumber}_{ControlNumber}_{index}.jpg";
        }

        async Task ImageView(Photo photo)
        {
            NavigationDirection = "ViewImage";
            Photo = new Photo(photo.Name, photo.Path, photo.PathThumbnail);
            await Shell.Current.GoToAsync($"{nameof(ImageViewPage)}?{nameof(ImageViewViewModel.Path)}={photo.Path}");
        }

        async Task ResetNewDocument()
        {
            if (!string.IsNullOrEmpty(ContainerNumber) && !string.IsNullOrEmpty(ControlNumber))
            {
                var result = await DialogAlertService.DialogYesNoAsync("Alerta!", "Deseja limpara o formulário?");

                if (result)
                {
                    await DeleteDirectorysAndPhotos();

                    ContainerNumber = string.Empty;
                    ControlNumber = string.Empty;
                    SealNumber = string.Empty;
                    Photos.Clear();
                    PhotoCounter = 0;
                    InputIsUpgradeable();
                }
            }
        }

        async Task DeleteDirectorysAndPhotos()
        {
            await DocumentService.DeleteThumbnails(GetFolderName());
            await DocumentService.DeleteDirectoryThumbnails(GetFolderName());

            await DocumentService.DeletePhotos(GetFolderName());
            await DocumentService.DeleteDirectoryPhotos(GetFolderName());
        }

        public async Task UploadDocument(Document document)
        {
            List<Document> copiedDocuments = new List<Document>();
            List<Photo> copiedPhotos = new List<Photo>();

            var folderName = $"{document.ContainerNumber}_{document.ControlNumber}";
            var existFolderOneDrive = await MSGraphService.ExistFolderOneDrive(folderName);

            if (existFolderOneDrive.Count > 0 && !existFolderOneDrive[0].Name.Contains(folderName))
            {
                await MSGraphService.CreateFolderOneDrive(folderName);
            }
            else if (existFolderOneDrive.Count == 0)
            {
                await MSGraphService.CreateFolderOneDrive(folderName);
            }

            var photos = await PhotoService.ListPhotoAsync(document.Id);

            foreach (var photo in photos)
            {
                var resultUpload = await MSGraphService.LoadOneDrive(photo.Path, Path.Combine(folderName, photo.Name));

                var responsePhotoUpload = resultUpload.AdditionalData["statusCode"].Equals("Created");

                if (responsePhotoUpload)
                    copiedPhotos.Add(photo);
            }

            foreach (var photo in copiedPhotos)
            {
                await DocumentService.DeletePhoto(photo);
                await PhotoService.DeleteAsync(photo.Id);
            }

            var resultCopied = photos.Except(copiedPhotos);

            if (resultCopied.Count() == 0)
            {
                var coumentUpdateResult = await DocumentService.UpdateAsync(document);

                if (coumentUpdateResult)
                {
                    copiedDocuments.Add(document);
                }
            }

            foreach (var doc in copiedDocuments)
            {

                var resultDocumentDelete = await DocumentService.DeleteAsync(doc.Id);

                if (resultDocumentDelete)
                {
                    var directoryName = $"{doc.ContainerNumber}_{doc.ControlNumber}";

                    await DocumentService.DeleteDirectoryThumbnails(directoryName);
                    await DocumentService.DeleteDirectoryPhotos(directoryName);
                }
            }
        }


        #region NAVIGATION_VALIDATION

        void OnAppearing()
        {
            Shell.Current.Navigating += Current_Navigating;
        }

        void OnDisappearing()
        {
            Shell.Current.Navigating -= Current_Navigating;
        }

        async void Current_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.CanCancel)
            {
                e.Cancel();
                await GoBack();
            }
        }

        async Task GoBack()
        {
            var formValidation = FormValidation();

            switch (NavigationDirection)
            {
                case "Save":
                    Shell.Current.Navigating -= Current_Navigating;
                    NavigationDirection = string.Empty;
                    await Shell.Current.GoToAsync("..", true);
                    break;

                case "ViewImage":
                    Shell.Current.Navigating -= Current_Navigating;
                    NavigationDirection = string.Empty;
                    await Shell.Current.GoToAsync($"{nameof(ImageViewPage)}?{nameof(ImageViewViewModel.Path)}={Photo.Path}", true);
                    break;

                default:
                    if (!SavedDocument && formValidation)
                    {
                        var result = await DialogAlertService.DialogYesNoAsync("Alerta!", "O documento não foi salvo.\n Deseja sair?");

                        if (result)
                        {
                            await DeleteDirectorysAndPhotos();

                            Shell.Current.Navigating -= Current_Navigating;
                            NavigationDirection = string.Empty;
                            await Shell.Current.GoToAsync("..", true);
                        }
                    }
                    else
                    {
                        Shell.Current.Navigating -= Current_Navigating;
                        NavigationDirection = string.Empty;
                        await Shell.Current.GoToAsync("..", true);
                    }
                    break;
            };

            SavedDocument = false;
        }

        bool FormValidation()
        {
            if (string.IsNullOrEmpty(ContainerNumber) || string.IsNullOrEmpty(ControlNumber))
                return false;
            else
                return true;
        }

        bool ValidateButton()
        {
            bool _isValidContainerNumber = string.IsNullOrEmpty(ContainerNumber);
            bool _isValidControlNumber = string.IsNullOrEmpty(ControlNumber);

            return !_isValidContainerNumber && !_isValidControlNumber;
        }

        void InputIsUpgradeable()
        {
            if (Photos.Count() > 0)
                EnableEntry = false;
            else
                EnableEntry = true;
        }

        #endregion
    }

}
