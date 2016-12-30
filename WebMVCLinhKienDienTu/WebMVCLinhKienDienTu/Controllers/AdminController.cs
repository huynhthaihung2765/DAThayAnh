using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDemo.Models;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;
using System.IO;
namespace WebDemo.Controllers
{
    
    public class AdminController : Controller
    {
        private QLLINHKIENEntities db;

        public AdminController()
        {
            db = new QLLINHKIENEntities();
        }



        // GET: Admin
        [Authorize(Roles = "Admin,Mod")]
        public ActionResult Index()
        {
            return View();
        }

        ///// Quản lý thành viên
        [Authorize(Roles = "Admin")]
        public ActionResult showall() // hiển thị tất cả thành viên
        {
            var show = db.AspNetUsers.ToList();
            return View(show);
        }

        /// Quản lý Linh kiện
        
        public ActionResult ShowAllLK(int ? page) //Hiển thị tất cả Linh Kiện
        {
            // Tạo biến quy định số trang hiển thị
            int pageSize = 10;
            // Tạo biến số trang 
            int pageNum = (page ?? 1);
            var show = db.LINHKIENs.ToList();
            return View(show.ToPagedList(pageNum,pageSize));
        }
        
        public ActionResult DetailsLK(int id) // Hiển thị chi tiết Linh kIỆN
        {
            var details = db.LINHKIENs.FirstOrDefault(n => n.MaLK == id);
            if(details == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(details);
        }
         public ActionResult EditLK(int id)
        {
            
            var Edit = db.LINHKIENs.FirstOrDefault(n => n.MaLK == id);
            if (Edit == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLoai = new SelectList(db.LOAILKs.OrderBy(n => n.TENLLK), "MaLLK", "TENLLK", Edit.MaLLK);
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX",Edit.MaNSX);
            return View(Edit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLK(LINHKIEN lINHKIEN,HttpPostedFileBase fileUpload)
        {
            ViewBag.MaLoai = new SelectList(db.LOAILKs, "MaLLK", "TENLLK", lINHKIEN.MaLLK);
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs, "MaNSX", "TenNSX", lINHKIEN.MaNSX);
            if (fileUpload==null)
            {
                ViewBag.Thongbao = "lỗi";
                return View(lINHKIEN);
            }
            if (ModelState.IsValid)
            {
                db.Entry(lINHKIEN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ShowAllLK");
            }
            
            return View(lINHKIEN);
        }
    }
}