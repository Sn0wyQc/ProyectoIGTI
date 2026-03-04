using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SkillSwap.Models;
using SkillSwap.Services;
using System.Collections.ObjectModel;
using SkillSwap.Views; 
using CommunityToolkit.Maui.Views; 

namespace SkillSwap.ViewModels
{
    public partial class ChatViewModel : ObservableObject, IRecipient<NuevoMensajeMessage>
    {
        private readonly ChatService _chatService;
        private readonly UserService _userService;

        [ObservableProperty]
        private ObservableCollection<User> contactos = new();

        [ObservableProperty]
        private ObservableCollection<Message> mensajes = new();

        [ObservableProperty]
        private User? contactoSeleccionado = null;

        [ObservableProperty]
        private string nuevoMensaje = string.Empty;

        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        private int mensajesNoLeidos = 0;

        public ChatViewModel(ChatService chatService, UserService userService)
        {
            _chatService = chatService;
            _userService = userService;

            WeakReferenceMessenger.Default.Register<NuevoMensajeMessage>(this);
        }

        public void Receive(NuevoMensajeMessage message)
        {
            var msg = message.Value;
            var userId = UserService.UsuarioActual?.Id;

            if (ContactoSeleccionado is not null &&
               ((msg.EmisorId == userId && msg.ReceptorId == ContactoSeleccionado.Id) ||
                (msg.EmisorId == ContactoSeleccionado.Id && msg.ReceptorId == userId)))
            {
                MainThread.BeginInvokeOnMainThread(() => Mensajes.Add(msg));
            }

            _ = ActualizarNoLeidosAsync();
        }

        public async Task CargarContactosAsync()
        {
            IsBusy = true;
            try
            {
                var todos = await _userService.ObtenerTodosUsuariosAsync();
                var sinYo = todos.Where(u => u.Id != UserService.UsuarioActual?.Id).ToList();
                Contactos = new ObservableCollection<User>(sinYo);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SeleccionarContactoAsync(User contacto)
        {
            ContactoSeleccionado = contacto;
            await CargarMensajesAsync();
        }

        private async Task CargarMensajesAsync()
        {
            if (ContactoSeleccionado is null || UserService.UsuarioActual is null) return;

            IsBusy = true;
            try
            {
                var lista = await _chatService.ObtenerConversacionAsync(
                    UserService.UsuarioActual.Id,
                    ContactoSeleccionado.Id);
                Mensajes = new ObservableCollection<Message>(lista);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task EnviarMensajeAsync()
        {
            if (string.IsNullOrWhiteSpace(NuevoMensaje) || ContactoSeleccionado is null) return;

            var usuario = UserService.UsuarioActual;
            if (usuario is null) return;

            var texto = NuevoMensaje;
            NuevoMensaje = string.Empty;

            await _chatService.EnviarMensajeAsync(
                usuario.Id, usuario.Nombre,
                ContactoSeleccionado.Id, ContactoSeleccionado.Nombre,
                texto);
        }

        private async Task ActualizarNoLeidosAsync()
        {
            if (UserService.UsuarioActual is null) return;
            MensajesNoLeidos = await _chatService.ObtenerNoLeidosAsync(UserService.UsuarioActual.Id);
        }

        public async Task CrearReporteDesdeChatAsync(string reason, string? customReason = null)
        {
            if (ContactoSeleccionado is null || UserService.UsuarioActual is null) return;

            await _chatService.CrearReporteAsync(
                UserService.UsuarioActual.Id,
                ContactoSeleccionado.Id,
                reason,
                customReason
            );

            var popup = new ResultadoPopup("Reporte enviado. Revisaremos el caso.");
            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        [RelayCommand]
        private async Task AbrirReporteAsync()
        {
            if (ContactoSeleccionado is null) return;

            var popup = new ReportePopup();
            var resultado = await Shell.Current.CurrentPage.ShowPopupAsync(popup) as DatosReporte;

            if (resultado != null)
            {
                await CrearReporteDesdeChatAsync(resultado.Motivo, resultado.Detalles);
            }
        }

        public void Cleanup()
        {
            WeakReferenceMessenger.Default.Unregister<NuevoMensajeMessage>(this);
        }
        [RelayCommand]
        private async Task AbrirReporteAsync()
        {
            if (ContactoSeleccionado is null) return;

            
            var popup = new ReportePopup();

            var resultado = await Shell.Current.CurrentPage.ShowPopupAsync(popup) as DatosReporte;

            
            if (resultado != null)
            {
                
                await CrearReporteDesdeChatAsync(resultado.Motivo, resultado.Detalles);
            }
        }
    }

}
