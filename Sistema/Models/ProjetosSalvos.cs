using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class ProjetosSalvos
    {
        public int Id { get; set; }
        [Required]
        public int ProjetoId { get; set; }
        [Required]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Projeto Projeto { get; set; }
        ICollection<Projeto> Projetos { get; set; }
    }
}