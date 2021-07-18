using DCE.Models;
using DCE.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DCE.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Document> Documents { get; set; }
        public ICommand LoadDocumentsCommand { get; }
        public ICommand DocumentTapped { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddDocumentCommand { get; }
        

        public HomeViewModel()
        {
            Documents = new ObservableCollection<Document>();
            LoadDocumentsCommand = new Command(async () => await ListDocument());
            DocumentTapped = new Command<Document>(OnDocumentSelected);
            UpdateCommand = new Command<Document>(UpdateDocument);
            DeleteCommand = new Command<Document>(async (doc) => await DeleteDocument(doc));
            AddDocumentCommand = new Command(async () => await AddDocument());

            ListDocument().ConfigureAwait(true);
        }


        private async Task ListDocument()
        {
            IsBusy = true;

            try
            {
                if (Documents.Count() > 0)
                    Documents.Clear();

                var documents = await DocumentService.ListAsync();
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

        async void OnDocumentSelected(Document document)
        {
            if (document == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(DocumentDetailPage)}?{nameof(DocumentDetailViewModel.DocumentId)}={document.Id}");
        }

        async void UpdateDocument(Document document)
        {
            if (document == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(UpdateDocumentPage)}?{nameof(UpdateDocumentViewModel.DocumentId)}={document.Id}");
        }

        async Task DeleteDocument(Document document)
        {
            try
            {
                var resultDialog = await DialogAlertService.DialogYesNoAsync("Alerta!", "Deseja apagar a foto?");

                if (resultDialog)
                {
                    if (document == null)
                        return;

                    var directoryName = $"{document.ContainerNumber}_{document.ControlNumber}";

                    await DocumentService.DeleteThumbnails(directoryName);

                    await DocumentService.DeleteDirectoryThumbnails(directoryName);

                    await DocumentService.DeletePhotos(directoryName);

                    await DocumentService.DeleteDirectoryPhotos(directoryName);

                    var documentPhotos = await PhotoService.ListPhotoAsync(document.Id);

                    foreach (var photo in documentPhotos)
                    {
                        await PhotoService.DeleteAsync(photo.Id);
                    }

                    var resultDelete = await DocumentService.DeleteAsync(document.Id);

                    if (resultDelete)
                        await ListDocument();
                }
            }
            catch (Exception ex)
            {
                await DialogAlertService.DialogYesAsync("Erro", ex.Message);
                return;
            }
        }

        private async Task AddDocument()
        {
            await Shell.Current.GoToAsync($"{nameof(NewDocumentPage)}", true);
        }
    }
}
