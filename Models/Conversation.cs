using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SkillSwap.Models
{
    public class Conversation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int User1Id { get; set; }
        public int User2Id { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
