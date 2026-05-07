using SQLite;

namespace SkillSwap.Models
{
    public class Report
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ReporterId { get; set; }
        public int ReportedUserId { get; set; }

        public string Reason { get; set; } = string.Empty;
        public string? CustomReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
