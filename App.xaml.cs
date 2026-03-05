namespace SkillSwap
{
    public partial class App : Application
    {
        public App(AppShell shell)
        {
            InitializeComponent();
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
