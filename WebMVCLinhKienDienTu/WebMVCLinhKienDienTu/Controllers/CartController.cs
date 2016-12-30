using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebDemo.Models;

namespace WebMVCLinhKienDienTu.Controllers
{
    public class CartController : Controller
    {
        QLLINHKIENEntities db = new QLLINHKIENEntities();

        string CartSession = "CartSession";
        // Lấy user hiên tại

        // GET: Cart
        string email = "";
        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
                decimal? a = 0;
                foreach (var item in list)
                {
                    a = a + (item.Product.Giaban * item.Quantity);
                }
                ViewBag.total = a.GetValueOrDefault(0).ToString("N0");
            }
            else
            {
                return RedirectToAction("ThongBaoGioHangTrong");
            }
            return View(list);
        }

        public ActionResult AddItem(int MaLK, int quantity, string returnUrl)
        {
            var product = db.LINHKIENs.Find(MaLK);
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.Product.MaLK == MaLK))
                {

                    foreach (var item in list)
                    {
                        if (item.Product.MaLK == MaLK)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //tạo mới đối tượng cart item
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                //Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                //tạo mới đối tượng cart item
                var item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                //Gán vào session
                Session[CartSession] = list;
            }
            return Redirect(returnUrl);
        }
        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Delete(int id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.Product.MaLK == id);
            if (sessionCart.Count == 0)
            {
                Session[CartSession] = null;
            }
            else
            {
                Session[CartSession] = sessionCart;
            }
            return Json(new
            {
                status = true
            });
        }
        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];

            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.MaLK == item.Product.MaLK);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public ActionResult Error()
        {
            return RedirectToAction("Login", "Account");
        }
        public ActionResult ThongBaoDatHang()
        {
            return View();
        }
        public ActionResult ThongBaoPayOnline()
        {
            return View();
        }

        public ActionResult ThongBaoGioHangTrong()
        {
            return View();
        }
        public void SendMail()
        {

            var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential("nguyenphuocnam.1995@gmail.com", "58948763"),       // Chỉnh email và password gửi đi
                EnableSsl = true,
            };

            var from = new MailAddress("nguyenphuocnam.1995@gmail.com", "Admin Web Linh Kiện By NamNguyen");
            var to = new MailAddress(Session[email].ToString());

            var mail = new MailMessage(from, to)
            {
                Subject = "Thông tin hàng đã đặt",
                Body = "Bạn đã đặt hàng thành công. Chúng tôi sẽ liên hệ bạn trong vòng 12 tiếng.Xin cảm ơn",
                IsBodyHtml = true,
            };

            client.Send(mail);
        }
        [Authorize]
        public ActionResult Payment(int flag)
        {
            var CurentID = User.Identity.GetUserId();
            if (Session[CartSession] == null)
            {
                return RedirectToAction("ThongBaoGioHangTrong", "Cart");
            }
            var user = (from a in db.AspNetUsers
                        where a.Id == CurentID
                        select new XacNhan()
                        {
                            Email = a.Email,
                            Hoten = a.Hoten,
                            Diachi = a.Diachi,
                            PhoneNumber = a.PhoneNumber,
                            flag = flag
                        }).FirstOrDefault();
            if (user == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(user);
        }
        [HttpPost]
        public ActionResult Payment(XacNhan xn)
        {
            Decimal? TongTien = 0;
            var UserCurent = db.AspNetUsers.Where(n => n.Email == xn.Email).FirstOrDefault();
            //Lưu lại thông tin

            UserCurent.Email = xn.Email;
            UserCurent.Hoten = xn.Hoten;
            UserCurent.Diachi = xn.Diachi;
            UserCurent.PhoneNumber = xn.PhoneNumber;
            Session[email] = xn.Email;
            db.SaveChanges();

            //Them Don hang

            DONDATHANG ddh = new DONDATHANG();
            var sessionCart = (List<CartItem>)Session[CartSession];
            ddh.Id = UserCurent.Id;
            ddh.Ngaydat = DateTime.Now;
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            db.DONDATHANGs.Add(ddh);
            db.SaveChanges();
            //Them chi tiet don hang 
            foreach (var item in sessionCart)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaLK = item.Product.MaLK;
                ctdh.Soluong = item.Quantity;
                ctdh.Dongia = (decimal)item.Product.Giaban * item.Quantity;
                TongTien += ctdh.Dongia;
                db.CHITIETDONTHANGs.Add(ctdh);
            }
            db.SaveChanges();
            Session[CartSession] = null;

            if (xn.flag == 0)
            {
                SendMail();
                return RedirectToAction("ThongBaoDatHang", "Cart");
            }
            else
            {

                return Redirect("https://www.baokim.vn/payment/product/version11?business=nguyenphuocnam.1995%40gmail.com&id=&order_description=&product_name=%C4%90%C6%A1n+Ha%CC%80ng+" + ddh.MaDonHang + "&product_price=" + TongTien + "&product_quantity=1&total_amount=" + TongTien + "&url_cancel=&url_detail=https%3A%2F%2Flocalhost%3A44364%2FCart&url_success=https%3A%2F%2Flocalhost%3A44364%2FCart%2FPayment%3Fflag%3D1");
            }
        }


        [Authorize]
        public ActionResult PaymentOnline()
        {
            var CurentID = User.Identity.GetUserId();
            var user = (from a in db.AspNetUsers
                        where a.Id == CurentID
                        select new XacNhan()
                        {
                            Email = a.Email,
                            Hoten = a.Hoten,
                            Diachi = a.Diachi,
                            PhoneNumber = a.PhoneNumber
                        }).FirstOrDefault();

            if (user == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(user);
        }
    }
}