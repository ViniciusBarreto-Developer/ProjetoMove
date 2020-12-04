using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema.Models
{
    public class IntegrantesProjeto
    {
        public int Id { get; set; }
        [Required]
        public int UsuarioID { get; set; }
        [Required]
        public int ProjetoId { get; set; }
        [Required]
        public Boolean Adm { get; set; }
        public Boolean Ativo { get; set; }
        public string Inativo { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Projeto Projeto { get; set; }
        ICollection<Projeto> Projetos { get; set; }

    }
}