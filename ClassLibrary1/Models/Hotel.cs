using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating {  get; set; }

        [ForeignKey(nameof(CountryId))]
        public int CountryId {  get; set; }
        public Country Country { get; set; }
    }
}
