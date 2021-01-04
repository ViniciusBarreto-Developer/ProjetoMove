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
    public class UsuarioController : Controller
    {
        Contexto db = new Contexto();
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
                usu.Ativo = true;
                usu.Foto = cad.Nome.ToLower()[0] + ".svg";

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
            if (usu != null && usu.Ativo == true)
            {
                int result = DateTime.Compare(usu.Punicao, DateTime.Now);
                if (result > 0)
                {
                    int dias = (usu.Punicao - DateTime.Now).Days;
                    TempData["MSG"] = "error|Sua conta foi bloqueada temporariamente por infringir as regras da comunidade. Sua conta será desbloqueada em " + dias + "dia(s)";
                    return View();
                }

                if (usu.Inativo == "Adm")
                {
                    TempData["MSG"] = "error|Sua conta foi bloqueada por infringir as regras da comunidade.";
                    return View();
                }

                if (usu.Adm == true)
                {
                    FormsAuthentication.SetAuthCookie(usu.Email + "|" + "adm", false);
                    return RedirectToAction("Index", "Adm");
                }

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
                    usu.Email = edit.Email;
                    usu.EmailRecuperacao = edit.EmailRecuperacao;

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
                usu.Ativo = false;
                usu.Inativo = "Usuário";
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

                FormsAuthentication.SignOut();
                return RedirectToAction("Principal");
            }
            TempData["MSG"] = "error|Senha atual errada";
            return RedirectToAction("EditarCadastro");
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
        public ActionResult EsqueceuSenhaRecuperacao()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EsqueceuSenhaRecuperacao(EsqueceuSenha esq)
        {
            if (ModelState.IsValid)
            {
                Contexto db = new Contexto();
                var usu = db.Usuario.Where(x => x.EmailRecuperacao == esq.Email).ToList().FirstOrDefault();
                if (usu != null)
                {
                    usu.Hash = Funcoes.Codifica(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss.ffff"));
                    db.Entry(usu).State = EntityState.Modified;
                    db.SaveChanges();
                    string msg = "<h3>Sistema</h3>";
                    msg += "Para alterar sua senha <a href='http://localhost:55455/Home/Redefinir/" + usu.Hash + "'target = '_blank' > clique aqui </ a > ";
                    Funcoes.EnviarEmail(usu.EmailRecuperacao, "Redefinição de senha", msg);
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
    }
}