using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sistema.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
            return View();
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
                Usuario usu = new Usuario();
                usu.Nome = cad.Nome;
                usu.NomeSocial = cad.NomeSocial;
                usu.DataNascimento = cad.DataNascimento;
                usu.Cpf = cad.Cpf;
                usu.Email = cad.Email;
                usu.EmailRecuperacao = cad.EmailRecuperacao;
                usu.Senha = Funcoes.HashTexto(cad.Senha, "SHA512");
                usu.Biografia = "     Aqui você pode colocar sua área de formação, seus interesses, e também as formas que as pessoas podem entrar em contato com você de forma mais rápida! Clique ao lado para personalizar sua Bio!";
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
                if (usu.NomeSocial == null || usu.NomeSocial == "")
                {
                    FormsAuthentication.SetAuthCookie(usu.Email + "|" + usu.Nome, false);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(usu.Email + "|" + usu.NomeSocial, false);
                }
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
            edit.NomeSocial = usu.NomeSocial;
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

                    if(Funcoes.ValidateCPF(edit.Cpf) == false)
                    {
                        ModelState.AddModelError("", "O CPF não é válido");
                        return View(edit);
                    }
                    usu.Nome = edit.Nome;
                    usu.NomeSocial = edit.NomeSocial;
                    usu.DataNascimento = edit.DataNascimento;
                    usu.Cpf = edit.Cpf;
                    edit.Email = usu.Email;
                    edit.EmailRecuperacao = usu.EmailRecuperacao;

                    if (edit.Senha != null)
                    {
                        usu.Senha = Funcoes.HashTexto(edit.Senha, "SHA512");
                    }

                    if (usu.NomeSocial == null || usu.NomeSocial == "")
                    {
                        FormsAuthentication.SetAuthCookie(usu.Email + "|" + usu.Nome, false);
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(usu.Email + "|" + usu.NomeSocial, false);
                    }

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
            vmp.NomeSocial = usu.NomeSocial;
            vmp.UsuarioTags = db.UsuarioTag.Where(x => x.UsuarioId == usu.Id).ToList();
            vmp.IntegrantesProjetos = db.IntegrantesProjeto.Where(x => x.UsuarioID == usu.Id).ToList();
            vmp.ProjetosSalvos = db.ProjetosSalvos.Where(x => x.UsuarioId == usu.Id).ToList();

            return View(vmp);
        }
        [HttpPost]
        public ActionResult EditarBiografia(Usuario usuario)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();
            usu.Biografia = usuario.Biografia;
            db.Usuario.AddOrUpdate(usu);
            db.SaveChanges();

            return RedirectToAction("MeuPerfil");
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
    }
}