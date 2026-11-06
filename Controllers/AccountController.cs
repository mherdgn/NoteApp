
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NoteApp.Data;
using NoteApp.Models;
using System.Linq;

namespace NoteApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // LOGIN GET
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Username == username);

            if (user == null || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Kullanıcı adı veya şifre yanlış!";
                return View();
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                password
            );

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                // Eski, açık metin şifrelerle uyumluluk
                if (user.Password != password)
                {
                    ViewBag.Error = "Kullanıcı adı veya şifre yanlış!";
                    return View();
                }

                // Başarılı girişten sonra şifreyi hash'le
                user.Password = _passwordHasher.HashPassword(user, password);
                _context.SaveChanges();
            }
            else if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _passwordHasher.HashPassword(user, password);
                _context.SaveChanges();
            }

            // Kullanıcı bilgilerini Session'a kaydet
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Notes");
        }

        // LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}
