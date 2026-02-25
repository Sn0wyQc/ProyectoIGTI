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
    }
}
