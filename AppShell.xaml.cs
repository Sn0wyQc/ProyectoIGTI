namespace SkillSwap
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Sincronizar el switch con el tema actual al abrir
            DarkModeSwitch.IsToggled = Application.Current?.RequestedTheme == AppTheme.Dark;

            // Cuando el Flyout se cierra, desactivarlo para ocultar el botón automático
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(FlyoutIsPresented) && !FlyoutIsPresented)
                {
                    FlyoutBehavior = FlyoutBehavior.Disabled;
                }
            };
        }

        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            if (Application.Current is null) return;
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        }
    }
}
