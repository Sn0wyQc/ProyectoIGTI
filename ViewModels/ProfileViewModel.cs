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
        private bool tieneFoto = false;

        [ObservableProperty]
        private ObservableCollection<Post> misPublicaciones = new();

        public ProfileViewModel(UserService userService, DatabaseService db)
        {
            _userService = userService;
            _db = db;
        }

        public async Task CargarPerfilAsync()
        {
            var u = UserService.UsuarioActual;
            if (u is null) return;

            Nombre = u.Nombre;
            Correo = u.Correo;
            Descripcion = u.Descripcion;
            Habilidades = u.Habilidades;

            if (!string.IsNullOrEmpty(u.FotoPerfil) && File.Exists(u.FotoPerfil))
            {
                FotoPerfil = ImageSource.FromFile(u.FotoPerfil);
                TieneFoto = true;
            }
            else
            {
                FotoPerfil = null;
                TieneFoto = false;
            }
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
        public async Task CambiarFotoPerfilAsync()
        {
            System.Diagnostics.Debug.WriteLine("[FOTO] Comando iniciado");

#if ANDROID || IOS
            var status = await Permissions.RequestAsync<Permissions.Photos>();
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("Permiso necesario", "Necesitamos acceso a tus fotos.", "OK");
                return;
            }
#endif

            var result = await MediaPicker.Default.PickPhotoAsync();
            if (result == null) return;

            try
            {
                IsBusy = true;

                byte[] originalBytes;
                using (var stream = await result.OpenReadAsync())
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    originalBytes = ms.ToArray();
                }

                byte[] bytes = ResizeImage(originalBytes, 300, 300);

                var folder = FileSystem.AppDataDirectory;
                var filePath = Path.Combine(folder, $"profile_{UserService.UsuarioActual!.Id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.jpg");
                await File.WriteAllBytesAsync(filePath, bytes);

                // Borrar foto anterior si existe
                if (!string.IsNullOrEmpty(UserService.UsuarioActual.FotoPerfil) &&
                    File.Exists(UserService.UsuarioActual.FotoPerfil) &&
                    UserService.UsuarioActual.FotoPerfil != filePath)
                    File.Delete(UserService.UsuarioActual.FotoPerfil);

                UserService.UsuarioActual.FotoPerfil = filePath;
                await _userService.ActualizarFotoPerfilAsync(filePath);

                // Actualizar UI en hilo principal
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    TieneFoto = false;
                    FotoPerfil = null;
                });
                await Task.Delay(150);
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    FotoPerfil = ImageSource.FromFile(filePath);
                    TieneFoto = true;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FOTO] ERROR: {ex.Message}\n{ex.StackTrace}");
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task EliminarFotoPerfilAsync()
        {
            bool confirmar = await Shell.Current.DisplayAlert("Eliminar foto", "¿Deseas eliminar tu foto de perfil?", "Sí", "No");
            if (!confirmar) return;

            if (!string.IsNullOrEmpty(UserService.UsuarioActual!.FotoPerfil) &&
                File.Exists(UserService.UsuarioActual.FotoPerfil))
                File.Delete(UserService.UsuarioActual.FotoPerfil);

            await _userService.ActualizarFotoPerfilAsync(string.Empty);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                FotoPerfil = null;
                TieneFoto = false;
            });
        }

        private static byte[] ResizeImage(byte[] imageBytes, int maxWidth, int maxHeight)
        {

            using var inputStream = new MemoryStream(imageBytes);
            using var bitmap = SkiaSharp.SKBitmap.Decode(inputStream);

            if (bitmap == null) return imageBytes;

            float scale = Math.Min((float)maxWidth / bitmap.Width, (float)maxHeight / bitmap.Height);
            int w = (int)(bitmap.Width * scale);
            int h = (int)(bitmap.Height * scale);

            using var resized = bitmap.Resize(new SkiaSharp.SKImageInfo(w, h), SkiaSharp.SKSamplingOptions.Default);
            if (resized == null) return imageBytes;

            using var image = SkiaSharp.SKImage.FromBitmap(resized);
            using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, 85);
            return data.ToArray();
           
        }
    }
}