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
    [QueryProperty(nameof(DocumentId), nameof(DocumentId))]
    public class UpdateDocumentViewModel : BaseViewModel
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

        private string _documentId;
        public string DocumentId
        {
            get => _documentId;
            set
            {
                _documentId = value;
                LoadDocument(value).ConfigureAwait(true);
            }
        }

        private Document _document;
        public Document Document
        {
            get => _document;
            set
            {
                SetProperty(ref _document, value);
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

        private int _photoCounter;
        public int PhotoCounter
        {
            get => _photoCounter;
            set
            {
                SetProperty(ref _photoCounter, value);
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

        private ObservableCollection<Photo> _updatePhotos;
        public ObservableCollection<Photo> UpdatePhotos
        {
            get => _updatePhotos;
            set
            {
                SetProperty(ref _updatePhotos, value);
                InputIsUpgradeable();
            }
        }


        public UpdateDocumentViewModel()
        {
            UpdatePhotos = new ObservableCollection<Photo>();

            SaveDocumentCommand = new Command(async () => await SaveDocumentUpdate());
            CapturePhotoCommand = new Command(async () => await CapturePhotoCrossMedia());
            ResetNewDocumentCommand = new Command(async () => await ResetNewDocument());
            TakeGalleriaPhotoCommand = new Command(async () => await TakeGalleriaPhoto());
            DeleteImageCommand = new Command<Photo>(async (img) => await DeleteImageFromUpdateList(img));

            ImageViewCommand = new Command<Photo>(async (img) =>
            {
                await ImageView(img);
            });

            OnDisappearingCommand = new Command(() => OnDisappearing());
            OnAppearingCommand = new Command(() => OnAppearing());
        }


        private async Task LoadDocument(string value)
        {
            try
            {
                IsEnable = false;
                var document = await DocumentService.GetByIdAsync(Guid.Parse(value));

                if (document == null)
                    return;

                Document = document;

                if (UpdatePhotos.Count == 0)
                {
                    var documentPhotos = await PhotoService.ListPhotoAsync(Document.Id);

                    foreach (var photo in documentPhotos)
                    {
                        var photoExist = UpdatePhotos.Any(p => p.Id == photo.Id);

                        if (!photoExist)
                            UpdatePhotos.Add(photo);
                    }
                }

                PhotoCounter = UpdatePhotos.Count();
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

        async Task DeleteImageFromUpdateList(Photo img)
        {
            IsEnable = false;
            var result = await DialogAlertService.DialogYesNoAsync("Alerta!", "Deseja apagar a foto?");

            if (result)
            {
                UpdatePhotos.Remove(img);
                PhotoCounter = UpdatePhotos.Count;
            }

            IsEnable = true;
        }

        async Task SaveDocumentUpdate()
        {
            try
            {
                IsBusy = true;
                IsEnable = false;

                if (!FormValidation())
                {
                    await DialogAlertService.DialogYesAsync("Alerta!", "O número de container e controle,são obrigatórios!");
                    return;
                }

                if (UpdatePhotos.Count() <= 0)
                {
                    await DialogAlertService.DialogYesAsync("Alerta!", "É necessário pelo menos uma foto.");
                    return;
                }

                foreach (var photo in UpdatePhotos)
                {
                    var photoExist = await PhotoService.GetByIdAsync(photo.Id);

                    if (photoExist == null)
                    {
                        string pathThumbnail = photo.PathThumbnail.Replace("Update/", "");
                        File.Move(photo.PathThumbnail, pathThumbnail);

                        string path = photo.Path.Replace("Update/", "");
                        File.Move(photo.Path, path);

                        photo.Path = path;
                        photo.PathThumbnail = pathThumbnail;

                        //TODO - Encapsular objeto
                        //Photo newPhoto = new Photo(
                        //    photo.Id,
                        //    photo.Name,
                        //    path,
                        //    pathThumbnail,
                        //    photo.DocumentId,
                        //    photo.InclusionDate);

                        bool resultUpdate = await PhotoService.UpdateAsync(photo);
                    }
                }

                List<Photo> updatePhotos = await PhotoService.ListPhotoAsync(Document.Id);

                foreach (var photo in updatePhotos)
                {
                    var photoExist = UpdatePhotos.Any(p => p.Id == photo.Id);

                    if (!photoExist)
                    {
                        await DocumentService.DeletePhoto(photo);
                        await PhotoService.DeleteAsync(photo.Id);
                    }
                }

                var doc = await DocumentService.GetByIdAsync(Document.Id);

                Document updatedDocument = new Document(
                    doc.Id,
                    doc.ContainerNumber,
                    doc.ControlNumber,
                    doc.SealNumber,
                    doc.InclusionDate,
                    UpdatePhotos.Count,
                    UpdatePhotos.ToList(),
                    doc.Copied);

                var result = await DocumentService.UpdateAsync(updatedDocument);

                if (result)
                {
                    await DeleteDirectorysAndPhotos();
                }

                var document = await DocumentService.GetDocumentByContainerNumberAndControlNumber(doc.ContainerNumber, doc.ControlNumber);


                if (FinishedContainerStuffing)
                {
                    if (await MSGraphService.CheckAccountLoggedIn() && await MSGraphService.CheckExistConfig())
                        await UploadDocument(document);

                    NavigationDirection = "Save";
                    SavedDocument = true;
                    //TODO - verificar navegação
                    //await Shell.Current.GoToAsync("///Home");
                    App.Current.MainPage = new AppShell();
                }
                else
                {
                    NavigationDirection = "Save";
                    SavedDocument = true;

                    await Shell.Current.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert($"Erro - {ex.InnerException}", ex.Message, "Ok");
            }
            finally
            {
                InputIsUpgradeable();
                IsEnable = true;
                IsBusy = false;
            }
        }

        async Task CapturePhotoCrossMedia()
        {
            try
            {

                IsEnable = false;

                if (UpdatePhotos.Count >= 12)
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
                    await DialogAlertService.DialogYesAsync("Alerta", "Número de container/controle, são obrigatórios.");
                    return;
                }

                var fileName = CreateFileName();

                var photo = await PhotoService.CapturePhotoCrossMediaAsync(GetFolderName(), fileName, true);

                if (photo == null)
                {
                    ToastService.ToastShort("Foto não tirada.");
                    return;
                }

                var newPhotoUpdated = new Photo(photo.Name, photo.Path, photo.PathThumbnail, Document.Id, DateTime.Now);

                UpdatePhotos.Add(newPhotoUpdated);
                PhotoCounter = UpdatePhotos.Count;
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

        async Task TakeGalleriaPhoto()
        {
            try
            {
                IsEnable = false;

                if (UpdatePhotos.Count >= 12)
                {
                    await DialogAlertService.DialogYesAsync("Informação", "Limite de 12 fotos atingido.");
                    return;
                }

                if (!ValidateButton())
                {
                    await DialogAlertService.DialogYesAsync("Alerta", "Número de container/controle, são obrigatórios.");
                    return;
                }

                var fileName = CreateFileName();

                var photo = await PhotoService.TakeGalleriaPhotoAsync(Path.Combine("Update", GetFolderName()), fileName, true);

                if (photo == null)
                {
                    ToastService.ToastShort("Foto não selecionada.");
                    return;
                }

                var newPhotoUpdated = new Photo(photo.Name, photo.Path, photo.PathThumbnail, Document.Id, DateTime.Now);

                UpdatePhotos.Add(newPhotoUpdated);
                PhotoCounter = UpdatePhotos.Count;
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
            return $"{Document.ContainerNumber}_{Document.ControlNumber}";
        }

        string CreateFileName()
        {
            var date = DateTime.Now;

            var index = $"{date.Year}{date.Month}{date.Day}{date.Hour}{date.Minute}{date.Second}";
            return $"{Document.ContainerNumber}_{Document.ControlNumber}_{index}.jpg";
        }

        async Task ImageView(Photo photo)
        {
            IsEnable = false;
            NavigationDirection = "ViewImage";
            Photo = new Photo(photo.Name, photo.Path, photo.PathThumbnail);
            await Shell.Current.GoToAsync($"{nameof(ImageViewPage)}?{nameof(ImageViewViewModel.Path)}={photo.Path}");
            IsEnable = true;
        }

        async Task ResetNewDocument()
        {
            IsEnable = false;
            var result = await DialogAlertService.DialogYesNoAsync("Alerta!", "O documento não foi salvo.\n Deseja sair?");

            if (result)
            {
                await DeleteDirectorysAndPhotos();

                Document.ClearDocument();
                //Document.ContainerNumber = string.Empty;
                //Document.ControlNumber = string.Empty;
                //Document.SealNumber = string.Empty;
                UpdatePhotos.Clear();
                PhotoCounter = 0;
                InputIsUpgradeable();
            }

            IsEnable = true;
        }

        async Task DeleteDirectorysAndPhotos()
        {
            IsEnable = false;
            await DocumentService.DeleteThumbnails(Path.Combine("Update", GetFolderName()));
            await DocumentService.DeleteDirectoryThumbnails(Path.Combine("Update", GetFolderName()));

            await DocumentService.DeletePhotos(Path.Combine("Update", GetFolderName()));
            await DocumentService.DeleteDirectoryPhotos(Path.Combine("Update", GetFolderName()));
            IsEnable = true;
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
                            foreach (var photo in UpdatePhotos)
                            {
                                var photoExist = await PhotoService.GetByIdAsync(photo.Id);

                                if (photoExist == null)
                                {
                                    await DeleteDirectorysAndPhotos();
                                    break;
                                }
                            }

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
            if (string.IsNullOrEmpty(Document.ContainerNumber) || string.IsNullOrEmpty(Document.ControlNumber))
                return false;
            else
                return true;
        }

        bool ValidateButton()
        {
            bool _isValidContainerNumber = string.IsNullOrEmpty(Document.ContainerNumber);
            bool _isValidControlNumber = string.IsNullOrEmpty(Document.ControlNumber);

            return !_isValidContainerNumber && !_isValidControlNumber;
        }

        void InputIsUpgradeable()
        {
            if (UpdatePhotos.Count() > 0)
                EnableEntry = false;
            else
                EnableEntry = true;
        }

        #endregion
    }
}
