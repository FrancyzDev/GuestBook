using Microsoft.AspNetCore.Mvc;
using StoringPassword.Models;
using StoringPassword.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace StoringPassword.Controllers
{
    public class HomeController : Controller
    {
        private readonly GuestBookContext _context;

        public HomeController(GuestBookContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var messages = _context.Messages
                .Include(m => m.User)
                .OrderByDescending(m => m.DateTime)
                .Select(m => new MessageModel
                {
                    Id = m.Id,
                    UserLogin = m.User.Login,
                    Text = m.Text,
                    DateTime = m.DateTime
                })
                .ToList();

            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMessage(string text)
        {
            var login = HttpContext.Session.GetString("Login");
            if (string.IsNullOrEmpty(login))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Login == login);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                var message = new Message
                {
                    User = user,
                    Text = text,
                    DateTime = DateTime.Now
                };

                _context.Messages.Add(message);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}