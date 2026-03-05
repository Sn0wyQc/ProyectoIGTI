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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is SavedPostViewModel vm)
                await vm.CargarPostsGuardadosAsync();
        }

        private void OnMenuClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }
    }
}
