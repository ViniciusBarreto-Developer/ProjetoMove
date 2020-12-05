using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class VMAdm
    {
        public int AdmId { get; set; }
        public string NomeAdm { get; set; }        
        public int NumeroUsuarios { get; set; }
        public int NumeroProjetos { get; set; }
        public virtual ICollection<Denuncias> DenunciasProjetos { get; set; }
        public virtual ICollection<Denuncias> DenunciasUsuarios { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<QuantidadeTagsProjetos> QuantidadeTagsProjetos { get; set; }
        public virtual ICollection<QuantidadeTagsUsuarios> QuantidadeTagsUsuarios { get; set; }
    }
    public class QuantidadeTagsProjetos
    {
        public int IdTag { get; set; }
        public int Quantidade { get; set; }
    }
    public class QuantidadeTagsUsuarios
    {
        public int IdTag { get; set; }
        public int Quantidade { get; set; }
    }
}