using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class VMPerfil
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
        public string Biografia { get; set; }
        [Display(Name = "Pesquisa por Tag")]
        public string PesquisaTag { get; set; }
        public string Logo { get; set; }
        [Display(Name = "Nome do Projeto")]
        [Required]
        public string NomeProjeto { get; set; }
        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        public string MotivoDenuncia { get; set; }
        public string Observacao { get; set; }
        public Boolean Adm { get; set; }
        [Display(Name = "Quantidade de Dias:")]
        public int Punicao { get; set; }
        public bool Ativo { get; set; }
        public virtual ICollection<ProjetosSalvos> ProjetosSalvos { get; set; }
        public virtual ICollection<IntegrantesProjeto> IntegrantesProjetos { get; set; }
        public virtual ICollection<UsuarioTag> UsuarioTags { get; set; }
    }
}