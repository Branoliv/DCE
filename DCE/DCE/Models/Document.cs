using System;
using System.Collections.Generic;

namespace DCE.Models
{
    public class Document : EntityBase
    {
        public Document(string containerNumber, string controlNumber, string sealNumber, DateTime inclusionDate, int photoCounter, bool copied = false)
        {
            ContainerNumber = containerNumber;
            ControlNumber = controlNumber;
            SealNumber = sealNumber;
            InclusionDate = inclusionDate;
            PhotoCounter = photoCounter;
            Copied = copied;
        }

        public Document(Guid id, string containerNumber, string controlNumber, string sealNumber, DateTime inclusionDate, int photoCounter, List<Photo> photos, bool copied = false) : base(id)
        {
            ContainerNumber = containerNumber;
            ControlNumber = controlNumber;
            SealNumber = sealNumber;
            InclusionDate = inclusionDate;
            PhotoCounter = photoCounter;
            Copied = copied;
            Photos = photos;
        }

        public void ClearDocument()
        {
            ContainerNumber = string.Empty;
            ControlNumber = string.Empty;
            SealNumber = string.Empty;
            PhotoCounter = 0;
            Copied = false;
        }

        protected Document() { }

        public string ContainerNumber { get; private set; }
        public string ControlNumber { get; private set; }
        public string SealNumber { get; private set; }
        public DateTime InclusionDate { get; private set; }
        public int PhotoCounter { get; private set; }
        public bool Copied { get; set; }

        public virtual List<Photo> Photos { get; private set; }
    }
}
