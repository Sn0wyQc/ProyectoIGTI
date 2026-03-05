using SkillSwap.ViewModels;

namespace SkillSwap.Views
{
    public partial class FeedPage : ContentPage
    {
        private readonly FeedViewModel _vm;

        public FeedPage(FeedViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.CargarPostsAsync();
        }

        // Handler para el filtro de categorías (evita binding complejo dentro de BindableLayout)
        private async void OnCategoriaClicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                _vm.CategoriaSeleccionada = btn.Text;
                await _vm.CargarPostsAsync();
            }
        }
    }
}
