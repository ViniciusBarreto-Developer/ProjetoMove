using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class Denuncias
    {
        public int Id { get; set; }
        [Required]
        public string DataCadastro { get; set; }
        [Required]
        public string Motivo { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]        
        public int UsuarioDenuncianteId { get; set; }
        [ForeignKey("UsuarioDenuncianteId")]
        public Usuario UsuarioDenunciante { get; set; }
        public int UsuarioDenunciadoId { get; set; }
        [ForeignKey("UsuarioDenunciadoId")]
        public Usuario UsuarioDenunciado { get; set; }
        public int ProjetoDenunciadoId { get; set; }
        [ForeignKey("ProjetoDenunciadoId")]
        public Projeto ProjetoDenunciado { get; set; }

        ICollection<Projeto> Projetos { get; set; }
        ICollection<Projeto> Usuarios { get; set; }
    }
}