namespace SkillSwap.Views
{
    public partial class FeedPage : ContentPage
    {
        public FeedPage(ViewModels.FeedViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Verificamos que el BindingContext sea nuestro ViewModel
            if (BindingContext is SkillSwap.ViewModels.FeedViewModel vm)
            {
                // Forzamos la carga de anuncios desde la DB
                await vm.CargarPostsAsync();
            }
        }

        // Maneja el clic de "Todas" y las demás categorías
        private async void OnCategoriaClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (BindingContext is ViewModels.FeedViewModel vm)
            {
                vm.CategoriaSeleccionada = button.Text;
                await vm.CargarPostsAsync();
            }
        }
    }
}   