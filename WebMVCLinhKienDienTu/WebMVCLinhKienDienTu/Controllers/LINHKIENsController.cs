using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebDemo.Models;
using PagedList;
using System.IO;
namespace WebDemo.Controllers
{
    
    public class LINHKIENsController : Controller
    {
        private QLLINHKIENEntities db = new QLLINHKIENEntities();

        // GET: LINHKIENs
        [Authorize(Roles ="Mod,Admin")]
        public ActionResult Index(int ? page)
        {
            // Tạo biến quy định số trang hiển thị
            int pageSize = 10;
            // Tạo biến số trang 
            int pageNum = (page ?? 1);
            var show = db.LINHKIENs.OrderByDescending(n=>n.MaLK).ToList();
            return View(show.ToPagedList(pageNum, pageSize));
        }
        [Authorize(Roles = "Admin,Mod")]
        // GET: LINHKIENs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LINHKIEN lINHKIEN = db.LINHKIENs.Find(id);
            if (lINHKIEN == null)
            {
                return HttpNotFound();
            }
            return View(lINHKIEN);
        }

        // GET: LINHKIENs/Create
        [Authorize(Roles ="Mod,Admin")]
       
        public ActionResult Create()
        {
            ViewBag.MaLLK = new SelectList(db.LOAILKs, "MaLLK", "TENLLK");
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs, "MaNSX", "TenNSX");
            return View();
        }

        // POST: LINHKIENs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]    // Vượt qua lỗi khi save có tinymce        
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaLK,TenLK,Giaban,Mota,Anhbia,Ngaycapnhat,Soluongton,MaLLK,MaNSX")] LINHKIEN lINHKIEN,HttpPostedFileBase fileUpload)
        {
            ViewBag.MaLLK = new SelectList(db.LOAILKs, "MaLLK", "TENLLK", lINHKIEN.MaLLK);
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs, "MaNSX", "TenNSX", lINHKIEN.MaNSX);
            if (ModelState.IsValid)
            {
                if (lINHKIEN.Ngaycapnhat == null) { lINHKIEN.Ngaycapnhat = DateTime.Now; }
                if (fileUpload == null)
                {
                    ViewBag.Thongbao = "Chưa chọn hình ảnh!!!";
                    return View(lINHKIEN);
                }
                 
                    // Lưu tên file
                    var filename = Path.GetFileName(fileUpload.FileName);
                    // Lưu đường dẫn của file 
                    var path = Path.Combine(Server.MapPath("~/ImagesProduct"), filename);
                    //Kiểm tra hình đã tồn tại chưa 
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "hình ảnh đã tồn tại!!!";
                        return View(lINHKIEN);
                    }
                    else
                    {                      
                        //lưu hình ảnh  mới vào đường dẫn
                        fileUpload.SaveAs(path);
                    }
                    lINHKIEN.Anhbia = filename; //update tên file hình
                db.LINHKIENs.Add(lINHKIEN);
                    db.SaveChanges();             
            }
            return RedirectToAction("Index");



        }

        // GET: LINHKIENs/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LINHKIEN lINHKIEN = db.LINHKIENs.Find(id);
            if (lINHKIEN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaLLK = new SelectList(db.LOAILKs, "MaLLK", "TENLLK", lINHKIEN.MaLLK);
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs, "MaNSX", "TenNSX", lINHKIEN.MaNSX);
            return View(lINHKIEN);
        }

        // POST: LINHKIENs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]    // Vượt qua lỗi khi save có tinymce
        [ValidateAntiForgeryToken]        
        public ActionResult Edit( [Bind(Include = "MaLK,TenLK,Giaban,Mota,Anhbia,Ngaycapnhat,Soluongton,MaLLK,MaNSX")] LINHKIEN lINHKIEN,HttpPostedFileBase fileUpload)
        {
            
            ViewBag.MaLLK = new SelectList(db.LOAILKs, "MaLLK", "TENLLK", lINHKIEN.MaLLK);
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs, "MaNSX", "TenNSX", lINHKIEN.MaNSX);
            if (ModelState.IsValid)
            {
                string success = "Sửa thành công";
                if (lINHKIEN.Ngaycapnhat == null) { lINHKIEN.Ngaycapnhat = DateTime.Now; }
                if (fileUpload != null)
                {
                    // Lưu tên file
                    var filename = Path.GetFileName(fileUpload.FileName);
                    // Lưu đường dẫn của file 
                    var path = Path.Combine(Server.MapPath("~/ImagesProduct"), filename);
                    //Kiểm tra hình đã tồn tại chưa 
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "hình ảnh đã tồn tại";
                        return View(lINHKIEN);
                    }
                    else
                    {
                        var pathdel = Path.Combine(Server.MapPath("~/ImagesProduct"), lINHKIEN.Anhbia);
                        System.IO.File.Delete(pathdel);// Xóa hình hiện tại
                        //lưu hình ảnh  mới vào đường dẫn
                        fileUpload.SaveAs(path);
                    }
                    lINHKIEN.Anhbia = filename; //update tên file hình
                    db.Entry(lINHKIEN).State = EntityState.Modified; //update Data
                    db.SaveChanges();
                    ViewBag.Success = success;
                    return View(lINHKIEN);
                }                
                db.Entry(lINHKIEN).State = EntityState.Modified; //update Data
                db.SaveChanges();
                ViewBag.Success = success;
                return View(lINHKIEN);
            }
            
            return View(lINHKIEN);
        }

        // GET: LINHKIENs/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LINHKIEN lINHKIEN = db.LINHKIENs.Find(id);
            if (lINHKIEN == null)
            {
                return HttpNotFound();
            }
            return View(lINHKIEN);
        }

        // POST: LINHKIENs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LINHKIEN lINHKIEN = db.LINHKIENs.Find(id);
            db.LINHKIENs.Remove(lINHKIEN);
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
