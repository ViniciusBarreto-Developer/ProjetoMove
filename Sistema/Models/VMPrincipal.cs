using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class VMPrincipal
    {
        public string PesquisaTag { get; set; }
        public virtual ICollection<UsuarioTag> UsuarioTags { get; set; }
        public virtual ICollection<ProjetoTags> ProjetoTags { get; set; }
    }
}