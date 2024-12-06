using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEStore.Entities;
using MyEStore.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;

namespace MyEStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly MyeStoreContext _ctx;
        public CustomerController(MyeStoreContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult ExportCSV()
        {
            var sb = new List<string>();
            sb.Add("Email Address,First Name,Last Name,Address,Phone Number");
            var dskh = _ctx.KhachHangs.Select(kh => 
                $"\"{kh.Email}\",\"{kh.MaKh}\",\"{kh.HoTen}\",\"{kh.DiaChi}\",\"{kh.DienThoai}\"").ToList();
            sb.AddRange(dskh);
            return File(Encoding.UTF8.GetBytes(string.Join("\r\n", sb)), "text/csv", "khachhang.csv");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            var kh = _ctx.KhachHangs.SingleOrDefault(p => p.MaKh == model.UserName && p.MatKhau == model.Password);
            if (kh == null)
            {
                ViewBag.ThongBao = "Sai thông tin đăng nhập";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, kh.HoTen),
                new Claim(ClaimTypes.Email, kh.Email),
                new Claim("UserId", kh.MaKh),
                //làm động lấy role trong DB
                new Claim(ClaimTypes.Role, "Administrator")
            };
            var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimPrincipal);
            if (!string.IsNullOrEmpty(ReturnUrl)){
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("Profile", "Customer");
        }

        [Authorize]
        public IActionResult PurchaseOrder()
        {
            return View();
        }

        [Authorize(Roles ="Accountant")]
        public IActionResult Statistics()
        {
            return View();
        }

        [HttpGet("/Forbidden")]
        public IActionResult Forbidden()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
