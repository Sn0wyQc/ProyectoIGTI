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

        private async void OnCategoriaClicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                // 1. Buscamos el contenedor que tiene todos los botones de categorías
                // Como el botón está dentro del BindableLayout del HorizontalStackLayout:
                var contenedor = btn.Parent as HorizontalStackLayout;

                if (contenedor != null)
                {
                    // 2. RECORREMOS todos los botones y les quitamos el borde (los apagamos)
                    foreach (var hijo in contenedor.Children)
                    {
                        if (hijo is Button botonHijo)
                        {
                            botonHijo.BorderWidth = 0;
                            botonHijo.BorderColor = Colors.Transparent;
                        }
                    }
                }

                // 3. ENCENDEMOS solo el botón que se acaba de presionar
                btn.BorderWidth = 2;
                btn.BorderColor = Color.FromArgb("#512BD4"); // Tu color Primary

                // 4. Actualizamos el ViewModel y cargamos los posts
                _vm.CategoriaSeleccionada = btn.Text;
                await _vm.CargarPostsAsync();
            }
        }
    }
}