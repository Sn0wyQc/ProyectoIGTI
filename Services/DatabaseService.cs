using SkillSwap.Models;
using SQLite;

namespace SkillSwap.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "SkillSwap.db");
        }

        private async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Post>();
            await _database.CreateTableAsync<Message>();
            await _database.CreateTableAsync<Skill>();
            await _database.CreateTableAsync<Report>(); // Aseguramos que los reportes también se creen
            await _database.CreateTableAsync<Conversation>();

            // ✨ LA NUEVA TABLA PARA GUARDAR ANUNCIOS ✨
            await _database.CreateTableAsync<SavedPost>();
        }

        // ─────────────────────────── USUARIOS ───────────────────────────

        public async Task<User?> GetUserByEmailAsync(string correo)
        {
            await InitAsync();
            return await _database!.Table<User>().FirstOrDefaultAsync(u => u.Correo == correo);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            await InitAsync();
            return await _database!.Table<User>().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await InitAsync();
            if (user.Id != 0)
                return await _database!.UpdateAsync(user);
            return await _database!.InsertAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await InitAsync();
            return await _database!.Table<User>().ToListAsync();
        }

        // ─────────────────────────── POSTS ───────────────────────────

        public async Task<List<Post>> GetAllPostsAsync()
        {
            await InitAsync();
            return await _database!.Table<Post>().OrderByDescending(p => p.FechaPublicacion).ToListAsync();
        }

        public async Task<List<Post>> GetPostsByCategoriaAsync(string categoria)
        {
            await InitAsync();
            if (categoria == "Todas")
                return await GetAllPostsAsync();
            return await _database!.Table<Post>().Where(p => p.Categoria == categoria).OrderByDescending(p => p.FechaPublicacion).ToListAsync();
        }

        public async Task<List<Post>> GetPostsByUserAsync(int userId)
        {
            await InitAsync();
            return await _database!.Table<Post>().Where(p => p.UsuarioId == userId).OrderByDescending(p => p.FechaPublicacion).ToListAsync();
        }

        public async Task<int> SavePostAsync(Post post)
        {
            await InitAsync();
            if (post.Id != 0)
                return await _database!.UpdateAsync(post);
            return await _database!.InsertAsync(post);
        }

        public async Task<int> DeletePostAsync(Post post)
        {
            await InitAsync();
            return await _database!.DeleteAsync(post);
        }

        // ─────────────────────────── ANUNCIOS GUARDADOS (¡NUEVO!) ───────────────────────────

        public async Task GuardarAnuncioAsync(int userId, int postId)
        {
            await InitAsync();

            // Verificamos si ya lo había guardado para no duplicar datos
            var existe = await _database!.Table<SavedPost>()
                                         .Where(x => x.UserId == userId && x.PostId == postId)
                                         .FirstOrDefaultAsync();
            if (existe == null)
            {
                await _database.InsertAsync(new SavedPost { UserId = userId, PostId = postId });
            }
        }

        public async Task<List<Post>> ObtenerAnunciosGuardadosAsync(int userId)
        {
            await InitAsync();

            // 1. Buscamos los registros de guardado de este usuario
            var guardados = await _database!.Table<SavedPost>()
                                            .Where(x => x.UserId == userId)
                                            .ToListAsync();

            // Si no tiene nada guardado, devolvemos lista vacía de inmediato
            if (!guardados.Any())
                return new List<Post>();

            // 2. Extraemos solo los IDs de los posts
            var idsGuardados = guardados.Select(g => g.PostId).ToList();

            // 3. Traemos todos los posts y filtramos los que guardó el usuario, ordenados por los más recientes
            var todosLosPosts = await _database.Table<Post>().ToListAsync();
            return todosLosPosts.Where(p => idsGuardados.Contains(p.Id))
                                .OrderByDescending(p => p.FechaPublicacion)
                                .ToList();
        }

        // ─────────────────────────── MENSAJES ───────────────────────────

        public async Task<List<Message>> GetConversacionAsync(int userId1, int userId2)
        {
            await InitAsync();
            return await _database!.Table<Message>()
                .Where(m => (m.EmisorId == userId1 && m.ReceptorId == userId2)
                         || (m.EmisorId == userId2 && m.ReceptorId == userId1))
                .OrderBy(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<List<Message>> GetMensajesRecibidosAsync(int userId)
        {
            await InitAsync();
            return await _database!.Table<Message>()
                .Where(m => m.ReceptorId == userId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<int> SaveMessageAsync(Message message)
        {
            await InitAsync();
            return await _database!.InsertAsync(message);
        }

        public async Task MarcarMensajesComoLeidosAsync(int emisorId, int receptorId)
        {
            await InitAsync();
            var mensajes = await _database!.Table<Message>()
                .Where(m => m.EmisorId == emisorId && m.ReceptorId == receptorId && !m.Leido)
                .ToListAsync();

            foreach (var m in mensajes)
            {
                m.Leido = true;
                await _database.UpdateAsync(m);
            }
        }

        public async Task<int> GetMensajesNoLeidosAsync(int userId)
        {
            await InitAsync();
            return await _database!.Table<Message>()
                .Where(m => m.ReceptorId == userId && !m.Leido)
                .CountAsync();
        }

        //─────────────────────────── REPORTES ───────────────────────────

        public async Task<int> SaveReportAsync(Report report)
        {
            await InitAsync();
            return await _database!.InsertAsync(report);
        }

        // ─────────────────────────── CONVERSATION ───────────────────────────

        public async Task<Conversation?> GetConversationAsync(int user1Id, int user2Id)
        {
            await InitAsync();

            return await _database!.Table<Conversation>()
                .Where(c =>
                    (c.User1Id == user1Id && c.User2Id == user2Id) ||
                    (c.User1Id == user2Id && c.User2Id == user1Id))
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveConversationAsync(Conversation conversation)
        {
            await InitAsync();
            return await _database!.InsertAsync(conversation);
        }
    }
}