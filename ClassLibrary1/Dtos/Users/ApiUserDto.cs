﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Dtos.Users
{
    public class ApiUserDto : LoginDto
    {
        [Required]
        public string FirstName {  get; set; }
        [Required]
        public string LastName { get; set; }
    }


}
