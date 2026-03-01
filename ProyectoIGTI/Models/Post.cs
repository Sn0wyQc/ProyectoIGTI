using SQLite;

namespace SkillSwap.Models
{
    public enum TipoAnuncio
    {
        Ofrezco,
        Busco
    }

    [Table("Posts")]
    public class Post
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public TipoAnuncio Tipo { get; set; } = TipoAnuncio.Ofrezco;

        [NotNull]
        public int UsuarioId { get; set; }

        public string NombreUsuario { get; set; } = string.Empty;

        public DateTime FechaPublicacion { get; set; } = DateTime.Now;
    }
}
