using Microsoft.AspNetCore.Mvc; 
using StoringPassword.Interfaces;
using StoringPassword.Models;
using StoringPassword.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace StoringPassword.Controllers
{
    public class AccountController : Controller 
    {
        private readonly IRepository _repository;

        public AccountController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel login_model)
        {
            if (!ModelState.IsValid)
            {
                return View(login_model);
            }
            var user = await _repository.GetUserByLoginAsync(login_model.Login);

            if (user == null)
            {
                ModelState.AddModelError("", "Невірний логін або пароль!");
                return View(login_model);
            }

            string salt = user.Salt;
            byte[] passwordBytes = Encoding.Unicode.GetBytes(salt + login_model.Password);

            using (var md5 = MD5.Create())
            {
                byte[] byteHash = md5.ComputeHash(passwordBytes);

                var hashBuilder = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                {
                    hashBuilder.Append(byteHash[i].ToString("X2"));
                }
                if (user.Password != hashBuilder.ToString())
                {
                    ModelState.AddModelError("", "Невірний логін або пароль!");
                    return View(login_model);
                }
            }
            HttpContext.Session.SetString("Login", user.Login);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel reg)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _repository.GetUserByLoginAsync(reg.Login);
                if (existingUser is not null)
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
                await _repository.CreateUserAsync(user);
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