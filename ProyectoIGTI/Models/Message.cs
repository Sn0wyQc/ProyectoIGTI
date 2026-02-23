using SQLite;

namespace SkillSwap.Models
{
    [Table("Messages")]
    public class Message
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Texto { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;

        [NotNull]
        public int EmisorId { get; set; }

        public string NombreEmisor { get; set; } = string.Empty;

        [NotNull]
        public int ReceptorId { get; set; }

        public string NombreReceptor { get; set; } = string.Empty;

        public bool Leido { get; set; } = false;
    }
}
