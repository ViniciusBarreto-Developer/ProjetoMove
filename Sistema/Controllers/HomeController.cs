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

            if (vmp.PesquisaTag != null)
            {
                vmp.ProjetoTags = db.ProjetoTags.Where(x => x.Tag.Nome == vmp.PesquisaTag).ToList();
            }
            //else
            //{
            //    List<ProjetoTags> recomenda = new List<ProjetoTags>();
            //    vmp.ProjetoTags = db.ProjetoTags.ToList();

            //    foreach (var pro in vmp.ProjetoTags)
            //    {
            //        foreach (var item in vmp.UsuarioTags)
            //        {
            //            if (pro.TagId == item.TagId)
            //            {
            //                recomenda.Add(new ProjetoTags() { Id = pro.Id, TagId = pro.TagId, ProjetoId = pro.ProjetoId });
            //                break;
            //            }
            //        }
            //    }
            //    vmp.ProjetoTags = recomenda;
            //}

            return View(vmp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Principal(VMPrincipal vmp)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            VMPrincipal vm = new VMPrincipal();

            if (usu == null)
            {
                if (vmp.PesquisaTag != null)
                {
                    vm.ProjetoTags = db.ProjetoTags.Where(x => x.Tag.Nome == vmp.PesquisaTag).ToList();
                    return View(vm);
                }
                return View();
            }

            vm.UsuarioId = usu.Id;
            vm.UsuarioTags = db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList();
            vm.ProjetosSalvos = db.ProjetosSalvos.Where(x => x.UsuarioId == usu.Id).ToList();

            if (vmp.PesquisaTag != null)
            {
                vm.ProjetoTags = db.ProjetoTags.Where(x => x.Tag.Nome == vmp.PesquisaTag).ToList();
            }

            return View(vm);
        }
        [HttpPost]
        public ActionResult PesquisarProjetos(VMPrincipal vmp)
        {
            vmp.ProjetoTags = db.ProjetoTags.Where(x => x.Tag.Nome == vmp.PesquisaTag).ToList();

            return RedirectToAction("Principal", new { vmp });
        }
        public ActionResult Cadastro()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro(Cadastro cad)
        {
            CaptchaResponse captchaResponse = Funcoes.ValidateCaptcha(Request["g-recaptcha-response"]);

            if (captchaResponse.Success && ModelState.IsValid)
            {
                if (db.Usuario.Where(x => x.Email == cad.Email).ToList().Count > 0)
                {
                    ModelState.AddModelError("", "E-mail já cadastrado");
                    return View(cad);
                }
                if (db.Usuario.Where(x => x.EmailRecuperacao == cad.EmailRecuperacao).ToList().Count > 0)
                {
                    ModelState.AddModelError("", "E-mail de Recuperação já cadastrado");
                    return View(cad);
                }
                if (cad.Email == cad.EmailRecuperacao)
                {
                    ModelState.AddModelError("", "O E-mail de Recuperação não pode ser igual ao E-mail");
                    return View(cad);
                }
                if (Funcoes.ValidateCPF(cad.Cpf) == false)
                {
                    ModelState.AddModelError("", "O CPF não é válido");
                    return View(cad);
                }
                Usuario usu = new Usuario();
                usu.Nome = cad.Nome;
                usu.Celular = cad.Celular;
                usu.DataNascimento = cad.DataNascimento;
                usu.Cpf = cad.Cpf;
                usu.Email = cad.Email;
                usu.EmailRecuperacao = cad.EmailRecuperacao;
                usu.Senha = Funcoes.HashTexto(cad.Senha, "SHA512");
                usu.Biografia = "";
                usu.ativo = true;

                db.Usuario.Add(usu);
                db.SaveChanges();
                TempData["MSG"] = "success|Cadastro Concluído!";
                return RedirectToAction("Acesso");

            }
            else if (captchaResponse.Success == false)
            {
                ModelState.AddModelError("", "reCAPTCHA Inválido");
                return View();
            }
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult ValidarEmail(string email)
        {
            string[] user = User.Identity.Name.Split('|');
            string emailAtual = user[0];
            Usuario usuAtual = db.Usuario.Where(t => t.Email == emailAtual).ToList().FirstOrDefault();

            if (usuAtual == null)
            {
                Usuario u = db.Usuario.Where(t => t.Email == email).FirstOrDefault();
                if (u == null)
                {
                    return Json("n");
                }

                return Json("s");
            }

            if (usuAtual.Email == email)
            {
                return Json("n");
            }
            if (db.Usuario.Where(x => x.Email == email).Count() > 0)
            {
                return Json("s");
            }
            return Json("n");
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult ValidarEmailRecuperacao(string email)
        {
            string[] user = User.Identity.Name.Split('|');
            string emailAtual = user[0];
            Usuario usuAtual = db.Usuario.Where(t => t.Email == emailAtual).ToList().FirstOrDefault();

            if (usuAtual == null)
            {
                Usuario u = db.Usuario.Where(t => t.EmailRecuperacao == email).FirstOrDefault();
                if (u == null)
                {
                    return Json("n");
                }

                return Json("s");
            }

            if (usuAtual.EmailRecuperacao == email)
            {
                return Json("n");
            }
            if (db.Usuario.Where(x => x.EmailRecuperacao == email).Count() > 0)
            {
                return Json("s");
            }
            return Json("n");
        }
        public ActionResult Acesso()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Acesso(Acesso ace)
        {
            string senhacrip = Funcoes.HashTexto(ace.Senha, "SHA512");
            Usuario usu = db.Usuario.Where(t => t.Email == ace.Email && t.Senha ==
            senhacrip).ToList().FirstOrDefault();
            if (usu != null && usu.ativo != false)
            {
                FormsAuthentication.SetAuthCookie(usu.Email + "|" + usu.Nome, false);

                return RedirectToAction("Principal", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Usuário/Senha inválidos");
                return View();
            }
        }
        public ActionResult Sair()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Principal");
        }
        public ActionResult EditarCadastro()
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];

            Usuario usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();
            EditarCadastro edit = new EditarCadastro();

            edit.Nome = usu.Nome;
            edit.Celular = usu.Celular;
            edit.DataNascimento = usu.DataNascimento;
            edit.Cpf = usu.Cpf;
            edit.Email = usu.Email;
            edit.EmailRecuperacao = usu.EmailRecuperacao;
            edit.Senha = null;
            edit.ConfirmarNovaSenha = null;

            return View(edit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCadastro(EditarCadastro edit)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];

            if (ModelState.IsValid)
            {
                Usuario usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

                if (Funcoes.HashTexto(edit.SenhaAtual, "SHA512") == usu.Senha)
                {
                    if (edit.Email == edit.EmailRecuperacao)
                    {
                        ModelState.AddModelError("", "O E-mail de Recuperação não pode ser igual ao E-mail");
                        return View(edit);
                    }
                    if (usu.Email != edit.Email)
                    {
                        if (db.Usuario.Where(x => x.Email == edit.Email).ToList().Count > 0)
                        {
                            ModelState.AddModelError("", "E-mail já cadastrado");
                            return View(edit);
                        }
                    }
                    if (usu.EmailRecuperacao != edit.EmailRecuperacao)
                    {
                        if (db.Usuario.Where(x => x.EmailRecuperacao == edit.EmailRecuperacao).ToList().Count > 0)
                        {
                            ModelState.AddModelError("", "E-mail de Recuperação já cadastrado");
                            return View(edit);
                        }
                    }

                    if (Funcoes.ValidateCPF(edit.Cpf) == false)
                    {
                        ModelState.AddModelError("", "O CPF não é válido");
                        return View(edit);
                    }
                    usu.Nome = edit.Nome;
                    usu.Celular = edit.Celular;
                    usu.DataNascimento = edit.DataNascimento;
                    usu.Cpf = edit.Cpf;
                    edit.Email = usu.Email;
                    edit.EmailRecuperacao = usu.EmailRecuperacao;

                    if (edit.Senha != null)
                    {
                        usu.Senha = Funcoes.HashTexto(edit.Senha, "SHA512");
                    }

                    FormsAuthentication.SetAuthCookie(usu.Email + "|" + usu.Nome, false);

                    db.Usuario.AddOrUpdate(usu);
                    db.SaveChanges();
                    TempData["MSG"] = "success|Seus dados foram atualizados!";
                    return RedirectToAction("MeuPerfil");
                }
                else
                {
                    TempData["MSG"] = "error|Senha atual errada";
                    return RedirectToAction("EditarCadastro");
                }

            }
            return View(edit);
        }
        public ActionResult ExcluirConta(EditarCadastro edit)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();
            if (edit.SenhaAtual == null)
            {
                TempData["MSG"] = "error|Preencha o campo Senha Atual";
                return RedirectToAction("EditarCadastro");
            }
            if (Funcoes.HashTexto(edit.SenhaAtual, "SHA512") == usu.Senha)
            {
                usu.ativo = false;
                db.Usuario.AddOrUpdate(usu);
                db.SaveChanges();

                FormsAuthentication.SignOut();
                return RedirectToAction("Principal");
            }
            TempData["MSG"] = "error|Senha atual errada";
            return RedirectToAction("EditarCadastro");
        }
        public ActionResult Email()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Email(Mensagem msg)
        {
            if (ModelState.IsValid)
            {
                TempData["MSG"] = Funcoes.EnviarEmail(msg.Email,
                msg.Assunto, msg.CorpoMsg);
            }
            else
            {
                TempData["MSG"] = "warning|Preencha todos os campos";
            }
            return View(msg);
        }
        public ActionResult EsqueceuSenha()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EsqueceuSenha(EsqueceuSenha esq)
        {
            if (ModelState.IsValid)
            {
                Contexto db = new Contexto();
                var usu = db.Usuario.Where(x => x.Email == esq.Email).ToList().FirstOrDefault();
                if (usu != null)
                {
                    usu.Hash = Funcoes.Codifica(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss.ffff"));
                    db.Entry(usu).State = EntityState.Modified;
                    db.SaveChanges();
                    string msg = "<h3>Sistema</h3>";
                    msg += "Para alterar sua senha <a href='http://localhost:55455/Home/Redefinir/" + usu.Hash + "'target = '_blank' > clique aqui </ a > ";
                    Funcoes.EnviarEmail(usu.Email, "Redefinição de senha", msg);
                    TempData["MSG"] = "success|Solicitação de redefinição de Senha feita com sucesso!";
                    return RedirectToAction("Principal");
                }
                TempData["MSG"] = "error|E-mail não encontrado";
                return View();
            }
            TempData["MSG"] = "warning|Preencha todos os campos";
            return View();
        }
        public ActionResult Redefinir(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                Contexto db = new Contexto();
                var usu = db.Usuario.Where(x => x.Hash == id).ToList().FirstOrDefault();
                if (usu != null)
                {
                    try
                    {
                        DateTime dt = Convert.ToDateTime(Funcoes.Decodifica(usu.Hash));
                        if (dt > DateTime.Now)
                        {
                            RedefinirSenha red = new RedefinirSenha();
                            red.Hash = usu.Hash;
                            red.Email = usu.Email;
                            return View(red);
                        }
                        TempData["MSG"] = "warning|Esse link já expirou!";
                        return RedirectToAction("Principal");
                    }
                    catch
                    {
                        TempData["MSG"] = "error|Hash inválida!";
                        return RedirectToAction("Principal");
                    }
                }
                TempData["MSG"] = "error|Hash inválida!";
                return RedirectToAction("Principal");
            }
            TempData["MSG"] = "error|Acesso inválido!";
            return RedirectToAction("Principal");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Redefinir(RedefinirSenha red)
        {
            if (ModelState.IsValid)
            {
                Contexto db = new Contexto();
                var usu = db.Usuario.Where(x => x.Hash == red.Hash).ToList().FirstOrDefault();
                if (usu != null)
                {
                    usu.Hash = null;
                    usu.Senha = Funcoes.HashTexto(red.Senha, "SHA512");
                    db.Entry(usu).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["MSG"] = "success|Senha redefinida com sucesso!";
                    return RedirectToAction("Principal");
                }
                TempData["MSG"] = "error|E-mail não encontrado";
                return View(red);
            }
            TempData["MSG"] = "warning|Preencha todos os campos";
            return View(red);
        }
        public ActionResult MeuPerfil()
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];

            if (user[0] == null || user[0] == "")
            {
                return RedirectToAction("Principal");
            }
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            VMPerfil vmp = new VMPerfil();

            vmp.Id = usu.Id;
            vmp.Biografia = usu.Biografia;
            vmp.Nome = usu.Nome;
            vmp.Email = usu.Email;
            vmp.Foto = usu.Foto;
            vmp.UsuarioTags = db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList();
            vmp.IntegrantesProjetos = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id).ToList();
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

            VMPerfil vmp = new VMPerfil();

            vmp.Id = usu.Id;
            vmp.Biografia = usu.Biografia;
            vmp.Nome = usu.Nome;
            vmp.Email = usu.Email;
            vmp.Foto = usu.Foto;
            vmp.UsuarioTags = db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList();
            vmp.IntegrantesProjetos = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id).ToList();
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
                    //Excluir foto antiga
                    Funcoes.Upload.ExcluirArquivo(Request.PhysicalApplicationPath + "Uploads\\" + usu.Foto);
                    usu.Foto = nomearq;
                    db.Usuario.AddOrUpdate(usu);
                    db.SaveChanges();
                    //return RedirectToAction("MeuPerfil");
                    return Json("s");
                }
                else
                {
                    ModelState.AddModelError("", valor);
                    TempData["MSG"] = "error|" + valor;
                    //return RedirectToAction("MeuPerfil");
                    return Json("n");
                }
            }
            else
            {
                TempData["MSG"] = "error|Escolha uma imagem primeiro";
                //return RedirectToAction("MeuPerfil");
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
        public ActionResult AdicionarTag(VMPerfil vmp)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            if (db.Tag.Where(t => t.Nome == vmp.PesquisaTag).ToList().FirstOrDefault() == null)
            {
                TempData["MSG"] = "error|Tag não encontrada";
                return RedirectToAction("MeuPerfil");
            }

            var tag = db.Tag.Where(t => t.Nome == vmp.PesquisaTag).ToList().FirstOrDefault();

            foreach (var item in db.UsuarioTag)
            {
                if (item.TagId == tag.Id && item.UsuarioId == usu.Id)
                {
                    TempData["MSG"] = "error|Tag já cadastrada";
                    return RedirectToAction("MeuPerfil");
                }
            }
            var usutag = new UsuarioTag();
            usutag.TagId = tag.Id;
            usutag.UsuarioId = usu.Id;

            db.UsuarioTag.Add(usutag);
            db.SaveChanges();
            return RedirectToAction("MeuPerfil");

        }
        public ActionResult ExcluirTag(int id)
        {
            var tag = db.UsuarioTag.Find(id);
            db.UsuarioTag.Remove(tag);
            db.SaveChanges();

            return RedirectToAction("MeuPerfil");
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

                IntegrantesProjeto inte = new IntegrantesProjeto();

                inte.Adm = true;
                inte.ProjetoId = pro.Id;
                inte.UsuarioID = usu.Id;

                db.IntegrantesProjeto.AddOrUpdate(inte);
                db.SaveChanges();

                ProjetoTags tags = new ProjetoTags();

                tags.ProjetoId = pro.Id;
                tags.TagId = 1;

                db.ProjetoTags.AddOrUpdate(tags);
                db.SaveChanges();

                return RedirectToAction("MeuProjeto", new { id = pro.Id });
            }
            return View(vmp);
        }
        public ActionResult MeuProjeto(int id)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            if (usu == null)
            {
                return RedirectToAction("Acesso");
            }

            Projeto pro = db.Projeto.Find(id);

            foreach (var item in pro.IntegrantesProjetos)
            {
                if (item.UsuarioID == usu.Id)
                {
                    VMProjeto vm = new VMProjeto();

                    vm.Id = pro.Id;
                    vm.Nome = pro.Nome;
                    vm.Descricao = pro.Descricao;
                    vm.Logo = pro.Logo;
                    vm.ProjetoTags = pro.ProjetoTags;
                    vm.ArquivosProjetos = pro.ArquivosProjetos;
                    vm.IntegrantesProjetos = pro.IntegrantesProjetos;

                    return View(vm);
                }
            }
            return RedirectToAction("VisitarProjeto", new { id = id });
        }
        public ActionResult VisitarProjeto(int id)
        {
            Projeto pro = db.Projeto.Find(id);
            VMProjeto vm = new VMProjeto();

            vm.Id = pro.Id;
            vm.Nome = pro.Nome;
            vm.Descricao = pro.Descricao;
            vm.Logo = pro.Logo;
            vm.ProjetoTags = pro.ProjetoTags;
            vm.ArquivosProjetos = pro.ArquivosProjetos;
            vm.IntegrantesProjetos = pro.IntegrantesProjetos;

            return View(vm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult EditarLogo(HttpPostedFileBase arq, VMProjeto vmp)
        {
            string valor = "";

            if (arq != null)
            {
                Funcoes.Upload.CriarDiretorio();
                string nomearq = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(arq.FileName);
                valor = Funcoes.Upload.UploadImagem(arq, nomearq);
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
        [HttpPost]
        public ActionResult AdicionarTagProjeto(VMProjeto vmp)
        {

            if (db.Tag.Where(t => t.Nome == vmp.PesquisaTag).ToList().FirstOrDefault() == null)
            {
                TempData["MSG"] = "error|Tag não encontrada";
                return RedirectToAction("MeuProjeto", new { id = vmp.Id });
            }

            var tag = db.Tag.Where(t => t.Nome == vmp.PesquisaTag).ToList().FirstOrDefault();

            foreach (var item in db.ProjetoTags)
            {
                if (item.TagId == tag.Id && item.ProjetoId == vmp.Id)
                {
                    TempData["MSG"] = "error|Tag já cadastrada";
                    return RedirectToAction("MeuProjeto", new { id = vmp.Id });
                }
            }

            var protag = new ProjetoTags();
            protag.TagId = tag.Id;
            protag.ProjetoId = vmp.Id;

            db.ProjetoTags.Add(protag);
            db.SaveChanges();
            return RedirectToAction("MeuProjeto", new { id = vmp.Id });
        }
        public ActionResult ExcluirTagProjeto(int id)
        {
            var tag = db.ProjetoTags.Find(id);
            db.ProjetoTags.Remove(tag);
            db.SaveChanges();

            return RedirectToAction("MeuProjeto", new { id = tag.ProjetoId });
        }
        public ActionResult AdicionarUpload(HttpPostedFileBase arq, VMProjeto vmp)
        {
            string valor = "";

            if (arq != null)
            {
                Funcoes.Upload.CriarDiretorio();
                string nomearq = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(arq.FileName);
                string kk = Path.GetExtension(arq.FileName);
                valor = Funcoes.Upload.UploadPdf(arq, nomearq);
                if (valor == "sucesso")
                {
                    var arqpro = new ArquivosProjeto();
                    arqpro.ProjetoId = vmp.Id;
                    arqpro.Tipo = ArquivosProjeto.Tipos.Arquivo;
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
        [HttpPost]
        public ActionResult AdicionarIntegrante(VMProjeto vmp)
        {
            Usuario usu = db.Usuario.Where(x => x.Email == vmp.PesquisaEmail).ToList().FirstOrDefault();

            var inte = new IntegrantesProjeto();
            inte.Adm = false;
            inte.ProjetoId = vmp.Id;
            inte.UsuarioID = usu.Id;

            db.IntegrantesProjeto.Add(inte);
            db.SaveChanges();

            return RedirectToAction("MeuProjeto", new { id = vmp.Id });
        }

        public ActionResult ExcluirIntegrante(int id)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            var inte = db.IntegrantesProjeto.Find(id);
            IntegrantesProjeto eu = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id && x.ProjetoId == inte.ProjetoId).FirstOrDefault();
            int quantAdm = db.IntegrantesProjeto.Where(x => x.Adm == true && x.ProjetoId == inte.ProjetoId).Count();
            int quant = db.IntegrantesProjeto.Where(x => x.ProjetoId == inte.ProjetoId).Count();

            if (eu.Adm == true || inte.UsuarioID == usu.Id)
            {
                if (inte.Adm == true)
                {
                    if (quantAdm > 1)
                    {
                        db.IntegrantesProjeto.Remove(inte);
                        db.SaveChanges();

                        return RedirectToAction("MeuProjeto", new { id = inte.ProjetoId });
                    }
                    else if (quant == 1)
                    {
                        db.IntegrantesProjeto.Remove(inte);                        

                        Projeto pro = db.Projeto.Find(inte.ProjetoId);                        
                        pro.Ativo = false;
                        db.Projeto.AddOrUpdate(pro);

                        db.SaveChanges();
                        return RedirectToAction("MeuPerfil");
                    }
                    else
                    {
                        TempData["MSG"] = "error|Primeiro, escolha um integrante para ser Adiministrador!";
                        return RedirectToAction("MeuProjeto", new { id = inte.ProjetoId });
                    }

                }
                db.IntegrantesProjeto.Remove(inte);
                db.SaveChanges();

                return RedirectToAction("MeuProjeto", new { id = inte.ProjetoId });
            }

            TempData["MSG"] = "error|Apenas os administradores podem remover um integrante!";
            return RedirectToAction("MeuProjeto", new { id = inte.ProjetoId });
        }
        public JsonResult AlterarAdm(int id)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            IntegrantesProjeto inte = db.IntegrantesProjeto.Find(Convert.ToInt32(id));
            IntegrantesProjeto eu = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id && x.ProjetoId == inte.ProjetoId).FirstOrDefault();
            int quant = db.IntegrantesProjeto.Where(x => x.Adm == true).Count();

            if (eu.Adm == true)
            {
                if (inte.Adm && quant > 1)
                {
                    inte.Adm = false;
                }
                else
                {
                    inte.Adm = true;
                }

                db.Entry(inte).State = EntityState.Modified;
                db.SaveChanges();
                return Json(inte.Adm ? "t" : "f");
            }
            else
            {
                TempData["MSG"] = "error|Apenas os administradores podem alterar os adminitrasores!";
                return Json("n");
            }
        }

    }
}