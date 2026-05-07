using SkillSwap.Models;
using SQLite;
using System;
using System.IO;

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

            _database = new SQLiteAsyncConnection(
                _dbPath,
                SQLiteOpenFlags.ReadWrite |
                SQLiteOpenFlags.Create |
                SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Post>();
            await _database.CreateTableAsync<Message>();
            await _database.CreateTableAsync<Skill>();
            await _database.CreateTableAsync<Report>();
            await _database.CreateTableAsync<Notificacion>();
            await _database.CreateTableAsync<SavedPost>();
        }

        // ───────────── USUARIOS ─────────────

        // ───────────── USUARIOS ─────────────

        public async Task<User?> GetUserByEmailAsync(string correo)
        {
            await InitAsync();
            return await _database!.Table<User>()
                                   .FirstOrDefaultAsync(u => u.Correo == correo);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            await InitAsync();
            return await _database!.Table<User>()
                                   .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await InitAsync();
            return user.Id != 0
                ? await _database!.UpdateAsync(user)
                : await _database!.InsertAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await InitAsync();
            return await _database!.Table<User>().ToListAsync();
        }

        // ───────────── POSTS ─────────────

        public async Task<List<Post>> GetAllPostsAsync()
        {
            await InitAsync();
            return await _database!.Table<Post>()
                                   .OrderByDescending(p => p.FechaPublicacion)
                                   .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByCategoriaAsync(string categoria)
        {
            await InitAsync();

            if (categoria == "Todas")
                return await GetAllPostsAsync();

            return await _database!.Table<Post>()
                                   .Where(p => p.Categoria == categoria)
                                   .OrderByDescending(p => p.FechaPublicacion)
                                   .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByUserAsync(int userId)
        {
            await InitAsync();
            return await _database!.Table<Post>()
                                   .Where(p => p.UsuarioId == userId)
                                   .OrderByDescending(p => p.FechaPublicacion)
                                   .ToListAsync();
        }

        public async Task<int> SavePostAsync(Post post)
        {
            await InitAsync();
            return post.Id != 0
                ? await _database!.UpdateAsync(post)
                : await _database!.InsertAsync(post);
        }

        public async Task<int> GuardarAnuncioAsync(Post post)
        {
            return await SavePostAsync(post);
        }
        public async Task<int> GuardarAnuncioAsync(string titulo, string descripcion)
        {
            await InitAsync();

            var post = new Post
            {
                Titulo = titulo,
                Descripcion = descripcion,
                FechaPublicacion = DateTime.Now
            };

            return await _database!.InsertAsync(post);
        }

        public async Task<int> DeletePostAsync(Post post)
        {
            await InitAsync();
            return await _database!.DeleteAsync(post);
        }
        
        // ───────────── POSTS GUARDADOS ─────────────

        public async Task GuardarAnuncioGuardadoAsync(int usuarioId, int postId)
        {
            await InitAsync();

            var existe = await _database!.Table<SavedPost>()
                .FirstOrDefaultAsync(x =>
                    x.UsuarioId == usuarioId &&
                    x.PostId == postId);

            if (existe != null)
                return;

            await _database.InsertAsync(new SavedPost
            {
                UsuarioId = usuarioId,
                PostId = postId
            });
        }

        public async Task<List<Post>> ObtenerAnunciosGuardadosAsync(int usuarioId)
        {
            await InitAsync();

            var guardados = await _database!.Table<SavedPost>()
                .Where(x => x.UsuarioId == usuarioId)
                .ToListAsync();

            var posts = new List<Post>();

            foreach (var guardado in guardados)
            {
                var post = await _database.Table<Post>()
                    .FirstOrDefaultAsync(p => p.Id == guardado.PostId);

                if (post != null)
                    posts.Add(post);
            }

            return posts;
        }

        public async Task EliminarAnuncioGuardadoAsync(int usuarioId, int postId)
        {
            await InitAsync();

            var guardado = await _database!.Table<SavedPost>()
                .FirstOrDefaultAsync(x =>
                    x.UsuarioId == usuarioId &&
                    x.PostId == postId);

            if (guardado != null)
                await _database.DeleteAsync(guardado);
        }

        // ───────────── MENSAJES ─────────────

        public async Task<List<Message>> GetConversacionAsync(int userId1, int userId2)
        {
            await InitAsync();
            return await _database!.Table<Message>()
                .Where(m =>
                    (m.EmisorId == userId1 && m.ReceptorId == userId2) ||
                    (m.EmisorId == userId2 && m.ReceptorId == userId1))
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
                .Where(m =>
                    m.EmisorId == emisorId &&
                    m.ReceptorId == receptorId &&
                    !m.Leido)
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

        // ───────────── REPORTES ─────────────

        public async Task<int> SaveReportAsync(Report report)
        {
            await InitAsync();
            return await _database!.InsertAsync(report);
        }

        // ───────────── NOTIFICACIONES ─────────────

        public async Task<List<Notificacion>> GetNotificacionesAsync()
        {
            await InitAsync();

            return await _database!.Table<Notificacion>()
                                   .OrderByDescending(n => n.Id)
                                   .ToListAsync();
        }

        public async Task<int> SaveNotificacionAsync(Notificacion notificacion)
        {
            await InitAsync();

            if (notificacion.Id != 0)
                return await _database!.UpdateAsync(notificacion);

            return await _database!.InsertAsync(notificacion);
        }

        public async Task<int> DeleteNotificacionAsync(Notificacion notificacion)
        {
            await InitAsync();
            return await _database!.DeleteAsync(notificacion);
        }

        public async Task MarcarComoLeidaAsync(Notificacion notificacion)
        {
            await InitAsync();

            if (notificacion == null) return;

            notificacion.NoLeida = false;
            await _database!.UpdateAsync(notificacion);
        }

        public async Task CrearNotificacionAsync(string titulo, string mensaje)
        {
            await InitAsync();

            var noti = new Notificacion
            {
                Titulo = titulo,
                Mensaje = mensaje,
                Fecha = DateTime.Now.ToString("g"),
                NoLeida = true,
                Icono = "dotnet_bot.png"
            };

            await _database!.InsertAsync(noti);
        }
    }
}