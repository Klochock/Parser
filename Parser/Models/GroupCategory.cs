using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Parser.Models
{
    public class GroupCategory
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("groups")]
        public List<Group> Groups { get; set; }
    }
}