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
            await _vm.CargarContactosAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _vm.Cleanup();
        }
    }
}
