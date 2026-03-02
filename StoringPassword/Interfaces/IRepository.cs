using StoringPassword.Models;
using StoringPassword.ViewModels;

namespace StoringPassword.Interfaces
{
    public interface IRepository
    {
        Task<User?> GetUserByLoginAsync(string login);
        Task<User> CreateUserAsync(User user);
        Task<List<MessageModel>> GetAllMessagesModelAsync();
        Task<Message> CreateMessageAsync(Message message);
    }
}