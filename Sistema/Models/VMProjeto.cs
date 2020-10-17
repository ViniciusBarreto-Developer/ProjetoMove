using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class VMProjeto
    {
        [Required]
        public string NomeTag { get; set; }
    }
}