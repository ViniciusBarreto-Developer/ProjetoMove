using Sistema.Models;
using System;
using System.Collections.Generic;
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
            return View();
        }
        public ActionResult DenunciaUsuario()
        {
            return View(db.Denuncias.Where(x => x.UsuarioDenunciadoId != null).ToList());
        }
        public ActionResult DenunciaProjeto()
        {
            return View(db.Denuncias.Where(x => x.ProjetoDenunciadoId != null).ToList());
        }
    }
}