using OnlineMarket.DataAccess.Data;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Models;
using System.Threading.Tasks;

namespace OnlineMarket.DataAccess.Repository
{
    public class InfoRepository : IInfoRepository
    {
        private readonly ApplicationDbContext _context;

        public InfoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppInfo> AddAsync(AppInfo info)
        {
            _context.Add(info);
            await _context.SaveChangesAsync();
            return info;
        }

        public AppInfo GetInfo(int id)
        {
            return _context.AppInfos.Find(id);
        }

        public async Task<AppInfo> UpdateAsync(AppInfo infoUpdate)
        {
            var data = _context.AppInfos.Attach(infoUpdate);
            data.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
            return infoUpdate;
        }
    }
}
