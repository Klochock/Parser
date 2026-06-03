using System.Text.Json.Serialization;

namespace Parser.Models
{
    public class Group
    {
        [JsonPropertyName("guid")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}