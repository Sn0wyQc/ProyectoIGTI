using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillSwap.Services;
using SkillSwap.Views;

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

        [ObservableProperty]
        private bool passwordOculta = true;

        [ObservableProperty]
        private string iconoPassword = "eye_off.png";

        [ObservableProperty]
        private bool regPasswordOculta = true;

        [ObservableProperty]
        private string iconoRegPassword = "eye_off.png";

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

                if (exito)
                {
                    await Shell.Current.GoToAsync("//FeedPage");
                }
                else
                {
                    ErrorMessage = mensaje;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RegistrarAsync()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                var (exito, mensaje) = await _userService.RegistrarAsync(
                    RegNombre, RegCorreo, RegPassword, RegDescripcion, RegHabilidades);

                if (exito)
                {
                    MostrandoRegistro = false;
                    Correo = RegCorreo;
                    Password = string.Empty;

                    // Popup de éxito
                    var popup = new ResultadoPopup("¡Cuenta creada! Ya puedes iniciar sesión.");
                    Shell.Current.CurrentPage.ShowPopup(popup);
                }
                else
                {
                    ErrorMessage = mensaje;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ToggleRegistro()
        {
            MostrandoRegistro = !MostrandoRegistro;
            ErrorMessage = string.Empty;
        }

        [RelayCommand]
        private void TogglePassword()
        {
            PasswordOculta = !PasswordOculta;
            IconoPassword = PasswordOculta ? "eye_off.png" : "eye.png";
        }

        [RelayCommand]
        private void ToggleRegPassword()
        {
            RegPasswordOculta = !RegPasswordOculta;
            IconoRegPassword = RegPasswordOculta ? "eye_off.png" : "eye.png";
        }
    }
}