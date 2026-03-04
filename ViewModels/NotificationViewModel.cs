using System.Collections.ObjectModel;
using System.Windows.Input;
using SkillSwap.Models;
using SkillSwap.Services;

namespace SkillSwap.ViewModels
{
    public class NotificacionesViewModel
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Notificacion> Notificaciones { get; set; }
            = new ObservableCollection<Notificacion>();

        public ICommand MarcarLeidaCommand { get; }
        public ICommand EliminarCommand { get; }

        public NotificacionesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            MarcarLeidaCommand = new Command<Notificacion>(MarcarLeida);
            EliminarCommand = new Command<Notificacion>(Eliminar);
        }

        public async Task CargarNotificacionesAsync()
        {
            var lista = await _databaseService.GetNotificacionesAsync();

            if (lista.Count == 0)
            {
                await _databaseService.SaveNotificacionAsync(new Notificacion
                {
                    Titulo = "Bienvenido",
                    Mensaje = "Tu app está lista 🚀",
                    Fecha = DateTime.Now.ToString("g"),
                    Icono = "bell.png",
                    NoLeida = true
                });

                lista = await _databaseService.GetNotificacionesAsync();
            }

            Notificaciones.Clear();

            foreach (var item in lista)
                Notificaciones.Add(item);
        }

        private async void MarcarLeida(Notificacion noti)
        {
            if (noti == null) return;

            noti.NoLeida = false;
            await _databaseService.SaveNotificacionAsync(noti);
        }

        private async void Eliminar(Notificacion noti)
        {
            if (noti == null) return;

            await _databaseService.DeleteNotificacionAsync(noti);
            Notificaciones.Remove(noti);
        }
    }
}