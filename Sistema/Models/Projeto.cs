using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class Projeto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Nome { get; set; }
        [StringLength(500, MinimumLength = 0)]
        [DataType(DataType.MultilineText)]
        public string Descricao { get; set; }
        public string Logo { get; set; }
        [Required]
        public DateTime DataCadastro { get; set; }
        ICollection<IntegrantesProjeto> IntegrantesProjetos { get; set; }
        ICollection<ArquivosProjeto> ArquivosProjetos { get; set; }
        ICollection<ProjetoTags> ProjetoTags { get; set; }

    }
}