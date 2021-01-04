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
        public string EmailAdm { get; set; }
        public int NumeroUsuarios { get; set; }
        public int NumeroProjetos { get; set; }
        public virtual ICollection<DenunciasEQuantidade> DenunciasProjetos { get; set; }
        public virtual ICollection<DenunciasEQuantidade> DenunciasUsuarios { get; set; }
        public virtual ICollection<Denuncias> MaisDenunciasProjetos { get; set; }
        public virtual ICollection<Denuncias> MaisDenunciasUsuarios { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual List<QuantidadeTagsProjetos> QuantidadeTagsProjetos { get; set; }
        public virtual List<QuantidadeTagsUsuarios> QuantidadeTagsUsuarios { get; set; }
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
    public class DenunciasEQuantidade
    {
        public int Id { get; set; }
        public string DataCadastro { get; set; }
        public string Motivo { get; set; }
        public string Observacao { get; set; }
        public string Status { get; set; }
        public int Punicao { get; set; }
        public Boolean Desativado { get; set; }
        public DateTime DataPunicao { get; set; }
        public string MotivoPunicao { get; set; }
        public int UsuarioDenuncianteId { get; set; }
        public virtual Usuario UsuarioDenunciante { get; set; }
        public int? UsuarioDenunciadoId { get; set; }
        public virtual Usuario UsuarioDenunciado { get; set; }
        public int? ProjetoDenunciadoId { get; set; }
        public virtual Projeto ProjetoDenunciado { get; set; }
        public int? AdmId { get; set; }
        public virtual Usuario Adm { get; set; }
        public int QuantidadeDenuncias { get; set; }
    }
}