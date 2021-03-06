﻿using System;
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
        [Required]
        public string Descricao { get; set; }
        [Required]
        public string Logo { get; set; }
        [Required]
        public DateTime DataCadastro { get; set; }
        [Required]
        public bool Ativo { get; set; }
        public DateTime Punicao { get; set; }
        public string Inativo { get; set; }
        public virtual ICollection<IntegrantesProjeto> IntegrantesProjetos { get; set; }
        public virtual ICollection<ArquivosProjeto> ArquivosProjetos { get; set; }
        public virtual ICollection<ProjetoTags> ProjetoTags { get; set; }

    }
}