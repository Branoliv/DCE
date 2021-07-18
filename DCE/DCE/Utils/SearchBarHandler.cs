using Xamarin.Forms;

namespace DCE.Utils
{
    public class SearchBarHandler: SearchHandler
    {
        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);
        }

        protected override void OnItemSelected(object item)
        {
            base.OnItemSelected(item);
        }
    }
}
