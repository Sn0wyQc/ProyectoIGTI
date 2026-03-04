using SkillSwap.ViewModels;
using SkillSwap.Views;

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

        // Filtro de categorías
        private async void OnCategoriaClicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                _vm.CategoriaSeleccionada = btn.Text;
                await _vm.CargarPostsAsync();
            }
        }

        private async void IrInicio(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FeedPage");
        }

        private async void IrMensajes(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ChatPage");
        }

        private async void IrNotificationPage(object sender, EventArgs e)
        {
            var page = App.Current.Handler.MauiContext.Services
                   .GetRequiredService<NotificationPage>();

            await Navigation.PushAsync(page);
        }

        private async void IrPerfil(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}