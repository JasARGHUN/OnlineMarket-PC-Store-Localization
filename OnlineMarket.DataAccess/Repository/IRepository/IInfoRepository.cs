using OnlineMarket.Models;
using System.Threading.Tasks;

namespace OnlineMarket.DataAccess.Repository.IRepository
{
    public interface IInfoRepository
    {
        Task<AppInfo> AddAsync(AppInfo info);
        AppInfo GetInfo(int id);
        Task<AppInfo> UpdateAsync(AppInfo infoUpdate);
    }
}
