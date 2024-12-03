
using AutoMapper;
using ClassLibrary1.Dtos.Country;
using ClassLibrary1.Dtos.Hotel;
using ClassLibrary1.Dtos.Users;
using ClassLibrary1.Models;

namespace WebApplication1.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Country,CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
            CreateMap<Country,CountryDto>().ReverseMap();
            CreateMap<Country,UpdateCountryDto>().ReverseMap();
            CreateMap<Hotel,HotelDto>().ReverseMap();
            CreateMap<Hotel,CreateHotelDto>().ReverseMap();

            CreateMap<ApiUserDto,ApiUser>().ReverseMap();
        }
    }
}

