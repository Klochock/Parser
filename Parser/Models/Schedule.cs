using System.Text.Json.Serialization;

namespace Parser.Models
{
    public class Schedule
    {
        [JsonPropertyName("FilialGUID")]
        public string FilialGUID { get; set; }

        [JsonPropertyName("Data")]
        public string Data { get; set; }

        [JsonPropertyName("DayOFWeek")]
        public string DayOfWeek { get; set; }

        [JsonPropertyName("Time_start")]
        public string TimeStart { get; set; }

        [JsonPropertyName("Time_end")]
        public string TimeEnd { get; set; }

        [JsonPropertyName("Group")]
        public string Group { get; set; }

        [JsonPropertyName("Type")]
        public string Type { get; set; }

        [JsonPropertyName("Discipline")]
        public string Discipline { get; set; }

        [JsonPropertyName("Employee")]
        public string Employee { get; set; }

        [JsonPropertyName("Classroom")]
        public string Classroom { get; set; }


        public string TimeLesson => $"{TimeStart} - {TimeEnd}";
    }
}