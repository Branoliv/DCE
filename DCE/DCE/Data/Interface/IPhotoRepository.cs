using DCE.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DCE.Data.Interface
{
    public interface IPhotoRepository : IBaseRepository<Photo>
    {
        Task<List<Photo>> ListPhotoAsync(Guid idDocument);
    }
}
