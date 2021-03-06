﻿using Sistema.Models;
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
            vma.EmailAdm = usu.Email;

            var projetosDenunciados = db.Denuncias.Where(x => x.ProjetoDenunciadoId != null && x.Status != "Concluído").OrderBy(x => x.ProjetoDenunciadoId).ToList();
            vma.DenunciasProjetos = FuncoesAdm.DenunciasAgrupadas(projetosDenunciados).OrderByDescending(x => x.QuantidadeDenuncias).ToList();

            var usuariosDenunciados = db.Denuncias.Where(x => x.UsuarioDenunciadoId != null && x.Status != "Concluído").OrderBy(x => x.UsuarioDenunciadoId).ToList();
            vma.DenunciasUsuarios = FuncoesAdm.DenunciasAgrupadas(usuariosDenunciados).OrderByDescending(x => x.QuantidadeDenuncias).ToList();

            vma.NumeroUsuarios = db.Usuario.Count();
            vma.NumeroProjetos = db.Projeto.Count();
            vma.Tags = db.Tag.OrderByDescending(x => x.Pesquisada).ToList();
            vma.QuantidadeTagsUsuarios = FuncoesAdm.QuantidadeTagsUsuarios(vma.Tags);
            vma.QuantidadeTagsProjetos = FuncoesAdm.QuantidadeTagsProjetos(vma.Tags);
            
            return View(vma);
        }
        public ActionResult Historico()
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var usu = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            VMAdm vma = new VMAdm();

            vma.AdmId = usu.Id;
            vma.NomeAdm = usu.Nome;
            vma.EmailAdm = usu.Email;

            vma.NumeroUsuarios = db.Usuario.Count();
            vma.NumeroProjetos = db.Projeto.Count();
            vma.Tags = db.Tag.OrderByDescending(x => x.Pesquisada).ToList();
            vma.QuantidadeTagsUsuarios = FuncoesAdm.QuantidadeTagsUsuarios(vma.Tags);
            vma.QuantidadeTagsProjetos = FuncoesAdm.QuantidadeTagsProjetos(vma.Tags);

            var projetosDenunciados = db.Denuncias.Where(x => x.ProjetoDenunciadoId != null && x.Status == "Concluído").OrderBy(x => x.ProjetoDenunciadoId).ToList();
            vma.DenunciasProjetos = FuncoesAdm.DenunciasAgrupadas(projetosDenunciados).OrderByDescending(x => x.QuantidadeDenuncias).ToList(); ;

            var usuariosDenunciados = db.Denuncias.Where(x => x.UsuarioDenunciadoId != null && x.Status == "Concluído").OrderBy(x => x.UsuarioDenunciadoId).ToList();
            vma.DenunciasUsuarios = FuncoesAdm.DenunciasAgrupadas(usuariosDenunciados).OrderByDescending(x => x.QuantidadeDenuncias).ToList(); ;

            return View(vma);
        }
        public ActionResult MaisDenunciasUsu(int id)
        {
            VMAdm vma = new VMAdm();
            vma.MaisDenunciasUsuarios = db.Denuncias.Where(x => x.UsuarioDenunciadoId == id && x.Status != "Concluído").ToList();

            if (vma.MaisDenunciasUsuarios.Count() == 0)
            {
                vma.MaisDenunciasUsuarios = db.Denuncias.Where(x => x.UsuarioDenunciadoId == id && x.Status == "Concluído").ToList();
            }

            var denuncias = vma.MaisDenunciasUsuarios.Select(z => new
            {
                DenuncianteUrl = "/Home/MeuPerfil/" + z.UsuarioDenuncianteId,
                DenuncianteNome = z.UsuarioDenunciante.Nome,
                DenuncianteFoto = z.UsuarioDenunciante.Foto,
                MotivoDenuncia = z.Motivo,
                ObsDenuncia = z.Observacao
            });

            return Json(denuncias);
        }
        [HttpPost]
        public JsonResult MaisDenunciasPro(int id)
        {
            VMAdm vma = new VMAdm();
            vma.MaisDenunciasProjetos = db.Denuncias.Where(x => x.ProjetoDenunciadoId == id && x.Status != "Concluído").ToList();

            if(vma.MaisDenunciasProjetos.Count() == 0)
            {
                vma.MaisDenunciasProjetos = db.Denuncias.Where(x => x.ProjetoDenunciadoId == id && x.Status == "Concluído").ToList();
            }

            var denuncias = vma.MaisDenunciasProjetos.Select(z => new
            {
                DenuncianteUrl = "/Home/MeuPerfil/" + z.UsuarioDenuncianteId,
                DenuncianteNome = z.UsuarioDenunciante.Nome,
                DenuncianteFoto = z.UsuarioDenunciante.Foto,
                MotivoDenuncia = z.Motivo,
                ObsDenuncia = z.Observacao
            }) ;

            return Json(denuncias);
        }        
        public ActionResult PenalizarProjeto(VMProjeto vmp)
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

            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var adm = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            var denuncias = db.Denuncias.Where(x => x.ProjetoDenunciadoId == pro.Id && x.Status != "Concluído").ToList();

            if(denuncias.Count() > 0)
            {
                foreach (var item in denuncias)
                {
                    Denuncias den = db.Denuncias.Find(item.Id);

                    den.AdmId = adm.Id;
                    den.Punicao = vmp.Punicao;
                    den.MotivoPunicao = vmp.MotivoPunicao;
                    den.DataPunicao = DateTime.Now;
                    den.Status = "Concluído";

                    db.Denuncias.AddOrUpdate(den);
                    db.SaveChanges();
                }
            }

            var proAdm = db.IntegrantesProjeto.Where(x => x.ProjetoId == pro.Id && x.Adm == true).ToList();

            if (vmp.Punicao > 0)
            {
                foreach (var item in proAdm)
                {
                    TempData["MSG"] = Funcoes.EnviarEmail(item.Usuario.Email,
                    "MOVE - Seu Projeto "+ pro.Nome +" sofreu uma Penalidade", "Seu projeto foi Desativado por " + vmp.Punicao + " dias, motivo: "+ vmp.MotivoPunicao);
                }
            }
            else if (vmp.MotivoPunicao != null)
            {
                foreach (var item in proAdm)
                {
                    TempData["MSG"] = Funcoes.EnviarEmail(item.Usuario.Email,
                    "MOVE - Comunicado sobre seu Projeto "+ pro.Nome +" ...", vmp.MotivoPunicao);
                }
            }
            

            TempData["MSG"] = "success|Penalidade Aplicada!";

            return RedirectToAction("Index");
        }
        public ActionResult DesativarProjeto(VMProjeto vmp)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var adm = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            Projeto pro = db.Projeto.Find(vmp.Id);
            pro.Ativo = false;
            db.Projeto.AddOrUpdate(pro);
            db.SaveChanges();

            var denuncias = db.Denuncias.Where(x => x.ProjetoDenunciadoId == pro.Id && x.Status != "Concluído").ToList();

            if (denuncias.Count() > 0)
            {
                foreach (var item in denuncias)
                {
                    Denuncias den = db.Denuncias.Find(item.Id);
                    den.AdmId = adm.Id;
                    den.Punicao = 0;
                    den.MotivoPunicao = vmp.MotivoPunicao;
                    den.DataPunicao = DateTime.Now;
                    den.Status = "Concluído";
                    den.Desativado = true;

                    db.Denuncias.AddOrUpdate(den);
                    db.SaveChanges();
                }
            }

            var proAdm = db.IntegrantesProjeto.Where(x => x.ProjetoId == pro.Id && x.Adm == true).ToList();

            foreach (var item in proAdm)
            {
                TempData["MSG"] = Funcoes.EnviarEmail(item.Usuario.Email,
                "MOVE - Seu projeto "+ pro.Nome +" foi Desativado!", vmp.MotivoPunicao);
            }

            TempData["MSG"] = "success|Projeto Desativado!";

            return RedirectToAction("Index");
        }
        public ActionResult PenalizarUsuario(VMPerfil vmp)
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

            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var adm = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

            var denuncias = db.Denuncias.Where(x => x.UsuarioDenunciadoId == usu.Id && x.Status != "Concluído").ToList();

            if (denuncias.Count() > 0)
            {
                foreach (var item in denuncias)
                {
                    Denuncias den = db.Denuncias.Find(item.Id);

                    den.AdmId = adm.Id;
                    den.Punicao = vmp.Punicao;
                    den.MotivoPunicao = vmp.MotivoPunicao;
                    den.DataPunicao = DateTime.Now;
                    den.Status = "Concluído";

                    db.Denuncias.AddOrUpdate(den);
                    db.SaveChanges();
                }
            }

            if (vmp.Punicao > 0)
            {
                TempData["MSG"] = Funcoes.EnviarEmail(usu.Email,
                "MOVE - Sua Conta foi Penalizada!", "Sua Conta foi Desativada por " + vmp.Punicao +" dias, motivo: "+ vmp.MotivoPunicao);
            }
            else if(vmp.MotivoPunicao != null)
            {
                TempData["MSG"] = Funcoes.EnviarEmail(usu.Email,
                "MOVE - Comunicado sobre sua conta...", vmp.MotivoPunicao);
            }

            TempData["MSG"] = "success|Penalidade Aplicada!";

            return RedirectToAction("Index");
        }
        public ActionResult DesativarUsuario(VMPerfil vmp)
        {
            string[] user = User.Identity.Name.Split('|');
            string email = user[0];
            var adm = db.Usuario.Where(t => t.Email == email).ToList().FirstOrDefault();

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

            TempData["MSG"] = Funcoes.EnviarEmail(usu.Email,
                "MOVE - Sua Conta foi Desativada!", vmp.MotivoPunicao);

            var denuncias = db.Denuncias.Where(x => x.UsuarioDenunciadoId == usu.Id && x.Status != "Concluído").ToList();

            if (denuncias.Count() > 0)
            {
                foreach (var item in denuncias)
                {
                    Denuncias den = db.Denuncias.Find(item.Id);
                    den.AdmId = adm.Id;
                    den.Punicao = 0;
                    den.MotivoPunicao = vmp.MotivoPunicao;
                    den.DataPunicao = DateTime.Now;
                    den.Status = "Concluído";
                    den.Desativado = true;

                    db.Denuncias.AddOrUpdate(den);
                    db.SaveChanges();
                }
            }

            TempData["MSG"] = "success|Conta Desativada!";

            return RedirectToAction("Index");
        }
    }
}