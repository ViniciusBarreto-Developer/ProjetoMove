using Sistema.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema.Controllers
{
    public class AdmController : Controller
    {
        Contexto db = new Contexto();
        public ActionResult Index()
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            VMAdm vma = new VMAdm();

            vma.AdmId = usu.Id;
            vma.NomeAdm = usu.Nome;
            vma.NumeroUsuarios = db.Usuario.Count();
            vma.NumeroProjetos = db.Projeto.Count();
            vma.DenunciasProjetos = db.Denuncias.Where(x => x.ProjetoDenunciadoId != null && x.Status != "Concluído").ToList();
            vma.DenunciasUsuarios = db.Denuncias.Where(x => x.UsuarioDenunciadoId != null && x.Status != "Concluído").ToList();
            vma.Tags = db.Tag.OrderByDescending(x => x.Pesquisada).ToList();

            int quantTags = vma.Tags.Count();

            var quantpro = Enumerable.Range(1, quantTags).Select(i => new QuantidadeTagsProjetos()).ToList();
            var quantusu = Enumerable.Range(1, quantTags).Select(i => new QuantidadeTagsUsuarios()).ToList();

            int y = 0;
            foreach (var item in vma.Tags)
            {
                quantpro[y].IdTag = item.Id;
                quantpro[y].Quantidade = db.ProjetoTags.Where(x => x.TagId == item.Id).Count();

                y++;
            }
            y = 0;
            foreach (var item in vma.Tags)
            {
                quantusu[y].IdTag = item.Id;
                quantusu[y].Quantidade = db.UsuarioTag.Where(x => x.TagId == item.Id).Count();

                y++;
            }
            vma.QuantidadeTagsProjetos = quantpro;
            vma.QuantidadeTagsUsuarios = quantusu;

            return View(vma);
        }
        public ActionResult DenunciaUsuario()
        {
            return View(db.Denuncias.Where(x => x.UsuarioDenunciadoId != null && x.Status != "Concluído").ToList());
        }
        public ActionResult DenunciaProjeto()
        {
            return View(db.Denuncias.Where(x => x.ProjetoDenunciadoId != null && x.Status != "Concluído").ToList());
        }
        public ActionResult ExcluirProjeto(VMProjeto vmp)
        {

            Projeto pro = db.Projeto.Find(vmp.Id);
            pro.Ativo = false;
            db.Projeto.AddOrUpdate(pro);

            db.SaveChanges();

            TempData["MSG"] = "success|Projeto Excluído!";

            return RedirectToAction("MeuProjeto", "Home", new { vmp.Id });
        }
        public ActionResult PunirProjeto(VMProjeto vmp)
        {
            Projeto pro = db.Projeto.Find(vmp.Id);

            int result = DateTime.Compare(pro.Punicao, DateTime.Now);

            if (result < 0)
            {
                pro.Punicao = DateTime.Now.AddDays(vmp.Punicao);
            }
            else
            {
                pro.Punicao = pro.Punicao.AddDays(vmp.Punicao);
            }

            db.Projeto.AddOrUpdate(pro);

            db.SaveChanges();
            TempData["MSG"] = "success|Punição Aplicada!";

            return RedirectToAction("MeuProjeto", "Home", new { vmp.Id });
        }
        public ActionResult ConcluirDenuncia(int id)
        {
            Denuncias den = db.Denuncias.Find(id);

            den.Status = "Concluído";
            db.Denuncias.AddOrUpdate(den);
            db.SaveChanges();

            if (den.ProjetoDenunciado == null)
            {
                return RedirectToAction("DenunciaUsuario");
            }
            return RedirectToAction("DenunciaProjeto");
        }
        public ActionResult PunirUsuario(VMPerfil vmp)
        {
            Usuario usu = db.Usuario.Find(vmp.Id);

            int result = DateTime.Compare(usu.Punicao, DateTime.Now);

            if (result < 0)
            {
                usu.Punicao = DateTime.Now.AddDays(vmp.Punicao);
            }
            else
            {
                usu.Punicao = usu.Punicao.AddDays(vmp.Punicao);
            }

            db.Usuario.AddOrUpdate(usu);

            db.SaveChanges();
            TempData["MSG"] = "success|Punição Aplicada!";

            return RedirectToAction("MeuPerfil", "Home", new { vmp.Id });
        }
        public ActionResult ExcluirUsuario(VMPerfil vmp)
        {
            Usuario usu = db.Usuario.Find(vmp.Id);
            usu.Ativo = false;
            usu.Inativo = "Adm";

            db.Usuario.AddOrUpdate(usu);
            db.SaveChanges();

            var integrante = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id && x.Ativo == true).ToList();

            foreach (var item in integrante)
            {
                Projeto pro = db.Projeto.Find(item.ProjetoId);
                IntegrantesProjeto inte = db.IntegrantesProjeto.Find(item.Id);
                int quantAdm = db.IntegrantesProjeto.Where(x => x.Adm == true && x.ProjetoId == item.ProjetoId).Count();
                int quant = db.IntegrantesProjeto.Where(x => x.ProjetoId == item.ProjetoId && x.Ativo == true).Count();

                if (item.Adm == true)
                {
                    if (quant == 1)
                    {
                        inte.Ativo = false;
                        inte.Inativo = "Usuário";
                        db.IntegrantesProjeto.AddOrUpdate(inte);
                        db.SaveChanges();

                        pro.Ativo = false;
                        pro.Inativo = "Usuário";
                        db.Projeto.AddOrUpdate(pro);
                        db.SaveChanges();
                    }
                    else if (quantAdm > 1)
                    {
                        inte.Ativo = false;
                        inte.Inativo = "Usuário";
                        inte.Adm = false;
                        db.IntegrantesProjeto.AddOrUpdate(inte);
                        db.SaveChanges();
                    }
                    else
                    {
                        IntegrantesProjeto NovoAdm = db.IntegrantesProjeto.Where(x => x.ProjetoId == item.ProjetoId && x.UsuarioID != usu.Id).FirstOrDefault();

                        NovoAdm.Adm = true;
                        db.IntegrantesProjeto.AddOrUpdate(NovoAdm);
                        db.SaveChanges();

                        inte.Ativo = false;
                        inte.Inativo = "Usuário";
                        inte.Adm = false;
                        db.IntegrantesProjeto.AddOrUpdate(inte);
                        db.SaveChanges();
                    }
                }
                else
                {
                    inte.Ativo = false;
                    inte.Inativo = "Usuário";
                    db.IntegrantesProjeto.AddOrUpdate(inte);
                    db.SaveChanges();
                }
            }

            TempData["MSG"] = "success|Usuario Excluído!";

            return RedirectToAction("MeuPerfil", "Home", new { vmp.Id });

        }


    }
}