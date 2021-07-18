using System;
using System.Collections.Generic;
using System.Text;

namespace DCE.Models
{
    public class Thumbnail
    {
        protected Thumbnail() { }
        public Thumbnail(string pathThumbnail)
        {
            PathThumbnail = pathThumbnail;
        }

        public string PathThumbnail { get; set; }
    }
}
