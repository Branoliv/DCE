using DCE.Models;
using DCE.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DCE.ViewModels
{
    [QueryProperty(nameof(DocumentId), nameof(DocumentId))]
    public class DocumentDetailViewModel : BaseViewModel
    {
        private Document _document;
        public ICommand ImageViewCommand { get; }
        public ICommand UpdateDocumentCommand { get; }

        public Document Document
        {
            get { return _document; }
            set { SetProperty(ref _document, value); }
        }

        private string _documentId;
        public string DocumentId
        {
            get { return _documentId; }
            set
            {
                _documentId = value;

                Task.Run(
                    async () =>
                    {
                        await LoadDocument(value);
                    });

            }
        }

        private List<Photo> _photos;
        public List<Photo> Photos
        {
            get => _photos;
            set
            {
                SetProperty(ref _photos, value);
            }
        }

        public DocumentDetailViewModel()
        {
            Photos = new List<Photo>();
            ImageViewCommand = new Command<Photo>(ImageView);
            UpdateDocumentCommand = new Command(UpdateDocument);
        }

        public async Task LoadDocument(string documentId)
        {
            try
            {
                Document = await DocumentService.GetByIdAsync(Guid.Parse(documentId));

                if (Photos.Count > 0)
                    Photos.Clear();

                Photos = await PhotoService.ListPhotoAsync(Document.Id);
            }
            catch (Exception ex)
            {
                await DialogAlertService.DialogYesAsync("Erro", ex.Message);
            }

        }

        async void ImageView(Photo photo)
        {
            await Shell.Current.GoToAsync($"{nameof(ImageViewPage)}?{nameof(ImageViewViewModel.Path)}={photo.Path}");
        }

        async void UpdateDocument()
        {
            if (Document == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(UpdateDocumentPage)}?{nameof(UpdateDocumentViewModel.DocumentId)}={Document.Id}");
        }
    }
}
