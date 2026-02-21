using SQLite;

namespace SkillSwap.Models
{
    [Table("Skills")]
    public class Skill
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Nombre { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;
    }

    // Categorías predefinidas para los filtros del Feed
    public static class Categorias
    {
        public static readonly List<string> Lista = new()
        {
            "Todas",
            "Tecnología",
            "Idiomas",
            "Música",
            "Arte",
            "Deportes",
            "Matemáticas",
            "Ciencias",
            "Diseño",
            "Otros"
        };
    }
}
