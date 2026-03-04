using SkillSwap.ViewModels;

namespace SkillSwap.Views
{
    public partial class SavedPostPage : ContentPage
    {
        public SavedPostPage(SavedPostViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // Este método se ejecuta CADA VEZ que el usuario navega a esta página
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is SavedPostViewModel vm)
                await vm.CargarPostsGuardadosAsync();
        }
    }
}