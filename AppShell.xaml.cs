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
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme =
                    Application.Current.UserAppTheme == AppTheme.Dark
                    ? AppTheme.Light : AppTheme.Dark;
            }
        });

        BindingContext = this;
    }
}
