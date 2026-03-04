namespace SkillSwap
{
    public partial class App : Application
    {
        public App(AppShell shell)
        {
            InitializeComponent();

            // Iniciar la aplicación en modo claro por defecto
            this.UserAppTheme = AppTheme.Light;

            MainPage = shell;
        }

        protected override async void OnStart()
        {
            base.OnStart();
            // Redirigir al login al arrancar
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}