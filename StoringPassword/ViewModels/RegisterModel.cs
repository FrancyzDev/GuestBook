using System.ComponentModel.DataAnnotations;

namespace StoringPassword.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Логін обов'язковий")]
        [Display(Name = "Логін")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Пароль обов'язковий")]
        [Display(Name = "Пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль повинен містити від 6 до 100 символів")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Підтвердження пароля обов'язкове")]
        [Display(Name = "Підтвердження пароля")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [DataType(DataType.Password)]
        public string? PasswordConfirm { get; set; }
    }
}