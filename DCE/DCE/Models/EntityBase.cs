using System;
using System.ComponentModel.DataAnnotations;

namespace DCE.Models
{
    public abstract class EntityBase
    {
        protected EntityBase()
        {
            Id = Guid.NewGuid();
        }

        protected EntityBase(Guid id)
        {
            Id = id;
        }

        [Key]
        public Guid Id { get; set; }
    }
}
