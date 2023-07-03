using System.ComponentModel.DataAnnotations;

namespace WebHashcat.Areas.Identity.Models
{
    public class RegisterViewModel
    {
        [EmailAddress(ErrorMessage = "Введіть корректно пошту")]
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string ConfirmPassword { get; set; }
    }
}
