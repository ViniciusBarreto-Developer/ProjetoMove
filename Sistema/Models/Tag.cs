using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        public int Pesquisada { get; set; }
    }
}