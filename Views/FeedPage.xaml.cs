using System;
using Microsoft.Maui.Controls;
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

        private void OnMenuClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        // Firma requerida por XAML: SearchBar SearchButtonPressed
        private void OnSearchButtonPressed(object sender, EventArgs e)
        {
            if (BindingContext is ViewModels.FeedViewModel vm && vm.SearchCommand.CanExecute(null))
            {
                vm.SearchCommand.Execute(null);
            }

            // Opcional: quitar foco para cerrar teclado
            if (sender is SearchBar sb)
                sb.Unfocus();
        }
    }
}
