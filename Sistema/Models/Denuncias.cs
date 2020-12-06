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

        public string Observacao { get; set; }

        [Required]
        public string Status { get; set; }

        public int Punicao { get; set; }
        public Boolean Desativado { get; set; }

        public DateTime DataPunicao { get; set; }

        public string MotivoPunicao { get; set; }

        [Required]        
        public int UsuarioDenuncianteId { get; set; }

        [ForeignKey("UsuarioDenuncianteId")]        
        public virtual Usuario UsuarioDenunciante { get; set; }

        public int? UsuarioDenunciadoId { get; set; }

        [ForeignKey("UsuarioDenunciadoId")]
        public virtual Usuario UsuarioDenunciado { get; set; }

        public int? ProjetoDenunciadoId { get; set; }

        [ForeignKey("ProjetoDenunciadoId")]
        public virtual Projeto ProjetoDenunciado { get; set; }

        public int? AdmId { get; set; }

        [ForeignKey("AdmId")]
        public virtual Usuario Adm { get; set; }

    }
}