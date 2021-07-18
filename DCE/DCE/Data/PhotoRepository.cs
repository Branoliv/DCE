using DCE.Data.Interface;
using DCE.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCE.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        public PhotoRepository() { }

        public async Task<bool> AddAsync(Photo obj)
        {
            try
            {
                using (var _context = new DCEContext())
                {
                    await _context.Photos
                    .AddAsync(obj);

                    var result = _context.SaveChangesAsync();

                    if (await result > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (SqliteException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using (var _context = new DCEContext())
            {
                var doc = await _context.Photos
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

                _context.Remove(doc);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<Photo> GetByIdAsync(Guid id)
        {
            using (var _context = new DCEContext())
            {
                return await _context.Photos
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            }
        }

        public async Task<List<Photo>> ListPhotoAsync(Guid idDocument)
        {
            using (var _context = new DCEContext())
            {
                return await _context.Photos
                .Where(d => d.DocumentId == idDocument)
                .ToListAsync();
            }
        }

        public async Task<IEnumerable<Photo>> ListAsync()
        {
            using (var _context = new DCEContext())
            {
                return await _context.Photos
                .ToListAsync();
            }
        }

        public async Task<bool> UpdateAsync(Photo obj)
        {
            using (var _context = new DCEContext())
            {
                var doc = _context.Photos.Update(obj).Entity;
                await _context.SaveChangesAsync();
                if (doc == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
