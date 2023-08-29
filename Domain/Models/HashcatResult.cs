using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class HashcatResult
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; }
        public string HashType { get; set; }
        public string Hash { get; set; }
        public DateTime TimeStarted { get; set; }
        public string TimePassed { get; set; }
        public DateTime TimeEstimated { get; set; }
        public string TimeLeft { get; set; }
        public double Progress { get; set; }
        public string Value { get; set; }

        public string UserId { get; set; }
    }
}
