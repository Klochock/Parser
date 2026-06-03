using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Parser.Models
{
    public class ScheduleResponse
    {
        [JsonPropertyName("schedule")]
        public Dictionary<string, List<Schedule>> Schedule { get; set; }
    }
}