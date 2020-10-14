using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class Cadastro
    {
        [StringLength(200, MinimumLength = 2)]
        [Required]
        public string Nome { get; set; }
        [StringLength(200, MinimumLength = 2)]
        [Display(Name = "Nome Social")]
        public string NomeSocial { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public string DataNascimento { get; set; }
        [Required]
        public long Cpf { get; set; }
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
        [RegularExpression("((?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,12})", ErrorMessage = "A senha deve conter aos menos uma letra maiúscula, minúscula e um número.Deve ser no mínimo 6 caracteres")]
        public string Senha { get; set; }
        [Required]
        [Display(Name = "Confirma Senha")]
        [DataType(DataType.Password)]
        [Compare("Senha")]
        public string ConfirmaSenha { get; set; }
    }
    public class Acesso
    {
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [RegularExpression("((?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,12})", ErrorMessage = "A senha deve conter aos menos uma letra maiúscula, minúscula e um número. Deve ser no mínimo 6 caracteres")]
        public string Senha { get; set; }
    }
}