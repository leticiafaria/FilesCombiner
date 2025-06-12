using System.Text.Json.Serialization;

namespace FilesCombiner.Configuration
{
    public class LogEntry
    {
        public required DateTimeOffset Timestamp { get; set; }
        public required string Level { get; set; }
        public required string Message { get; set; }
        public required Dictionary<string, object> Properties { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Exception { get; set; }
    }
}
