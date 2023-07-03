using Domain.Models;
using System.Text.Json.Serialization;

namespace WebHashcat.ViewModels
{
    public class DataLookupTableViewModel
    {
        public string Hash { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Enums.HashType HashType { get; set; }
        public string? Password { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; }
    }
}
