using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebHashcat.Models
{
    public class HashcatArguments
    {
        [Required(ErrorMessage = "Це обов'язкове поле")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttackMode AttackMode { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string Hash { get; set; }
    }
}
