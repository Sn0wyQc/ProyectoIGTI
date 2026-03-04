using SkillSwap.ViewModels;

namespace SkillSwap.Views
{
    public partial class ChatPage : ContentPage
    {
        private readonly ChatViewModel _vm;

        public ChatPage(ChatViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
            await _vm.CargarContactosAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _vm.Cleanup();
        }

        private void OnMenuClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }
    }
}