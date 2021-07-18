using DCE.Models;
using DCE.Views;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DCE.ViewModels
{
    public class UploadViewModel : BaseViewModel
    {
        private ObservableCollection<Document> _documents;
        public ObservableCollection<Document> Documents
        {
            get => _documents;
            set
            {
                SetProperty(ref _documents, value);
            }
        }

        public ICommand LoadDocumentsCommand { get; }
        public ICommand UpLoadCommand { get; }
        public ICommand SelectForUploadCommand { get; }


        public readonly ProgressBar ProgressTotal = new ProgressBar();
        public readonly ProgressBar ProgressItem = new ProgressBar();

        private bool _isUpdate = false;
        public bool IsUpdate
        {
            get => _isUpdate;
            set
            {
                SetProperty(ref _isUpdate, value);
            }
        }

        private bool _visableSelect = false;
        public bool VisableSelect
        {
            get => _visableSelect;
            set
            {
                SetProperty(ref _visableSelect, value);
            }
        }

        private double _countProgressTotal;
        public double CountProgressTotal
        {
            get => _countProgressTotal;
            set
            {
                SetProperty(ref _countProgressTotal, value);
            }
        }

        private double _countProgressItem;
        public double CountProgressItem
        {
            get => _countProgressItem;
            set
            {
                SetProperty(ref _countProgressItem, value);
            }
        }

        private double _progressCurrentItem;
        public double ProgressCurrentItem
        {
            get => _progressCurrentItem;
            set
            {
                SetProperty(ref _progressCurrentItem, value);
            }
        }

        private double _progressTotalItem;
        public double ProgressTotalItem
        {
            get => _progressTotalItem;
            set
            {
                SetProperty(ref _progressTotalItem, value);
            }
        }

        private double _progressCurrentDocument;
        public double ProgressCurrentDocument
        {
            get => _progressCurrentDocument;
            set
            {
                SetProperty(ref _progressCurrentDocument, value);
            }
        }

        private double _progressTotalDocument;
        public double ProgressTotalDocument
        {
            get => _progressTotalDocument;
            set
            {
                SetProperty(ref _progressTotalDocument, value);
            }
        }

        private Document _selecteddocument;
        public Document SelectedItem
        {
            get => _selecteddocument;
            set
            {
                SetProperty(ref _selecteddocument, value);
                OnDocumentSelected(value);
            }
        }

        public UploadViewModel()
        {
            Documents = new ObservableCollection<Document>();
            LoadDocumentsCommand = new Command(async () => await ListDocument());
            SelectForUploadCommand = new Command<Document>(async (doc) =>
            {
                await SelectForUpload(doc);
            });

            UpLoadCommand = new Command(async () => await UpLoadAsync());

            ListDocument().ConfigureAwait(true);
        }

        async Task SelectForUpload(Document document)
        {
            try
            {
                if (document != null)
                {
                    var documents = Documents.ToList();

                    if (Documents.Count() > 0)
                        Documents.Clear();

                    foreach (var doc in documents)
                    {
                        var docUpload = new Document(doc.Id, doc.ContainerNumber, doc.ControlNumber, doc.SealNumber, doc.InclusionDate, doc.PhotoCounter, doc.Photos, doc.Copied);

                        if (doc.Id == document.Id)
                        {
                            docUpload = new Document(doc.Id, doc.ContainerNumber, doc.ControlNumber, doc.SealNumber, doc.InclusionDate, doc.PhotoCounter, doc.Photos, !doc.Copied);
                        }

                        await DocumentService.UpdateAsync(docUpload);

                        Documents.Add(docUpload);
                    }
                }
            }
            catch (Exception )
            {
                await DialogAlertService.DialogYesAsync("Erro", "Não foi possível carregar a lista");
            }
        }

        private async Task UpLoadAsync()
        {
            try
            {
                if (Documents.Where(d => d.Copied == true).Count() == 0)
                {
                    await DialogAlertService.DialogYesAsync("Atenção", "Nem um item foi selecionado para upload.");
                    return;
                }

                var checkAccountLoggedIn = await MSGraphService.CheckAccountLoggedIn();
                var checkExistConfig = await MSGraphService.CheckExistConfig();

                if (!checkAccountLoggedIn || !checkExistConfig)
                {
                    await DialogAlertService.DialogYesAsync("Aviso", "Login é necessário.");
                    return;
                }

                IsUpdate = true;

                VisableSelect = true;

                ProgressCurrentDocument = 0;
                var documentForUpload = Documents.Where(d => d.Copied == true).ToList();
                ProgressTotalDocument = documentForUpload.Count();

                List<Document> copiedDocuments = new List<Document>();

                foreach (var doc in documentForUpload)
                {
                    List<Models.Photo> copiedPhotos = new List<Models.Photo>();

                    if (doc.Copied)
                    {
                        var folderName = $"{doc.ContainerNumber}_{doc.ControlNumber}";
                        var existFolderOneDrive = await MSGraphService.ExistFolderOneDrive(folderName);

                        if (existFolderOneDrive.Count > 0 && !existFolderOneDrive[0].Name.Contains(folderName))
                        {
                            var result = await MSGraphService.CreateFolderOneDrive(folderName);
                        }
                        else if (existFolderOneDrive.Count == 0)
                        {
                            var result = await MSGraphService.CreateFolderOneDrive(folderName);
                        }

                        var photos = await PhotoService.ListPhotoAsync(doc.Id);

                        ProgressCurrentItem = 0;
                        ProgressTotalItem = photos.Count();

                        foreach (var photo in photos)
                        {
                            var resultUpload = await MSGraphService.LoadOneDrive(photo.Path, Path.Combine(folderName, photo.Name));

                            var responsePhotoUpload = resultUpload.AdditionalData["statusCode"].Equals("Created");

                            if (responsePhotoUpload)
                                copiedPhotos.Add(photo);

                            ProgressCurrentItem++;
                            CountProgressItem = ProgressCurrentItem / photos.Count();
                            await ProgressTotal.ProgressTo(CountProgressItem, 5, Easing.Linear);
                        }

                        foreach (var photo in copiedPhotos)
                        {
                            await DocumentService.DeletePhoto(photo);
                            await PhotoService.DeleteAsync(photo.Id);
                        }

                        var resultCopied = photos.Except(copiedPhotos);

                        if (resultCopied.Count() == 0)
                        {
                            var coumentUpdateResult = await DocumentService.UpdateAsync(doc);

                            if (coumentUpdateResult)
                            {
                                copiedDocuments.Add(doc);
                            }
                        }
                    }

                    ProgressCurrentDocument++;
                    CountProgressTotal = ProgressCurrentDocument / ProgressTotalDocument;
                    await ProgressItem.ProgressTo(CountProgressTotal, 5, Easing.Linear);
                }

                foreach (var doc in copiedDocuments)
                {

                    var resultDocumentDelete = await DocumentService.DeleteAsync(doc.Id);

                    if (resultDocumentDelete)
                    {
                        Documents.Remove(doc);
                        var directoryName = $"{doc.ContainerNumber}_{doc.ControlNumber}";

                        await DocumentService.DeleteDirectoryThumbnails(directoryName);
                        await DocumentService.DeleteDirectoryPhotos(directoryName);
                    }
                }

                VisableSelect = false;

                ToastService.ToastLong("Upload completo.");

                await Shell.Current.GoToAsync("///Home", true);
            }
            catch (MsalException mEx)
            {
                if (mEx.ErrorCode.Equals("access_denied"))
                {
                    ToastService.ToastShort("Não foi possível realizar o login.");
                }
            }
            catch (Exception ex)
            {
                IsUpdate = false;
                await DialogAlertService.DialogYesAsync("Erro", "Desculpe, alguma coisa não conforme previsto.");
            }
            finally
            {
                IsUpdate = false;
            }
        }

        private async Task ListDocument()
        {
            CountProgressTotal = 0;
            CountProgressItem = 0;
            ProgressCurrentItem = 0;
            ProgressTotalItem = 0;
            ProgressCurrentDocument = 0;
            ProgressTotalDocument = 0;

            IsBusy = true;
            VisableSelect = false;

            try
            {
                if (Documents.Count() > 0)
                    Documents.Clear();

                var documents = DocumentService.ListAsync().Result;

                foreach (var item in documents.OrderByDescending(d => d.InclusionDate))
                {
                    Documents.Add(item);
                }
            }
            catch (Exception)
            {
                await DialogAlertService.DialogYesAsync("Erro", "Não foi possível carregar a lista");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task OnDisappearing()
        {
            try
            {
                if (!IsUpdate)
                {
                    foreach (var doc in Documents)
                    {
                        if (doc.Copied)
                        {
                            var docUpload = new Document(doc.Id, doc.ContainerNumber, doc.ControlNumber, doc.SealNumber, doc.InclusionDate, doc.PhotoCounter, doc.Photos, !doc.Copied);
                            await DocumentService.UpdateAsync(docUpload);
                        }
                    }
                }
            }
            catch (Exception)
            {
                await DialogAlertService.DialogYesAsync("Erro", "Não foi possível carregar a lista");
            }
        }

        async void OnDocumentSelected(Document document)
        {
            if (document == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(DocumentDetailPage)}?{nameof(DocumentDetailViewModel.DocumentId)}={document.Id}");
        }
    }
}
