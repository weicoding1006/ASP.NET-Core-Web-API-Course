using AutoMapper;
using ClassLibrary1;
using ClassLibrary1.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Contracts;
using WebApplication1.Exceptions;

namespace WebApplication1.Repository
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly AppDbContext _context;
        public CountriesRepository(AppDbContext context ,IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<Country> GetDetails(int id)
        {
            var country = await _context.Countries.Include(q => q.Hotels).FirstOrDefaultAsync(q => q.Id == id);
            if(country == null)
            {
                throw new NotFoundException(nameof(GetDetails), id);
            }
            return country; 
        }
    }
}
