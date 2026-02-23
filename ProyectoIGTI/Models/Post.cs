using SQLite;

namespace SkillSwap.Models
{
    public enum TipoAnuncio
    {
        Ofrezco,
        Busco
    }

    /// <summary>
    /// Clase auxiliar para centralizar las ramas/categorías del sistema
    /// </summary>
    public static class Categorias
    {
        public static List<string> Lista = new()
        {
            "Todas",
            "Tecnología",
            "Música",
            "Matemáticas",
            "Deportes",
            "Idiomas",
            "Arte",
            "Cocina",
            "Otros"
        };
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
