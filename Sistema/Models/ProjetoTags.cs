using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class ProjetoTags
    {
        public int Id { get; set; }
        [Required]
        public int ProjetoId { get; set; }
        [Required]
        public int TagId { get; set; }
        public virtual Projeto Projeto { get; set; }
        public virtual Tag Tag { get; set; }
    }
}