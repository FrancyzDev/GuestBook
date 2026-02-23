using Microsoft.AspNetCore.Mvc; 
using StoringPassword.Models;
using StoringPassword.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace StoringPassword.Controllers
{
    public class AccountController : Controller 
    {
        private readonly GuestBookContext _context;

        public AccountController(GuestBookContext context)
        {
            _context = context;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel login_model)
        {
            if (ModelState.IsValid) 
            {
                if (_context.Users.ToList().Count == 0)
                {
                    ModelState.AddModelError("", "Невірний логін або пароль!");
                    return View(login_model);
                }

                var users = _context.Users.Where(a => a.Login == login_model.Login);
                if (users.ToList().Count == 0)
                {
                    ModelState.AddModelError("", "Невірний логін або пароль!");
                    return View(login_model);
                }

                var user = users.First();
                string? salt = user.Salt;

                byte[] password = Encoding.Unicode.GetBytes(salt + login_model.Password);
                var md5 = MD5.Create();
                byte[] byteHash = md5.ComputeHash(password);

                var hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));
                if (user.Password != hash.ToString())
                {
                    ModelState.AddModelError("", "Невірний логін або пароль!");
                    return View(login_model);
                }

                HttpContext.Session.SetString("Login", user.Login);
                return RedirectToAction("Index", "Home");
            }
            return View(login_model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel reg)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Login == reg.Login);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Login", "Користувач з таким логіном вже існує");
                    return View(reg);
                }

                var user = new User();
                user.Login = reg.Login;


                byte[] saltbuf = new byte[16];
                var r = RandomNumberGenerator.Create();
                r.GetBytes(saltbuf);

                var sb = new StringBuilder(16);
                for (int i = 0; i < 16; i++)
                    sb.Append(string.Format("{0:X2}", saltbuf[i]));

                var salt = sb.ToString();

                byte[] password = Encoding.Unicode.GetBytes(salt + reg.Password);
                var md5 = MD5.Create();
                byte[] byteHash = md5.ComputeHash(password);

                var hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));

                user.Password = hash.ToString();
                user.Salt = salt;
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(reg);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}