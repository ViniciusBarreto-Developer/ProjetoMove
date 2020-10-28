using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class VMPerfil
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string NomeSocial { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
        public string Biografia { get; set; }
        public string PesquisaTag { get; set; }
        public virtual ICollection<ProjetosSalvos> ProjetosSalvos { get; set; }
        public virtual ICollection<IntegrantesProjeto> IntegrantesProjetos { get; set; }
        public virtual ICollection<UsuarioTag> UsuarioTags { get; set; }
    }
}