using DCE.Data.Interface;
using DCE.Models;
using DCE.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DCE.Services
{
    public class ConfigurationService : IConfigurationService
    {
        readonly IConfigurationRepository _configurationRepository;

        public ConfigurationService()
        {
            _configurationRepository = DependencyService.Get<IConfigurationRepository>();
        }

        public async Task<bool> AddAsync(Configuration obj)
        {
            var configs = await _configurationRepository.ListAsync();

            if(configs.Count() > 0)
            {
                foreach (var cfg in configs)
                {
                    await _configurationRepository.DeleteAsync(cfg.Id);
                }
            }

            return await _configurationRepository.AddAsync(obj);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _configurationRepository.DeleteAsync(id);
        }

        public async Task<Configuration> GetByIdAsync(Guid id)
        {
            return await _configurationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Configuration>> ListAsync()
        {
            return await _configurationRepository.ListAsync();
        }

        public async Task<bool> UpdateAsync(Configuration obj)
        {
            return await _configurationRepository.UpdateAsync(obj);
        }
    }
}
