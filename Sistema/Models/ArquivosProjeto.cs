using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class ArquivosProjeto
    {
        public int Id { get; set; }
        [Required]
        public string Arquivo { get; set; }
        [Required]
        public string Nome{ get; set; }
        [Required]
        public int ProjetoId { get; set; }
        public virtual Projeto Projeto { get; set; }

    }
}