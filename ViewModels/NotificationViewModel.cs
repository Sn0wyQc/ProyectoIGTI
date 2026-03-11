using System.Collections.ObjectModel;
using System.Windows.Input;
using SkillSwap.Models;
using SkillSwap.Services;

namespace SkillSwap.ViewModels
{
    public class NotificationViewModel
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Notificacion> Notificaciones { get; set; } = new();

        public ICommand MarcarLeidaCommand { get; }
        public ICommand EliminarCommand { get; }

        public NotificationViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            MarcarLeidaCommand = new Command<Notificacion>(async (n) => await MarcarLeida(n));
            EliminarCommand = new Command<Notificacion>(async (n) => await Eliminar(n));
        }

        public async Task CargarNotificacionesAsync()
        {
            var lista = await _databaseService.GetNotificacionesAsync();

            if (lista.Count == 0)
            {
                await _databaseService.SaveNotificacionAsync(new Notificacion
                {
                    Titulo = "Bienvenido",
                    Mensaje = "Tu app está lista",
                    Fecha = DateTime.Now.ToString("g"),
                    NoLeida = true
                });

                lista = await _databaseService.GetNotificacionesAsync();
            }

            Notificaciones.Clear();

            foreach (var item in lista)
            {
                Notificaciones.Add(item);
            }
        }

        private async Task MarcarLeida(Notificacion noti)
        {
            if (noti == null) return;

            noti.NoLeida = false;

            await _databaseService.SaveNotificacionAsync(noti);
        }

        private async Task Eliminar(Notificacion noti)
        {
            if (noti == null) return;

            await _databaseService.DeleteNotificacionAsync(noti);

            Notificaciones.Remove(noti);
        }
    }
}