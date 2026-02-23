namespace SkillSwap
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Registramos la ruta para poder usar GoToAsync
            Routing.RegisterRoute("PublicarPage", typeof(SkillSwap.Views.PublicarPage));
        }
    }
}
