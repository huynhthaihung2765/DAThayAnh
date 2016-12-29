using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebDemo.Models;
using System.Collections.Generic;
using BotDetect.Web.Mvc;


namespace WebMVCLinhKienDienTu.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext context;
        QLLINHKIENEntities db = new QLLINHKIENEntities();
        public AccountController()
        {
            context = new ApplicationDbContext();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            QLLINHKIENEntities db = new QLLINHKIENEntities();
            AspNetUser user = db.AspNetUsers.FirstOrDefault(m => m.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("CustomError", "Tài khoản không tồn tại !!!");
                return View(model);
            }

            else
            {
                if (user.EmailConfirmed == false)
                {
                    ModelState.AddModelError("CustomError", "Tài khoản chưa xác thực !!!");
                    return View(model);
                }


            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        return RedirectToAction("Index", "LinhKien");
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu !!!");
                    return View(model);
            }

        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidation("CaptchaCode", "ExampleCaptcha", "Mã xác nhận không đúng!")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Hoten = model.Hoten, Diachi = model.Diachi, Gioitinh = model.Gioitinh, PhoneNumber = model.PhoneNumber };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false); // đăng nhập
                    await UserManager.AddToRoleAsync(user.Id, "User");
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Xác thực tài khoản", "Vui lòng xác thực tài khoản của bạn bằng cách nhấp vào <a href=\"" + callbackUrl + "\">đây</a>");
                    ViewBag.Thongbao = "Đăng ký thành công. Chúng tôi đã gửi cho bạn mail kích hoạt";
                    //return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // đăng ký quyền cho User

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult RegisterRole()
        {
            ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name");
            ViewBag.UserName = new SelectList(context.Users.ToList(), "UserName", "UserName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> RegisterRole(RegisterViewModel model, ApplicationUser user)
        {
            var userId = context.Users.Where(i => i.UserName == user.UserName).Select(s => s.Id);
            string updateId = "";
            foreach (var i in userId)
            {
                updateId = i.ToString();
            }
            //chèn role cho user            
            await this.UserManager.AddToRoleAsync(updateId, model.Name);
            return RedirectToAction("RegisterRole", "Account");

        }


        //Delete Role User

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteRole(string email)
        {
            string id = db.AspNetUsers.Where(n => n.Email == email).Select(x => x.Id).FirstOrDefault();
            var a = await UserManager.GetRolesAsync(id);
            var user = new RoleUser()
            {
                id = id,
                email = email

            };
            ViewBag.role = new SelectList(a);
            return View(user);
        }
        [HttpPost]
        public ActionResult DeleteRole(RoleUser ru)
        {
            UserManager.RemoveFromRole(ru.id, ru.role);
            return RedirectToAction("RegisterRole", "Account");
        }

        //Edit User

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditUser(string id)
        {
            var userId = await UserManager.FindByIdAsync(id);
            if (userId == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(userId);
        }
        [HttpPost]
        public ActionResult EditUser(ApplicationUser user)
        {

            //update user

            var userr = context.Users.Where(s => s.Id == user.Id).FirstOrDefault();
            if (userr != null)
            {
                userr.AccessFailedCount = user.AccessFailedCount; userr.Diachi = user.Diachi; userr.UserName = user.UserName; userr.Email = user.Email; userr.EmailConfirmed = user.EmailConfirmed;
                userr.Tuoi = user.Tuoi; userr.PhoneNumber = user.PhoneNumber; userr.PhoneNumberConfirmed = user.PhoneNumberConfirmed; userr.TwoFactorEnabled = user.TwoFactorEnabled; userr.LockoutEnabled = user.LockoutEnabled;
                userr.LockoutEndDateUtc = user.LockoutEndDateUtc; userr.Gioitinh = user.Gioitinh;
                context.SaveChanges();
                return RedirectToAction("showall", "Admin");
            }
            Response.StatusCode = 404;
            return null;


        }

        //Xoa User

        public async Task<ActionResult> DeleteUser(string id)
        {
            var userId = await UserManager.FindByIdAsync(id);
            if (userId == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var a = await UserManager.DeleteAsync(userId);


            return RedirectToAction("showall", "Admin");
        }

    }
}