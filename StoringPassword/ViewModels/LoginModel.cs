using System.ComponentModel.DataAnnotations;

namespace StoringPassword.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Логін обов'язковий")]
        [Display(Name = "Логін")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Пароль обов'язковий")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }
    }
}