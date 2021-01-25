using OnlineMarket.DataAccess.Data;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OnlineMarket.DataAccess.Repository
{
    public class AppDataRepository : IAppDataRepository
    {
        private ApplicationDbContext _repository;
        public AppDataRepository(ApplicationDbContext repository)
        {
            _repository = repository;
        }
        public AppSocialAddress Add(AppSocialAddress data)
        {
            _repository.AppSocialAddresses.Add(data);
            _repository.SaveChanges();
            return data;
        }
        public IEnumerable<AppSocialAddress> GetAllAppSocialAddress()
        {
            return _repository.AppSocialAddresses;
        }
        public IQueryable<AppSocialAddress> AppSocialAddress => _repository.AppSocialAddresses;

        public AppSocialAddress GetInfo(int id)
        {
            return _repository.AppSocialAddresses.Find(id);
        }

        public AppSocialAddress Update(AppSocialAddress data)
        {
            var item = _repository.AppSocialAddresses.Attach(data);
            item.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _repository.SaveChanges();
            return data;
        }

        public async Task<AppSocialAddress> Delete(int id)
        {
            AppSocialAddress item = await _repository.AppSocialAddresses.FindAsync(id);
            if (item != null)
            {
                _repository.AppSocialAddresses.Remove(item);
                _repository.SaveChanges();
            }
            return item;
        }
    }
}
