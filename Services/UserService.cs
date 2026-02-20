using SkillSwap.Models;

namespace SkillSwap.Services
{
    public class UserService
    {
        private readonly DatabaseService _db;

        // Usuario actualmente autenticado (sesión)
        public static User? UsuarioActual { get; private set; }

        public UserService(DatabaseService db)
        {
            _db = db;
        }

        public async Task<(bool exito, string mensaje)> RegistrarAsync(string nombre, string correo, string password, string descripcion, string habilidades)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(nombre))
                return (false, "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(correo) || !correo.Contains('@'))
                return (false, "El correo no es válido.");
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return (false, "La contraseña debe tener al menos 6 caracteres.");

            var existente = await _db.GetUserByEmailAsync(correo);
            if (existente is not null)
                return (false, "Ya existe una cuenta con ese correo.");

            var user = new User
            {
                Nombre = nombre.Trim(),
                Correo = correo.Trim().ToLower(),
                Password = HashPassword(password),
                Descripcion = descripcion,
                Habilidades = habilidades,
                FechaRegistro = DateTime.Now
            };

            await _db.SaveUserAsync(user);
            return (true, "Registro exitoso.");
        }

        public async Task<(bool exito, string mensaje)> IniciarSesionAsync(string correo, string password)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(password))
                return (false, "Completa todos los campos.");

            var user = await _db.GetUserByEmailAsync(correo.Trim().ToLower());
            if (user is null)
                return (false, "No existe una cuenta con ese correo.");

            if (user.Password != HashPassword(password))
                return (false, "Contraseña incorrecta.");

            UsuarioActual = user;
            return (true, "Inicio de sesión exitoso.");
        }

        public async Task<(bool exito, string mensaje)> ActualizarPerfilAsync(string nombre, string descripcion, string habilidades)
        {
            if (UsuarioActual is null)
                return (false, "No hay sesión activa.");
            if (string.IsNullOrWhiteSpace(nombre))
                return (false, "El nombre es obligatorio.");

            UsuarioActual.Nombre = nombre.Trim();
            UsuarioActual.Descripcion = descripcion;
            UsuarioActual.Habilidades = habilidades;

            await _db.SaveUserAsync(UsuarioActual);
            return (true, "Perfil actualizado.");
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
        }

        public async Task<List<User>> ObtenerTodosUsuariosAsync()
        {
            return await _db.GetAllUsersAsync();
        }

        // Hash simple (en producción usar BCrypt o similar)
        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
