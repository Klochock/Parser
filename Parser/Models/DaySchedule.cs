using System.Collections.Generic;

namespace Parser.Models
{
    public class DaySchedule
    {
        public string Date { get; set; }          // "20.04.2026 - Понедельник"
        public string DayOfWeek { get; set; }     // "Понедельник"
        public List<Schedule> Schedules { get; set; }
    }
}