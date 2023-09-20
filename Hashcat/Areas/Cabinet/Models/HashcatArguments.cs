using Domain.Models;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebHashcat.Areas.Cabinet.Models
{
    public class HashcatArguments
    {
        [Required(ErrorMessage = "Це обов'язкове поле")]
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public string HashType { get; set; }
        [Required(ErrorMessage = "Це обов'язкове поле")]
        public string Hash { get; set; }
    }
}
