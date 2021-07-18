using DCE.Utils.Interfaces;
using System.Threading.Tasks;

namespace DCE.Utils
{
    public class DialogAlertService : IDialogAlertService
    {
        public async Task DialogYesAsync(string title, string message)
        {
            await App.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        public async Task<bool> DialogYesNoAsync(string title, string message)
        {
            return await App.Current.MainPage.DisplayAlert(title, message, "Sim", "Não");
        }
    }
}
