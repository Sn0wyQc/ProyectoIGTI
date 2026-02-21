using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using SkillSwap.Models;

namespace SkillSwap.Services
{
    // Mensaje que se envía al recibir un nuevo mensaje (WeakReferenceMessenger)
    public class NuevoMensajeMessage : ValueChangedMessage<Message>
    {
        public NuevoMensajeMessage(Message value) : base(value) { }
    }

    public class ChatService
    {
        private readonly DatabaseService _db;

        public ChatService(DatabaseService db)
        {
            _db = db;
        }

        public async Task<List<Message>> ObtenerConversacionAsync(int emisorId, int receptorId)
        {
            var mensajes = await _db.GetConversacionAsync(emisorId, receptorId);
            await _db.MarcarMensajesComoLeidosAsync(receptorId, emisorId); // marcar como leídos al abrir chat
            return mensajes;
        }

        public async Task<(bool exito, string mensaje)> EnviarMensajeAsync(int emisorId, string nombreEmisor, int receptorId, string nombreReceptor, string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return (false, "El mensaje no puede estar vacío.");

            var message = new Message
            {
                EmisorId = emisorId,
                NombreEmisor = nombreEmisor,
                ReceptorId = receptorId,
                NombreReceptor = nombreReceptor,
                Texto = texto.Trim(),
                Fecha = DateTime.Now,
                Leido = false
            };

            await _db.SaveMessageAsync(message);

            // Notificar a los ViewModels suscritos usando WeakReferenceMessenger
            WeakReferenceMessenger.Default.Send(new NuevoMensajeMessage(message));

            return (true, "Mensaje enviado.");
        }

        public async Task<List<Message>> ObtenerMensajesRecibidosAsync(int userId)
        {
            return await _db.GetMensajesRecibidosAsync(userId);
        }

        public async Task<int> ObtenerNoLeidosAsync(int userId)
        {
            return await _db.GetMensajesNoLeidosAsync(userId);
        }

        // Obtener lista de contactos (usuarios con los que se ha conversado)
        public async Task<List<User>> ObtenerContactosAsync(int userId, List<User> todosUsuarios)
        {
            var mensajes = await _db.GetMensajesRecibidosAsync(userId);
            var idEmisorIds = mensajes.Select(m => m.EmisorId).Distinct().ToList();
            return todosUsuarios.Where(u => idEmisorIds.Contains(u.Id) || u.Id != userId).ToList();
        }
    }
}
