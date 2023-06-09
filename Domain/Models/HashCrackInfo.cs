namespace Domain.Models
{
    public class HashCrackInfo
    {
        public string Hash { get; set; }
        public HashType HashType { get; set; }
        public Status Status { get; set; }
        public int Progress { get; set; }
        public string? Password { get; set; }
    }
}
