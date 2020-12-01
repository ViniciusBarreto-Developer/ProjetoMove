using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [StringLength(200, MinimumLength = 2)]
        [Required]
        public string Nome { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public string DataNascimento { get; set; }
        [Required]
        [StringLength(14, MinimumLength = 14)]
        public string Cpf { get; set; }
        [StringLength(15, MinimumLength = 15)]
        public string Celular { get; set; }
        [MaxLength(100)]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [MaxLength(100)]
        [Required]
        [Display(Name = "Email de recuperação")]
        [DataType(DataType.EmailAddress)]
        public string EmailRecuperacao { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
        public string Foto { get; set; }
        public string Biografia { get; set; }
        [Required]
        public bool Ativo { get; set; }
        public string Hash { get; set; }
        public bool Adm { get; set; }
        public DateTime Punicao { get; set; }
        ICollection<ProjetosSalvos> ProjetosSalvos { get; set; }
        ICollection<IntegrantesProjeto> IntegrantesProjetos { get; set; }
        ICollection<UsuarioTag> UsuarioTags{ get; set; }
    }
}