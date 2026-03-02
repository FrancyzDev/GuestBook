using Microsoft.AspNetCore.Mvc;
using StoringPassword.Interfaces;
using StoringPassword.Models;

namespace StoringPassword.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllMessagesModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMessage(string text)
        {
            var login = HttpContext.Session.GetString("Login");
            if (string.IsNullOrEmpty(login))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _repository.GetUserByLoginAsync(login);
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

                await _repository.CreateMessageAsync(message);
            }

            return RedirectToAction("Index");
        }
    }
}