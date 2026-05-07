using SQLite;

namespace SkillSwap.Models
{
    public class SavedPost
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PostId { get; set; }
    }
}
