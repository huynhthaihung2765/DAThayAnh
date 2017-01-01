using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebMVCLinhKienDienTu.Models;

namespace WebMVCLinhKienDienTu.Controllers
{
    public class DONDATHANGsController : Controller
    {
        private QLLINHKIENEntities db = new QLLINHKIENEntities();

        // GET: DONDATHANGs
        public ActionResult Index()
        {
            var dONDATHANGs = db.DONDATHANGs.Include(d => d.AspNetUser);
            return View(dONDATHANGs.ToList());
        }

        // GET: DONDATHANGs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONDATHANG dONDATHANG = db.DONDATHANGs.Find(id);
            if (dONDATHANG == null)
            {
                return HttpNotFound();
            }
            return View(dONDATHANG);
        }


    }
}