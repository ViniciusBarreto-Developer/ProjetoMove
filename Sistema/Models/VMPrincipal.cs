using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class VMPrincipal
    {
        public int UsuarioId { get; set; }
        public string PesquisaTag { get; set; }
        public virtual ICollection<UsuarioTag> UsuarioTags { get; set; }
        public virtual ICollection<ProjetosSalvos> ProjetosSalvos { get; set; }
        public virtual ICollection<ProjetoTags> ProjetoTags { get; set; }
        public virtual ICollection<IntegrantesProjeto> IntegrantesProjetos { get; set; }
        public virtual ICollection<Projeto> Projetos { get; set; }
    }
}