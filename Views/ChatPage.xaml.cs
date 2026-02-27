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
        private async void OnReportClicked(object sender, EventArgs e)
        {
            if (_vm.ContactoSeleccionado == null)
                return;

            string action = await DisplayActionSheet(
                "Razón del reporte",
                "Cancelar",
                null,
                "Acoso",
                "Uso indebido del chat",
                "Mensajes hirientes",
                "Otro"
            );

            if (action == "Cancelar")
                return;

            string? customReason = null;

            if (action == "Otro")
            {
                customReason = await DisplayPromptAsync(
                    "Describe el motivo",
                    "Escribe la razón específica:"
                );

                if (string.IsNullOrWhiteSpace(customReason))
                    return;
            }

            await _vm.CrearReporteDesdeChatAsync(action, customReason);

            await DisplayAlert("Reporte enviado", "Gracias por tu reporte.", "OK");
        }
    }
}