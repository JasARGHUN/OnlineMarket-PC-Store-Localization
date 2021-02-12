using OnlineMarket.DataAccess.Data;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Models;

namespace OnlineMarket.DataAccess.Repository
{
    public class CallBackRepository : Repository<CallBack>, ICallBackRepository
    {
        private readonly ApplicationDbContext _context;

        public CallBackRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
