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
        [Display(Name = "Nome*")]
        public string Nome { get; set; }
        [StringLength(200, MinimumLength = 2)]
        [Display(Name = "Nome social")]
        public string NomeSocial { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de nascimento*")]
        public string DataNascimento { get; set; }
        [Required]
        [Display(Name = "Cpf*")]
        public long Cpf { get; set; }
        [MaxLength(100)]
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail*")]
        public string Email { get; set; }
        [MaxLength(100)]
        [Required]
        [Display(Name = "E-mail de recuperação*")]
        [DataType(DataType.EmailAddress)]
        public string EmailRecuperacao { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha*")]
        [RegularExpression("((?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,12})", ErrorMessage = "A senha deve conter aos menos uma letra maiúscula, minúscula e um número.Deve ser no mínimo 6 caracteres")]
        public string Senha { get; set; }
        [Required]
        [Display(Name = "Confirmar senha*")]
        [DataType(DataType.Password)]
        [Compare("Senha")]
        public string ConfirmaSenha { get; set; }
    }
    public class EditarCadastro
    {
        [StringLength(200, MinimumLength = 2)]
        [Required]
        [Display(Name = "Nome*")]
        public string Nome { get; set; }
        [StringLength(200, MinimumLength = 2)]
        [Display(Name = "Nome social")]
        public string NomeSocial { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de nascimento*")]
        public string DataNascimento { get; set; }
        [Required]
        [Display(Name = "Cpf*")]
        public long Cpf { get; set; }
        [MaxLength(100)]
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail*")]
        public string Email { get; set; }
        [MaxLength(100)]
        [Required]
        [Display(Name = "E-mail de recuperação*")]
        [DataType(DataType.EmailAddress)]
        public string EmailRecuperacao { get; set; }
        [Display(Name = "Nova senha")]
        [DataType(DataType.Password)]
        [RegularExpression("((?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,12})", ErrorMessage = "A senha deve conter aos menos uma letra maiúscula, minúscula e um número.Deve ser no mínimo 6 caracteres")]
        public string Senha { get; set; }
        [Display(Name = "Confirmar nova senha")]
        [DataType(DataType.Password)]
        [Compare("Senha")]
        public string ConfirmarNovaSenha { get; set; }
        [Required]
        [Display(Name = "Senha Atual*")]
        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; }
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
    public class Mensagem
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Assunto { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mensagem")]
        public string CorpoMsg { get; set; }
    }
    public class EsqueceuSenha
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
    public class RedefinirSenha
    {
        public string Email { get; set; }
        public string Hash { get; set; }
        [DataType(DataType.Password)]
        [RegularExpression("((?=.*\\d)(?=.*[a-z])(?=.*[AZ]).{6,12})", ErrorMessage = "A senha deve conter aosmenos uma letra maiúscula, minúscula e um número.Deveser no mínimo 6 caracteres")]
        public string Senha { get; set; }
        [DataType(DataType.Password)]
        [Compare("Senha")]
        [Display(Name = "Confirma Senha")]
        public string ConfirmaSenha { get; set; }
    }
}