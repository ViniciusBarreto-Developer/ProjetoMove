using Sistema.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Sistema.Controllers
{
    public class HomeController : Controller
    {
        Contexto db = new Contexto();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EditarUsuario()
        {
            string[] user = User.Identity.Name.Split('*');

            Usuario usu = db.Usuario.Find(Convert.ToInt32(user[0]));
            Cadastro cad = new Cadastro();

            cad.Nome = usu.Nome;
            cad.NomeSocial = usu.NomeSocial;
            cad.DataNascimento = usu.DataNascimento;
            cad.Cpf = usu.Cpf;
            
            cad.Email = usu.Email;
            cad.EmailRecuperacao = usu.EmailRecuperacao;
            cad.Senha = "";
            cad.ConfirmaSenha = "";
            
            return View(cad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarUsuario(Cadastro cad)
        {
            string[] user = User.Identity.Name.Split('*');

            if (ModelState.IsValid)
            {
                Usuario usu = db.Usuario.Find(Convert.ToInt32(user[0]));

                usu.Nome = cad.Nome;
                usu.NomeSocial = cad.NomeSocial;
                usu.DataNascimento = cad.DataNascimento;
                usu.Cpf = cad.Cpf;

                usu.Email = cad.Email;
                usu.EmailRecuperacao = cad.EmailRecuperacao;
                usu.Senha = Funcoes.HashTexto(cad.Senha, "SHA512");

                if (usu.NomeSocial == null || usu.NomeSocial == "")
                {
                    FormsAuthentication.SetAuthCookie(usu.Id + "*" + usu.Nome, false);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(usu.Id + "*" + usu.NomeSocial, false);
                }

                db.Usuario.AddOrUpdate(usu);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(cad);
        }
        public ActionResult Sair()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
        public ActionResult Acesso()
        {            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Acesso(Acesso ace, string ReturnUrl)
        {
            string senhacrip = Funcoes.HashTexto(ace.Senha, "SHA512");
            Usuario usu = db.Usuario.Where(t => t.Email == ace.Email && t.Senha ==
            senhacrip).ToList().FirstOrDefault();
            if (usu != null)
            {
                if(usu.NomeSocial == null || usu.NomeSocial == "")
                {
                    FormsAuthentication.SetAuthCookie(usu.Id + "*" + usu.Nome, false);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(usu.Id + "*" + usu.NomeSocial, false);
                }                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Usuário/Senha inválidos");
                return View();
            }
        }
        public ActionResult Cadastro()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro(Cadastro cad)
        {
            if (ModelState.IsValid)
            {
                if (db.Usuario.Where(x => x.Email == cad.Email).ToList().Count > 0)
                {
                    ModelState.AddModelError("", "E-mail já cadastrado");
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

                db.Usuario.Add(usu);
                db.SaveChanges();
                return RedirectToAction("Acesso");
            }
            return View();
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
                    return RedirectToAction("Index");
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
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        TempData["MSG"] = "error|Hash inválida!";
                        return RedirectToAction("Index");
                    }
                }
                TempData["MSG"] = "error|Hash inválida!";
                return RedirectToAction("Index");
            }
            TempData["MSG"] = "error|Acesso inválido!";
            return RedirectToAction("Index");
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
                    return RedirectToAction("Index");
                }
                TempData["MSG"] = "error|E-mail não encontrado";
                return View(red);
            }
            TempData["MSG"] = "warning|Preencha todos os campos";
            return View(red);
        }
    }
}