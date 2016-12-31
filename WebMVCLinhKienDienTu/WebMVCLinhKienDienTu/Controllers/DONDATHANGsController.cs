using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebDemo.Models;

namespace WebDemo.Controllers
{
    public class DONDATHANGsController : Controller
    {
        private QLLINHKIENEntities db = new QLLINHKIENEntities();

        // GET: DONDATHANGs
        public ActionResult Index()
        {
            var dONDATHANGs = db.DONDATHANGs.Include(d => d.AspNetUser);
            return View(DONDATHANGs.ToList());
        }
	}
}