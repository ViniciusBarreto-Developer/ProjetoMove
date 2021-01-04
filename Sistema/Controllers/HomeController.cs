using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sistema.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace Sistema.Controllers
{
    public class HomeController : Controller
    {
        Contexto db = new Contexto();
        public ActionResult Principal()
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            if (usu == null)
            {
                return View();
            }

            VMPrincipal vmp = new VMPrincipal();

            vmp.UsuarioId = usu.Id;
            vmp.UsuarioTags = db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList();
            vmp.ProjetosSalvos = db.ProjetosSalvos.Where(x => x.UsuarioId == usu.Id).ToList();
            List<ProjetoTags> recomenda = new List<ProjetoTags>();

            if (vmp.UsuarioTags.Count() > 0)
            {
                vmp.ProjetoTags = db.ProjetoTags.Where(x => x.Projeto.Ativo == true).OrderBy(x => x.ProjetoId).ToList();

                int proId = 0;
                foreach (var protag in vmp.ProjetoTags)
                {
                    foreach (var usutag in vmp.UsuarioTags)
                    {
                        if (protag.TagId == usutag.TagId)
                        {
                            if (proId == protag.ProjetoId)
                            {
                                break;
                            }

                            bool salvo = false;
                            bool meu = false;

                            Projeto pro = db.Projeto.Find(protag.ProjetoId);

                            if (pro.Punicao > DateTime.Now)
                            {
                                break;
                            }

                            foreach (var subitem in pro.IntegrantesProjetos)
                            {
                                if (subitem.UsuarioID == usu.Id && subitem.Ativo == true)
                                {
                                    meu = true;
                                    break;
                                }
                            }
                            var prosal = db.ProjetosSalvos.Where(x => x.UsuarioId == usu.Id).ToList();
                            foreach (var subitem in prosal)
                            {
                                if (subitem.ProjetoId == protag.ProjetoId)
                                {
                                    salvo = true;
                                    break;
                                }
                            }

                            if (salvo == false && meu == false)
                            {
                                recomenda.Add(protag);
                                proId = protag.ProjetoId;
                                break;
                            }
                        }
                    }
                }

                vmp.ProjetoTags = recomenda;
            }
            return View(vmp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Principal(VMPrincipal vmp)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            VMPrincipal vmpNova = new VMPrincipal();

            if (usu == null)
            {
                if (vmp.PesquisaTag != null)
                {
                    vmpNova.ProjetoTags = db.ProjetoTags.Where(x => x.Tag.Nome == vmp.PesquisaTag && x.Projeto.Ativo == true && x.Projeto.Punicao < DateTime.Now).ToList();

                    Tag tag = db.Tag.Where(x => x.Nome == vmp.PesquisaTag).FirstOrDefault();
                    tag.Pesquisada++;

                    db.Tag.AddOrUpdate(tag);
                    db.SaveChanges();
                    return View(vmpNova);
                }
                return View();
            }

            vmpNova.UsuarioId = usu.Id;
            vmpNova.UsuarioTags = db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList();
            vmpNova.ProjetosSalvos = db.ProjetosSalvos.Where(x => x.UsuarioId == usu.Id).ToList();
            List<ProjetoTags> resultado = new List<ProjetoTags>();

            if (vmp.PesquisaTag != null)
            {
                foreach (var item in db.ProjetoTags.Where(x => x.Tag.Nome == vmp.PesquisaTag && x.Projeto.Ativo == true).ToList())
                {
                    bool salvo = false;
                    bool meu = false;

                    foreach (var subitem in item.Projeto.IntegrantesProjetos)
                    {
                        if (subitem.UsuarioID == usu.Id && subitem.Ativo == true)
                        {
                            meu = true;
                        }
                    }
                    foreach (var subitem in vmpNova.ProjetosSalvos)
                    {
                        if (subitem.ProjetoId == item.ProjetoId)
                        {
                            salvo = true;
                        }
                    }
                    if (meu == false && salvo == false)
                    {
                        resultado.Add(item);
                    }
                }

                Tag tag = db.Tag.Where(x => x.Nome == vmp.PesquisaTag).FirstOrDefault();
                tag.Pesquisada++;

                db.Tag.AddOrUpdate(tag);
                db.SaveChanges();
            }
            vmpNova.ProjetoTags = resultado;
            return View(vmpNova);
        }        
        public ActionResult MeuPerfil(int? id, int? idDenuncia)
        {
            VMPerfil vmp = new VMPerfil();
            Usuario usu = new Usuario();

            if (id == null)
            {
                string[] user = User.Identity.Name.Split('|');
                string email = user[0];

                if (user[0] == null || user[0] == "")
                {
                    return RedirectToAction("Principal");
                }
                usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            }
            else
            {
                usu = db.Usuario.Find(id);
                vmp.Adm = true;

                if(idDenuncia != null)
                {
                    vmp.IdDenuncia = (int)idDenuncia;
                }
            }

            vmp.Id = usu.Id;
            vmp.Biografia = usu.Biografia;
            vmp.Nome = usu.Nome;
            vmp.Email = usu.Email;
            vmp.Foto = usu.Foto;
            vmp.Ativo = usu.Ativo;
            vmp.UsuarioTags = db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList();
            vmp.IntegrantesProjetos = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id && x.Ativo == true).ToList();
            vmp.ProjetosSalvos = db.ProjetosSalvos.Where(x => x.UsuarioId == usu.Id).ToList();

            return View(vmp);
        }
        public ActionResult VisitarPerfil(int id)
        {
            Usuario usu = db.Usuario.Find(id);

            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usuAtual = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            if (usuAtual.Id == usu.Id)
            {
                return RedirectToAction("MeuPerfil");
            }
            if (usuAtual.Adm == true)
            {
                return RedirectToAction("MeuPerfil", new { id });
            }

            VMPerfil vmp = new VMPerfil();

            vmp.Id = usu.Id;
            vmp.Biografia = usu.Biografia;
            vmp.Nome = usu.Nome;
            vmp.Email = usu.Email;
            vmp.Foto = usu.Foto;
            vmp.UsuarioTags = db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList();
            vmp.IntegrantesProjetos = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id && x.Ativo == true && x.Projeto.Punicao < DateTime.Now).ToList();
            vmp.ProjetosSalvos = db.ProjetosSalvos.Where(x => x.UsuarioId == usu.Id).ToList();

            return View(vmp);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult EditarFoto(HttpPostedFileBase arq)
        {
            string valor = "";

            if (arq != null)
            {
                Funcoes.Upload.CriarDiretorio();
                string nomearq = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(arq.FileName);
                valor = Funcoes.Upload.UploadImagem(arq, nomearq);
                if (valor == "sucesso")
                {
                    string[] user = User.Identity.Name.Split('|');
                    string email = user[0];
                    var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();
                    //Excluir foto antiga se não for foto padrão
                    if(!Regex.IsMatch(usu.Foto, @"^[a-z.]+$"))
                    {
                        Funcoes.Upload.ExcluirArquivo(Request.PhysicalApplicationPath + "Uploads\\" + usu.Foto);
                    }
                    usu.Foto = nomearq;
                    db.Usuario.AddOrUpdate(usu);
                    db.SaveChanges();
                    return Json("s");
                }
                else
                {
                    ModelState.AddModelError("", valor);
                    TempData["MSG"] = "error|" + valor;
                    return Json("n");
                }
            }
            else
            {
                TempData["MSG"] = "error|Escolha uma imagem primeiro";
                return Json("n");
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult EditarBiografia(string biografia)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();
            usu.Biografia = biografia;
            db.Usuario.AddOrUpdate(usu);
            db.SaveChanges();

            return Json('s');
        }
        [HttpPost]
        public JsonResult AdicionarTag(VMPerfil vmp)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            var tag = db.Tag.Where(t => t.Nome == vmp.PesquisaTag).ToList().FirstOrDefault();

            foreach (var item in db.UsuarioTag)
            {
                if (item.TagId == tag.Id && item.UsuarioId == usu.Id)
                {
                    return Json("n");
                }
            }
            var usutag = new UsuarioTag();
            usutag.TagId = tag.Id;
            usutag.UsuarioId = usu.Id;

            db.UsuarioTag.Add(usutag);
            db.SaveChanges();


            return Json(db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList().Select(x => new { x.Tag.Nome }));

        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult ExcluirTag(string texto, int id)
        {
            var tag = db.UsuarioTag.Where(x => x.Tag.Nome == texto).ToList().FirstOrDefault();
            db.UsuarioTag.Remove(tag);
            db.SaveChanges();

            var usu = db.Usuario.Where(t => t.Id == id).ToList().FirstOrDefault();

            return Json(db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList().Select(x => new { x.Tag.Nome }));
        }
        public ActionResult ExcluirProjetosSalvos(int id)
        {
            var pro = db.ProjetosSalvos.Find(id);
            db.ProjetosSalvos.Remove(pro);
            db.SaveChanges();

            return RedirectToAction("MeuPerfil");
        }
        public ActionResult CriarProjeto(HttpPostedFileBase arquivo, VMPerfil vmp)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            if (ModelState.IsValid)
            {
                Projeto pro = new Projeto();

                pro.Logo = "projeto.svg";
                pro.Nome = vmp.NomeProjeto;
                pro.Descricao = vmp.Descricao;
                pro.Ativo = true;
                pro.DataCadastro = DateTime.Now;

                db.Projeto.AddOrUpdate(pro);
                db.SaveChanges();

                IntegrantesProjeto integrante = new IntegrantesProjeto();

                integrante.Adm = true;
                integrante.Ativo = true;
                integrante.ProjetoId = pro.Id;
                integrante.UsuarioID = usu.Id;

                db.IntegrantesProjeto.AddOrUpdate(integrante);
                db.SaveChanges();

                ProjetoTags tag = new ProjetoTags();

                tag.ProjetoId = pro.Id;
                tag.TagId = 1;

                db.ProjetoTags.AddOrUpdate(tag);
                db.SaveChanges();

                return RedirectToAction("MeuProjeto", new { id = pro.Id });
            }
            TempData["MSG"] = "error|Preencha os dois campos para criar um projeto";
            return RedirectToAction("MeuPerfil");
        }
        public ActionResult MeuProjeto(int id, int? idDenuncia)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            if (usu == null)
            {
                return RedirectToAction("Acesso", "Usuario");
            }

            Projeto pro = db.Projeto.Find(id);

            foreach (var item in pro.IntegrantesProjetos)
            {
                if (item.UsuarioID == usu.Id && item.Ativo == false)
                {
                    return RedirectToAction("VisitarProjeto", new { id });
                }
                if (item.UsuarioID == usu.Id || user[1] == "adm")
                {
                    VMProjeto vmp = new VMProjeto();

                    vmp.Id = pro.Id;
                    vmp.Nome = pro.Nome;
                    vmp.Descricao = pro.Descricao;
                    vmp.Logo = pro.Logo;
                    vmp.ProjetoTags = pro.ProjetoTags;
                    vmp.ArquivosProjetos = pro.ArquivosProjetos;
                    vmp.IntegrantesProjetos = pro.IntegrantesProjetos.Where(x => x.Ativo == true).ToList();
                    vmp.EuIntegrante = vmp.IntegrantesProjetos.Where(x => x.UsuarioID == usu.Id).FirstOrDefault();
                    vmp.Ativo = pro.Ativo;

                    if (user[1] == "adm")
                    {
                        vmp.Adm = true;

                        if (idDenuncia != null)
                        {
                            vmp.IdDenuncia = (int)idDenuncia;
                        }
                        return View(vmp);
                    }

                    int result = DateTime.Compare(pro.Punicao, DateTime.Now);
                    if (result > 0)
                    {
                        int dias = (pro.Punicao - DateTime.Now).Days;
                        if (dias == 0)
                        {
                            TempData["MSG"] = "error|O projeto foi bloqueado temporariamente por infringir as regras da comunidade. O projeto será desbloqueado em menos de 24 horas.";
                        }
                        else
                        {
                            TempData["MSG"] = "error|O projeto foi bloqueado temporariamente por infringir as regras da comunidade. O projeto será desbloqueado em " + dias + "dia(s)";

                        }
                        return RedirectToAction("MeuPerfil");
                    }

                    return View(vmp);
                }
            }

            return RedirectToAction("VisitarProjeto", new { id });
        }
        public ActionResult VisitarProjeto(int id)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            Projeto pro = db.Projeto.Find(id);
            VMProjeto vmp = new VMProjeto();

            vmp.Id = pro.Id;
            vmp.Nome = pro.Nome;
            vmp.Descricao = pro.Descricao;
            vmp.Logo = pro.Logo;
            vmp.ProjetoTags = pro.ProjetoTags;
            vmp.ArquivosProjetos = pro.ArquivosProjetos;
            vmp.IntegrantesProjetos = pro.IntegrantesProjetos.Where(x => x.Ativo == true).ToList();
            vmp.ProjetosSalvos = db.ProjetosSalvos.Where(x => x.UsuarioId == usu.Id).ToList();

            return View(vmp);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult EditarLogo(HttpPostedFileBase arq, VMProjeto vmp)
        {
            if (arq != null)
            {
                Funcoes.Upload.CriarDiretorio();
                string nomearq = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(arq.FileName);
                string valor = Funcoes.Upload.UploadImagem(arq, nomearq);
                if (valor == "sucesso")
                {
                    Projeto pro = db.Projeto.Find(vmp.Id);
                    //Excluir foto antiga
                    if (pro.Logo != "projeto.svg")
                    {
                        Funcoes.Upload.ExcluirArquivo(Request.PhysicalApplicationPath + "Uploads\\" + pro.Logo);
                    }
                    pro.Logo = nomearq;
                    db.Projeto.AddOrUpdate(pro);
                    db.SaveChanges();
                    TempData["MSG"] = "success|Logo alterada com sucesso!";
                    return Json('s');
                }
                else
                {
                    ModelState.AddModelError("", valor);
                    TempData["MSG"] = "error|" + valor;
                    return Json('n');
                }
            }
            else
            {
                TempData["MSG"] = "error|Escolha uma imagem primeiro";
                return Json('n');
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult EditarDescricao(VMProjeto vmp)
        {
            var pro = db.Projeto.Where(t => t.Id == vmp.Id).ToList().FirstOrDefault();

            if (vmp.Descricao == null)
            {
                return Json('n');
            }
            pro.Descricao = vmp.Descricao;
            db.Projeto.AddOrUpdate(pro);
            db.SaveChanges();

            return Json('s');
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult EditarNomeProjeto(VMProjeto vmp)
        {
            var pro = db.Projeto.Where(t => t.Id == vmp.Id).ToList().FirstOrDefault();

            if (vmp.Nome == null)
            {
                return Json('n');
            }
            pro.Nome = vmp.Nome;
            db.Projeto.AddOrUpdate(pro);
            db.SaveChanges();

            return Json('s');
        }
        [HttpPost]
        public JsonResult AdicionarTagProjeto(VMProjeto vmp)
        {
            var tag = db.Tag.Where(t => t.Nome == vmp.PesquisaTag).ToList().FirstOrDefault();

            foreach (var item in db.ProjetoTags)
            {
                if (item.TagId == tag.Id && item.ProjetoId == vmp.Id)
                {
                    return Json("n");
                }
            }

            var protag = new ProjetoTags();
            protag.TagId = tag.Id;
            protag.ProjetoId = vmp.Id;

            db.ProjetoTags.Add(protag);
            db.SaveChanges();

            var pro = db.Projeto.Where(t => t.Id == vmp.Id).ToList().FirstOrDefault();

            return Json(db.ProjetoTags.Where(x => x.ProjetoId == pro.Id).ToList().Select(x => new { x.Tag.Id, x.Tag.Nome }));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult ExcluirTagProjeto(string texto, int id)
        {
            var tag = db.ProjetoTags.Where(x => x.Tag.Nome == texto).ToList().FirstOrDefault();
            db.ProjetoTags.Remove(tag);
            db.SaveChanges();

            var pro = db.Projeto.Where(t => t.Id == id).ToList().FirstOrDefault();

            return Json(db.ProjetoTags.Where(x => x.ProjetoId == pro.Id).ToList().Select(x => new { x.Tag.Id, x.Tag.Nome }));
        }
        public ActionResult AdicionarUpload(HttpPostedFileBase arq, VMProjeto vmp)
        {
            if (arq != null)
            {
                Funcoes.Upload.CriarDiretorio();
                string nomearq = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(arq.FileName);
                string valor = Funcoes.Upload.UploadPdf(arq, nomearq);
                if (valor == "sucesso")
                {
                    var arqpro = new ArquivosProjeto();
                    arqpro.ProjetoId = vmp.Id;
                    arqpro.Nome = vmp.NomeArquivo;
                    arqpro.Arquivo = nomearq;

                    db.ArquivosProjeto.Add(arqpro);
                    db.SaveChanges();
                    TempData["MSG"] = "success|Documento adicionado!";
                    return RedirectToAction("MeuProjeto", new { id = vmp.Id });
                }
                else
                {
                    ModelState.AddModelError("", valor);
                    TempData["MSG"] = "error|" + valor;
                    return RedirectToAction("MeuProjeto", new { id = vmp.Id });
                }
            }
            else
            {
                TempData["MSG"] = "error|Escolha uma imagem primeiro";
                return RedirectToAction("MeuProjeto", new { id = vmp.Id });
            }
        }
        public ActionResult RemoverUpload(int idProjeto, int idArquivo)
        {
            ArquivosProjeto arq = db.ArquivosProjeto.Find(idArquivo);

            Funcoes.Upload.ExcluirArquivo(Request.PhysicalApplicationPath + "Uploads\\" + arq.Arquivo);

            db.ArquivosProjeto.Remove(arq);
            db.SaveChanges();

            TempData["MSG"] = "success|Documento removido!";
            return RedirectToAction("MeuProjeto", new { id = idProjeto });
        }
            [HttpPost]
        public ActionResult AdicionarIntegrante(VMProjeto vmp)
        {
            Usuario usu = db.Usuario.Where(x => x.Email == vmp.PesquisaEmail).ToList().FirstOrDefault();
            var integrantes = db.IntegrantesProjeto.Where(x => x.ProjetoId == vmp.Id).ToList();

            var integrante = new IntegrantesProjeto();

            if (usu == null || usu.Ativo == false)
            {
                TempData["MSG"] = "error|E-mail não encontrado";
                return RedirectToAction("MeuProjeto", new { id = vmp.Id });
            }
            foreach (var item in integrantes)
            {
                if (item.UsuarioID == usu.Id)
                {
                    if (item.Ativo == true)
                    {
                        TempData["MSG"] = "error|Integrante já adicionado";
                        return RedirectToAction("MeuProjeto", new { id = vmp.Id });
                    }
                    else
                    {
                        integrante = db.IntegrantesProjeto.Find(item.Id);
                        integrante.Ativo = true;
                        integrante.Inativo = null;
                        db.IntegrantesProjeto.AddOrUpdate(integrante);
                        db.SaveChanges();

                        return RedirectToAction("MeuProjeto", new { id = vmp.Id });
                    }

                }
            }

            integrante.Adm = false;
            integrante.Ativo = true;
            integrante.ProjetoId = vmp.Id;
            integrante.UsuarioID = usu.Id;

            db.IntegrantesProjeto.Add(integrante);
            db.SaveChanges();

            return RedirectToAction("MeuProjeto", new { id = vmp.Id });
        }

        public ActionResult ExcluirIntegrante(int id)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            var integrante = db.IntegrantesProjeto.Find(id);
            IntegrantesProjeto eu = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id && x.ProjetoId == integrante.ProjetoId).FirstOrDefault();
            int quantAdm = db.IntegrantesProjeto.Where(x => x.Adm == true && x.ProjetoId == integrante.ProjetoId).Count();
            int quant = db.IntegrantesProjeto.Where(x => x.ProjetoId == integrante.ProjetoId && x.Ativo == true).Count();

            if (eu.Adm == true || integrante.UsuarioID == usu.Id)
            {
                if (integrante.Adm == true)
                {
                    if (quantAdm > 1)
                    {
                        integrante.Ativo = false;
                        integrante.Inativo = "Usuário";
                        integrante.Adm = false;
                        db.IntegrantesProjeto.AddOrUpdate(integrante);
                        db.SaveChanges();

                        return RedirectToAction("MeuProjeto", new { id = integrante.ProjetoId });
                    }
                    else if (quant == 1)
                    {
                        integrante.Ativo = false;
                        integrante.Inativo = "Usuário";
                        db.IntegrantesProjeto.AddOrUpdate(integrante);

                        Projeto pro = db.Projeto.Find(integrante.ProjetoId);
                        pro.Ativo = false;
                        pro.Inativo = "Usuário";
                        db.Projeto.AddOrUpdate(pro);

                        db.SaveChanges();
                        return RedirectToAction("MeuPerfil");
                    }
                    else
                    {
                        TempData["MSG"] = "error|Primeiro, escolha um integrante para ser Adiministrador!";
                        return RedirectToAction("MeuProjeto", new { id = integrante.ProjetoId });
                    }

                }
                integrante.Ativo = false;
                integrante.Inativo = "Usuário";
                integrante.Adm = false;
                db.IntegrantesProjeto.AddOrUpdate(integrante);
                db.SaveChanges();

                return RedirectToAction("MeuProjeto", new { id = integrante.ProjetoId });
            }

            TempData["MSG"] = "error|Apenas os administradores podem remover um integrante!";
            return RedirectToAction("MeuProjeto", new { id = integrante.ProjetoId });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult AlterarAdm(int id)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            IntegrantesProjeto integrante = db.IntegrantesProjeto.Find(id);
            IntegrantesProjeto eu = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id && x.ProjetoId == integrante.ProjetoId).FirstOrDefault();
            int quantAdm = db.IntegrantesProjeto.Where(x => x.Adm == true && x.ProjetoId == integrante.ProjetoId).Count();

            if (eu.Adm == true)
            {
                if (integrante.Adm)
                {
                    if (quantAdm > 1)
                    {
                        integrante.Adm = false;
                    }
                    else
                    {
                        return Json("nn");
                    }
                }
                else
                {
                    integrante.Adm = true;
                }

                db.Entry(integrante).State = EntityState.Modified;
                db.SaveChanges();
                return Json(integrante.Adm ? "t" : "f");
            }
            else
            {
                return Json("n");
            }
        }
        public JsonResult AutoCompleteTags()
        {
            return Json(db.Tag.ToList());
        }
        public JsonResult TagsUsuario()
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            return Json(db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList().Select(x => new { x.Tag.Id, x.Tag.Nome }));
        }

        public JsonResult TagsProjeto(int id)
        {
            var pro = db.Projeto.Where(t => t.Id == id).ToList().FirstOrDefault();

            return Json(db.ProjetoTags.Where(x => x.ProjetoId == pro.Id).ToList().Select(x => new { x.Tag.Id, x.Tag.Nome }));
        }
        public JsonResult SalvarProjeto(int id)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            var prosal = new ProjetosSalvos();

            prosal.ProjetoId = id;
            prosal.UsuarioId = usu.Id;

            db.ProjetosSalvos.AddOrUpdate(prosal);
            db.SaveChanges();

            return Json("s");
        }
        public JsonResult RemoverProjetoSalvo(int id)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            var prosal = db.ProjetosSalvos.Where(x => x.UsuarioId == usu.Id && x.ProjetoId == id).FirstOrDefault();

            db.ProjetosSalvos.Remove(prosal);
            db.SaveChanges();

            return Json("s");
        }
        public ActionResult VisualizarPdf(int id)
        {
            VMPdf vm = new VMPdf();

            vm.ArquivosProjetos = db.ArquivosProjeto.Find(id);

            return View(vm);
        }
        [HttpPost]
        public ActionResult DenunciarUsuario(VMPerfil vmp)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            Denuncias den = new Denuncias();
            den.UsuarioDenuncianteId = usu.Id;
            den.UsuarioDenunciadoId = vmp.Id;
            den.DataCadastro = DateTime.Now.ToString();
            den.Status = "Esperando análise";
            if (vmp.MotivoDenuncia == null)
            {
                TempData["MSG"] = "error|Selecione uma Opção!";
                return RedirectToAction("VisitarPerfil", new { id = vmp.Id });
            }
            den.Motivo = vmp.MotivoDenuncia;
            den.Observacao = vmp.Observacao;

            db.Denuncias.Add(den);
            db.SaveChanges();

            TempData["MSG"] = "success|Agradeçemos sua contribuição!";

            return RedirectToAction("VisitarPerfil", new { id = vmp.Id });
        }
        public ActionResult DenunciarProjeto(VMProjeto vmp)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            Denuncias den = new Denuncias();
            den.UsuarioDenuncianteId = usu.Id;
            den.ProjetoDenunciadoId = vmp.Id;
            den.DataCadastro = DateTime.Now.ToString();
            den.Status = "Esperando análise";
            den.Motivo = vmp.MotivoDenuncia;
            den.Observacao = vmp.Observacao;

            db.Denuncias.Add(den);
            db.SaveChanges();

            TempData["MSG"] = "success|Agradeçemos sua contribuição!";

            return RedirectToAction("MeuProjeto", new { id = vmp.Id });
        }
    }

}