using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UnitAspExample.Models
{
    public class Test
    {
        [Required]
        public string Name { get; set; }
    }
}
