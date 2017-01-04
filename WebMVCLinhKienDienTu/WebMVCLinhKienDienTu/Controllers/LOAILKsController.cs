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
    public class LOAILKsController : Controller
    {
        private QLLINHKIENEntities db = new QLLINHKIENEntities();

        // GET: LOAILKs
        public ActionResult Index()
        {
            return View(db.LOAILKs.ToList());
        }

      

        // GET: LOAILKs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LOAILKs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaLLK,TENLLK")] LOAILK lOAILK)
        {
            if (ModelState.IsValid)
            {
                db.LOAILKs.Add(lOAILK);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lOAILK);
        }

        // GET: LOAILKs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAILK lOAILK = db.LOAILKs.Find(id);
            if (lOAILK == null)
            {
                return HttpNotFound();
            }
            return View(lOAILK);
        }

        // POST: LOAILKs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLLK,TENLLK")] LOAILK lOAILK)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lOAILK).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lOAILK);
        }

        // GET: LOAILKs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAILK lOAILK = db.LOAILKs.Find(id);
            if (lOAILK == null)
            {
                return HttpNotFound();
            }
            return View(lOAILK);
        }

        // POST: LOAILKs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LOAILK lOAILK = db.LOAILKs.Find(id);
            db.LOAILKs.Remove(lOAILK);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
