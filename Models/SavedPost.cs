using SQLite;

namespace SkillSwap.Models
{
    public class SavedPost
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; } // El ID de quien guarda el anuncio
        public int PostId { get; set; } // El ID del anuncio guardado
    }
}