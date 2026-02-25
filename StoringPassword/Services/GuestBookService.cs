using Microsoft.EntityFrameworkCore;
using StoringPassword.Models;
using StoringPassword.ViewModels;

namespace StoringPassword.Services
{
    public class GuestBookService
    {
        private readonly GuestBookContext _context;
        public GuestBookService(GuestBookContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<MessageModel>> GetAllMessagesModelAsync()
        {
            return await _context.Messages
                .Include(m => m.User)
                .OrderByDescending(m => m.DateTime)
                .Select(m => new MessageModel
                {
                    Id = m.Id,
                    UserLogin = m.User.Login,
                    Text = m.Text,
                    DateTime = m.DateTime
                })
                .ToListAsync();
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return await _context.Messages
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == message.Id);
        }
    }
}