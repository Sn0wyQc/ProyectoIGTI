using System.Windows.Input;

namespace SkillSwap;

public partial class AppShell : Shell
{
    public ICommand ToggleThemeCommand { get; }

    public AppShell()
    {
        InitializeComponent();

        ToggleThemeCommand = new Command(() =>
        {
            InitializeComponent();

            // Sincronizar el switch con la preferencia actual (UserAppTheme),
            // usando RequestedTheme como fallback si no hay preferencia.
            var app = Application.Current;
            bool isDark = false;
            if (app != null)
            {
                isDark = app.UserAppTheme == AppTheme.Dark
                         || (app.UserAppTheme == AppTheme.Unspecified && app.RequestedTheme == AppTheme.Dark);
            }
            DarkModeSwitch.IsToggled = isDark;
        }

        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            if (Application.Current is null) return;
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        }
    }
}