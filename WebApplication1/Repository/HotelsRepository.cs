using AutoMapper;
using ClassLibrary1;
using ClassLibrary1.Models;
using WebApplication1.Contracts;

namespace WebApplication1.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private readonly AppDbContext _context;
        public HotelsRepository(AppDbContext context,IMapper mapper) : base(context,mapper)
        {
            _context = context;

        }
    }
}
