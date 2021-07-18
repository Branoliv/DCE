using System.Threading.Tasks;

namespace DCE.Utils.Interfaces
{
    public interface IDialogAlertService 
    {
        Task DialogYesAsync(string title, string message);
        Task<bool> DialogYesNoAsync(string title, string message);
    }
}
