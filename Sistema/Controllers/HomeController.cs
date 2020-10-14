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
            string[] user = User.Identity.Name.Split(' ');

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
            string[] user = User.Identity.Name.Split(' ');

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

                FormsAuthentication.SetAuthCookie(usu.Id + " " + usu.Email, false);
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
                FormsAuthentication.SetAuthCookie(usu.Id + " " + usu.Email, false);
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
    }
}