using SQLite;
using SkillSwap.Models;

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
    }
}
