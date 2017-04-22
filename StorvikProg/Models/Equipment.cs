using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StorvikProg.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Number { get; set; }
        
        public string Serial { get; set; }

    }
}