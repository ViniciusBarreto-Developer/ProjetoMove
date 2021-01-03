using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class FuncoesAdm
    {        
        public static List<QuantidadeTagsUsuarios> QuantidadeTagsUsuarios(ICollection<Tag> tags) 
        {
            Contexto db = new Contexto();

            int quantTags = db.Tag.Count();

            var quantusu = Enumerable.Range(1, quantTags).Select(i => new QuantidadeTagsUsuarios()).ToList();

            int y = 0;
            foreach (var item in tags)
            {
                quantusu[y].IdTag = item.Id;
                quantusu[y].Quantidade = db.UsuarioTag.Where(x => x.TagId == item.Id).Count();

                y++;
            }

            return quantusu;
        }
        public static List<QuantidadeTagsProjetos> QuantidadeTagsProjetos(ICollection<Tag> tags)
        {
            Contexto db = new Contexto();

            int quantTags = db.Tag.Count();

            var quantpro = Enumerable.Range(1, quantTags).Select(i => new QuantidadeTagsProjetos()).ToList();

            int y = 0;
            foreach (var item in tags)
            {
                quantpro[y].IdTag = item.Id;
                quantpro[y].Quantidade = db.ProjetoTags.Where(x => x.TagId == item.Id).Count();

                y++;
            }

            return quantpro;
        }
    }
}