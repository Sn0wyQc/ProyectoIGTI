using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using SkillSwap.Models;
using SkillSwap.Services;

namespace SkillSwap.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly UserService _userService;
        private readonly DatabaseService _db;

        [ObservableProperty]
        private string nombre = string.Empty;

        [ObservableProperty]
        private string correo = string.Empty;

        [ObservableProperty]
        private string descripcion = string.Empty;

        [ObservableProperty]
        private string habilidades = string.Empty;

        [ObservableProperty]
        private string mensajeEstado = string.Empty;

        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        private ImageSource? fotoPerfil;

        [ObservableProperty]
        private ObservableCollection<Post> misPublicaciones = new();

        public ProfileViewModel(UserService userService, DatabaseService db)
        {
            _userService = userService;
            _db = db;
        }

        public void CargarPerfil()
        {
            var u = UserService.UsuarioActual;
            if (u is null) return;

            Nombre = u.Nombre;
            Correo = u.Correo;
            Descripcion = u.Descripcion;
            Habilidades = u.Habilidades;

            if (!string.IsNullOrEmpty(u.FotoPerfil) && File.Exists(u.FotoPerfil))
                FotoPerfil = ImageSource.FromFile(u.FotoPerfil);
        }
        public async Task CargarPublicacionesAsync()
        {
            var u = UserService.UsuarioActual;
            if (u is null) return;

            var lista = await _db.GetPostsByUserAsync(u.Id);
            MisPublicaciones = new ObservableCollection<Post>(lista);
        }

        [RelayCommand]
        private async Task GuardarPerfilAsync()
        {
            IsBusy = true;
            MensajeEstado = string.Empty;

            try
            {
                var (exito, mensaje) = await _userService.ActualizarPerfilAsync(Nombre, Descripcion, Habilidades);
                MensajeEstado = mensaje;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CerrarSesionAsync()
        {
            bool confirmar = await Shell.Current.DisplayAlert("Cerrar sesión", "¿Deseas cerrar sesión?", "Sí", "No");
            if (!confirmar) return;

            _userService.CerrarSesion();
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        private async Task CambiarFotoPerfilAsync()
        {
            var result = await MediaPicker.Default.PickPhotoAsync();
            if (result == null) return;

            // Redimensionar con SkiaSharp
            using var stream = await result.OpenReadAsync();
            var bytes = ResizeImage(stream, 300, 300);

            // Guardar en carpeta local de la app
            var folder = FileSystem.AppDataDirectory;
            var filePath = Path.Combine(folder, $"profile_{UserService.UsuarioActual!.Id}.jpg");
            await File.WriteAllBytesAsync(filePath, bytes);

            // Actualizar en base de datos
            UserService.UsuarioActual.FotoPerfil = filePath;
            await _userService.ActualizarFotoPerfilAsync(filePath);

            // Actualizar UI
            FotoPerfil = ImageSource.FromFile(filePath);
        }

        [RelayCommand]
        private async Task EliminarFotoPerfilAsync()
        {
            bool confirmar = await Shell.Current.DisplayAlert("Eliminar foto", "¿Deseas eliminar tu foto de perfil?", "Sí", "No");
            if (!confirmar) return;

            // Borrar archivo si existe
            if (!string.IsNullOrEmpty(UserService.UsuarioActual!.FotoPerfil) &&
                File.Exists(UserService.UsuarioActual.FotoPerfil))
                File.Delete(UserService.UsuarioActual.FotoPerfil);

            // Limpiar en BD
            await _userService.ActualizarFotoPerfilAsync(null!);

            FotoPerfil = null;
        }

        private static byte[] ResizeImage(Stream imageStream, int maxWidth, int maxHeight)
        {
            using var bitmap = SkiaSharp.SKBitmap.Decode(imageStream);
            float scale = Math.Min((float)maxWidth / bitmap.Width, (float)maxHeight / bitmap.Height);
            int w = (int)(bitmap.Width * scale);
            int h = (int)(bitmap.Height * scale);

            using var resized = bitmap.Resize(new SkiaSharp.SKImageInfo(w, h), SkiaSharp.SKFilterQuality.High);
            using var image = SkiaSharp.SKImage.FromBitmap(resized);
            using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, 85);
            return data.ToArray();
        }
    }
}
