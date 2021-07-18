namespace DCE.Models
{
    public class WorkUnitFolder
    {
        protected WorkUnitFolder() { }
        public WorkUnitFolder(string folderId, string folderName)
        {
            FolderId = folderId;
            FolderName = folderName;
        }

        public string FolderId { get; private set; }
        public string FolderName { get; private set; }
    }
}
