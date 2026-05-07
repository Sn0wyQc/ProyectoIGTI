using SkillSwap.Models;
using SkillSwap.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SkillSwap.ViewModels;

public class NotificationViewModel
{
    private readonly DatabaseService _db;

    public ObservableCollection<Notificacion> Notificaciones { get; set; } = new();

    public ICommand MarcarLeidaCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand CrearNotificacionCommand { get; }
    public ICommand OpcionesCommand { get; }
    public ICommand AbrirDetalleCommand { get; }

    public NotificationViewModel(DatabaseService db)
    {
        _db = db;

        MarcarLeidaCommand = new Command<Notificacion>(async (n) => await MarcarLeida(n));
        EliminarCommand = new Command<Notificacion>(async (n) => await Eliminar(n));
        CrearNotificacionCommand = new Command(async () => await CrearNotificacionTest());
        OpcionesCommand = new Command<Notificacion>(async (n) => await MostrarOpciones(n));
        AbrirDetalleCommand = new Command<Notificacion>(async (n) => await AbrirDetalle(n));
    }

    public async Task CargarNotificacionesAsync()
    {
        var lista = await _db.GetNotificacionesAsync();

        if (lista == null || lista.Count == 0)
        {
            await _db.CrearNotificacionAsync("Bienvenido", "Tu app está lista");
            lista = await _db.GetNotificacionesAsync();
        }

        Notificaciones.Clear();

        foreach (var item in lista)
        {
            Notificaciones.Add(item);
        }
    }

    //  MARCAR COMO LEÍDA 
    private async Task MarcarLeida(Notificacion n)
    {
        if (n == null) return;

        await _db.MarcarComoLeidaAsync(n);
    }

    //  ELIMINAR 
    private async Task Eliminar(Notificacion n)
    {
        if (n == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Eliminar",
            "¿Seguro que quieres eliminar?",
            "Sí",
            "No");

        if (!confirm) return;

        var filas = await _db.DeleteNotificacionAsync(n);

        if (filas > 0)
        {
            Notificaciones.Remove(n);
        }
    }

    public async Task CrearNotificacionTest()
    {
        await _db.CrearNotificacionAsync("Prueba", "Esto funciona");
        await CargarNotificacionesAsync();
    }

    public async Task MostrarOpciones(Notificacion n)
    {
        if (n == null) return;

        string accion = await Application.Current.MainPage.DisplayActionSheet(
            "Opciones",
            "Cancelar",
            null,
            "Marcar como leída",
            "Eliminar"
        );

        if (accion == "Marcar como leída")
        {
            await MarcarLeida(n);
        }
        else if (accion == "Eliminar")
        {
            await Eliminar(n);
        }
    }
    public async Task AbrirDetalle(Notificacion n)
    {
        if (n == null) return;

        if (n.NoLeida)
        {
            await _db.MarcarComoLeidaAsync(n);
        }

        await Application.Current.MainPage.Navigation.PushAsync(
            new SkillSwap.Views.NotificationDetailPage(n, _db)
        );
    }
}