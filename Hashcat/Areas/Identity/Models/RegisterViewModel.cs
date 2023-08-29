using System.ComponentModel.DataAnnotations;

namespace WebHashcat.Areas.Identity.Models
{
    public class RegisterViewModel
    {
        [EmailAddress(ErrorMessage = "Введіть корректно пошту")]
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string Value { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string ConfirmValue { get; set; }
    }
}
