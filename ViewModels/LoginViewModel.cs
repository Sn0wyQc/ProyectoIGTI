using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillSwap.Services;

namespace SkillSwap.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly UserService _userService;

        [ObservableProperty]
        private string correo = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isBusy = false;

        // --- LÓGICA DE MOSTRAR/OCULTAR ---
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PasswordIcon))]
        private bool isPasswordHidden = true;

        // Esta propiedad cambia el nombre del archivo de imagen automáticamente
        public string PasswordIcon => IsPasswordHidden ? "ojo_cerrado.png" : "ojo_abierto.png";

        [RelayCommand]
        private void TogglePassword()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }
        // ---------------------------------

        [ObservableProperty]
        private string regNombre = string.Empty;

        [ObservableProperty]
        private string regCorreo = string.Empty;

        [ObservableProperty]
        private string regPassword = string.Empty;

        [ObservableProperty]
        private string regDescripcion = string.Empty;

        [ObservableProperty]
        private string regHabilidades = string.Empty;

        [ObservableProperty]
        private bool mostrandoRegistro = false;

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
        }

        [RelayCommand]
        private async Task IniciarSesionAsync()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                var (exito, mensaje) = await _userService.IniciarSesionAsync(Correo, Password);
                if (exito) await Shell.Current.GoToAsync("//FeedPage");
                else ErrorMessage = mensaje;
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task RegistrarAsync()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                var (exito, mensaje) = await _userService.RegistrarAsync(RegNombre, RegCorreo, RegPassword, RegDescripcion, RegHabilidades);
                if (exito)
                {
                    await Shell.Current.DisplayAlert("¡Listo!", "Cuenta creada.", "OK");
                    MostrandoRegistro = false;
                    Correo = RegCorreo;
                }
                else ErrorMessage = mensaje;
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private void ToggleRegistro()
        {
            MostrandoRegistro = !MostrandoRegistro;
            ErrorMessage = string.Empty;
        }
    }
}


