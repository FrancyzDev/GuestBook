using Microsoft.AspNetCore.Mvc;
using StoringPassword.Models;
using StoringPassword.ViewModels;
using StoringPassword.Services;
using Microsoft.EntityFrameworkCore;

namespace StoringPassword.Controllers
{
    public class HomeController : Controller
    {
        private readonly GuestBookService _guestBookService;

        public HomeController(GuestBookService guestBookService)
        {
            _guestBookService = guestBookService;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _guestBookService.GetAllMessagesModelAsync();
            return View(messages);
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

            var user = await _guestBookService.GetUserByLoginAsync(login);
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

                await _guestBookService.CreateMessageAsync(message);
            }

            return RedirectToAction("Index");
        }
    }
}