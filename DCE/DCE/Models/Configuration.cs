namespace DCE.Models
{
    public class Configuration: EntityBase
    {
        protected Configuration() { }
        public Configuration(string userLogged, string groupName, string groupId, string workUnit, string workUnitId)
        {
            UserLogged = userLogged;
            GroupName = groupName;
            GroupId = groupId;
            WorkUnit = workUnit;
            WorkUnitId = workUnitId;
        }

        public string UserLogged { get; private set; }
        public string GroupName { get; private set; }
        public string GroupId { get; private set; }
        public string WorkUnit { get; private set; }
        public string WorkUnitId { get; private set; }
    }
}
