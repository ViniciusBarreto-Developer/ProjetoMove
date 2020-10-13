using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class UsuarioTag
    {
        public int Id { get; set; }
        [Required]
        public int TagId { get; set; }
        [Required]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Tag Tag { get; set; }
        ICollection<Tag> Tags { get; set; }
    }
}