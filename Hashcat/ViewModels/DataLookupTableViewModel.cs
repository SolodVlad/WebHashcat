using Domain.Models;

namespace WebHashcat.ViewModels
{
    public class DataLookupTableViewModel
    {
        public string Hash { get; set; }
        public Enums.HashType HashType { get; set; }
        public string? Password { get; set; }
        public Status Status { get; set; }
    }
}
