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
            _vm.CargarPerfil();
            await _vm.CargarPublicacionesAsync();
        }
    }
}
