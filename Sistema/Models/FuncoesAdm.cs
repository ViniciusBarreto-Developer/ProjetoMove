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

            int quantTags = tags.Count();

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

            int quantTags = tags.Count();

            var quantProjeto = Enumerable.Range(1, quantTags).Select(i => new QuantidadeTagsProjetos()).ToList();

            int y = 0;
            foreach (var item in tags)
            {
                quantProjeto[y].IdTag = item.Id;
                quantProjeto[y].Quantidade = db.ProjetoTags.Where(x => x.TagId == item.Id).Count();

                y++;
            }

            return quantProjeto;
        }
        public static ICollection<DenunciasEQuantidade> DenunciasAgrupadas(ICollection<Denuncias> denunciados)
        {
            List<DenunciasEQuantidade> denunciasAgrupadas = new List<DenunciasEQuantidade>();            

            int IdDenunciado = 0;
            int y = -1;
            foreach (var item in denunciados)
            {
                if (IdDenunciado != item.UsuarioDenunciadoId && IdDenunciado != item.ProjetoDenunciadoId)
                {
                    DenunciasEQuantidade resultado = new DenunciasEQuantidade();

                    resultado.Motivo = item.Motivo;
                    resultado.MotivoPunicao = item.MotivoPunicao;
                    resultado.Observacao = item.Observacao;
                    resultado.ProjetoDenunciado = item.ProjetoDenunciado;
                    resultado.ProjetoDenunciadoId = item.ProjetoDenunciadoId;
                    resultado.Punicao = item.Punicao;
                    resultado.Status = item.Status;
                    resultado.UsuarioDenunciado = item.UsuarioDenunciado;
                    resultado.UsuarioDenunciadoId = item.UsuarioDenunciadoId;
                    resultado.UsuarioDenunciante = item.UsuarioDenunciante;                                                                             
                    resultado.QuantidadeDenuncias = 0;

                    denunciasAgrupadas.Add(resultado);

                    if(item.UsuarioDenunciadoId != null)
                    {
                        IdDenunciado = (int)item.UsuarioDenunciadoId;
                    }
                    else
                    {
                        IdDenunciado = (int)item.ProjetoDenunciadoId;
                    }
                    
                    y++;
                }
                else
                {
                    denunciasAgrupadas[y].QuantidadeDenuncias++;
                }
            }

            return denunciasAgrupadas;             
        }
    }
}