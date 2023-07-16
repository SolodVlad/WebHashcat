using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace WebHashcat.Models
{
    public class HashcatArguments
    {
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public AttackMode AttackMode { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string Hash { get; set; }
    }
}
