using System.Windows.Input;

namespace SkillSwap;

public partial class AppShell : Shell
{
    // 1. Debe ser una propiedad pública { get; }
    public ICommand ToggleThemeCommand { get; }

    public AppShell()
    {
        InitializeComponent();

        // 2. Definimos la acción
        ToggleThemeCommand = new Command(() =>
        {
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme =
                    Application.Current.UserAppTheme == AppTheme.Dark
                    ? AppTheme.Light : AppTheme.Dark;
            }
        });

        // 3. ESTA LÍNEA ES VITAL: Si no está, el botón no tiene "funcionalidad"
        BindingContext = this;
    }
}
