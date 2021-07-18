using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace DCE.Models
{
    public class Photo : EntityBase
    {
        protected Photo() { }
        public Photo(string name, string path, string pathThumbnail, Guid documentId, DateTime inclusionDate)
        {
            Name = name;
            Path = path;
            PathThumbnail = pathThumbnail;
            DocumentId = documentId;
            InclusionDate = inclusionDate;
        }

        public Photo(Guid id, string name, string path, string pathThumbnail, Guid documentId, DateTime inclusionDate) : base(id)
        {
            Name = name;
            Path = path;
            PathThumbnail = pathThumbnail;
            DocumentId = documentId;
            InclusionDate = inclusionDate;
        }

        public Photo(string name, string path, string pathThumbnail)
        {
            Name = name;
            Path = path;
            PathThumbnail = pathThumbnail;
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public string PathThumbnail { get; set; }
        public DateTime InclusionDate { get; set; }


        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }

        public virtual Document Document { get; set; }
    }
}
