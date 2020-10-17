using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class PesquisarProjetos
    {
        [Required]
        [Display(Name = "")]
        public string Palavra { get; set; }
    }
}