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

        // filtro de categorías 
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
            await Navigation.PushAsync(new NotificationPage());
        }

        private async void IrPerfil(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}
