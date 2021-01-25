using OnlineMarket.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.DataAccess.Repository.IRepository
{
    public interface IAppAddressRepository
    {
        Task<AppAddress> AddAsync(AppAddress data);
        IQueryable<AppAddress> AppAddress { get; }
        Task<AppAddress> GetInfoAsync(int id);
        Task<AppAddress> UpdateAsync(AppAddress data);
        Task<AppAddress> DeleteAsync(int id);
        IEnumerable<AppAddress> GetAllAppAddress();
    }
}
