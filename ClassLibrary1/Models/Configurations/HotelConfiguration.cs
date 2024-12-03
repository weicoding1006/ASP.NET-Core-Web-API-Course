using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models.Configurations
{
    internal class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                    new Hotel
                    {
                        Id = 1,
                        Name = "圓山飯店",
                        Address = "台北市",
                        CountryId = 1,
                        Rating = 4.2
                    },
                    new Hotel
                    {
                        Id = 2,
                        Name = "日本飯店",
                        Address = "東京",
                        CountryId = 2,
                        Rating = 4.6
                    }
                );
        }
    }
}
