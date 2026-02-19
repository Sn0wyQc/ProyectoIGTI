using SQLite;

namespace SkillSwap.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Nombre { get; set; } = string.Empty;

        [NotNull, Unique]
        public string Correo { get; set; } = string.Empty;

        [NotNull]
        public string Password { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string Habilidades { get; set; } = string.Empty;

        public string FotoPerfil { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
