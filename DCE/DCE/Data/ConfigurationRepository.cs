using DCE.Data.Interface;
using DCE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCE.Data
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        public async Task<bool> AddAsync(Configuration obj)
        {
            using (var _context = new DCEContext())
            {
                await _context.Configurations
                    .AddAsync(obj);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using (var _context = new DCEContext())
            {
                var cfg = await _context.Configurations
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync();

                _context.Remove(cfg);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<Configuration> GetByIdAsync(Guid id)
        {
            using (var _contex = new DCEContext())
            {
                return await _contex.Configurations
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<Configuration>> ListAsync()
        {
            using (var _context = new DCEContext())
            {
                return await _context.Configurations
                    .ToListAsync();
            }
        }

        public async Task<bool> UpdateAsync(Configuration obj)
        {
            using (var _context = new DCEContext())
            {
                var cfg = _context.Configurations
                    .Update(obj)
                    .Entity;

                await _context.SaveChangesAsync();

                if (cfg == null)
                    return false;
                else
                    return true;
            }
        }
    }
}
