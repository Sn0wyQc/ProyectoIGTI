using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillSwap.Models;
using SkillSwap.Services;
using System.Collections.ObjectModel;
using SkillSwap.Views; // Agregado para el Popup
using CommunityToolkit.Maui.Views; // Agregado para ShowPopup

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

                if (exito)
                {
                   
                    var popup = new ResultadoPopup("Cambios guardados correctamente");
                    Shell.Current.CurrentPage.ShowPopup(popup);
                }
                else
                {
                    
                    MensajeEstado = mensaje;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CerrarSesionAsync()
        {
            // Creamos nuestro popup de confirmación personalizado
            var popup = new ConfirmacionPopup("Cerrar sesión", "¿Estás seguro de que deseas salir de tu cuenta?");

            // Lo mostramos y esperamos a que el usuario toque Sí o No. Devuelve un object, así que lo convertimos a bool?
            bool? confirmar = await Shell.Current.CurrentPage.ShowPopupAsync(popup) as bool?;

            // Si tocó "No" o cerró el popup tocando fuera, confirmar será false o null
            if (confirmar != true) return;

            _userService.CerrarSesion();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}