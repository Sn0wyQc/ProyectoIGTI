using SkillSwap.ViewModels;

namespace SkillSwap.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly ProfileViewModel _vm;

        public ProfilePage(ProfileViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.CargarPerfilAsync();
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
            await _vm.CargarPublicacionesAsync();
        }
        private void OnMenuClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
            Shell.Current.FlyoutIsPresented = true;
        }
        private async void OnCambiarFotoClicked(object sender, EventArgs e)
        {
            await _vm.CambiarFotoPerfilAsync();
        }

    }
}
