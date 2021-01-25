using OnlineMarket.DataAccess.Data;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.DataAccess.Repository
{
    public class AppAddressRepository : IAppAddressRepository
    {
        private ApplicationDbContext _repository;
        public AppAddressRepository(ApplicationDbContext repository)
        {
            _repository = repository;
        }
        public async Task<AppAddress> AddAsync(AppAddress data)
        {
            _repository.AppAddresses.Add(data);
            await _repository.SaveChangesAsync();
            return data;
        }
        public IEnumerable<AppAddress> GetAllAppAddress()
        {
            return _repository.AppAddresses;
        }
        public IQueryable<AppAddress> AppAddress => _repository.AppAddresses;

        public async Task<AppAddress> GetInfoAsync(int id)
        {
            return await _repository.AppAddresses.FindAsync(id);
        }

        public async Task<AppAddress> UpdateAsync(AppAddress data)
        {
            var item = _repository.AppAddresses.Attach(data);
            item.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _repository.SaveChangesAsync();
            return data;
        }

        public async Task<AppAddress> DeleteAsync(int id)
        {
            AppAddress item = await _repository.AppAddresses.FindAsync(id);
            if (item != null)
            {
                _repository.AppAddresses.Remove(item);
                await _repository.SaveChangesAsync();
            }
            return item;
        }
    }
}
