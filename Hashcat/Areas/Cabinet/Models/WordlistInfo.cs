namespace WebHashcat.Areas.Cabinet.Models
{
    public class WordlistInfo
    {
        public string Name { get; set; }
        public double Size { get; set; }

        public override string ToString() => $"{Name} ({Size} МБ)";
    }
}
