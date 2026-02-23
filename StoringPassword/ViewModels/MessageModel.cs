namespace StoringPassword.ViewModels
{
    public class MessageModel
    {
        public int Id { get; set; }
        public string? UserLogin { get; set; }
        public string? Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}