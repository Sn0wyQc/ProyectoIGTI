using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SkillSwap.Models;

public class Notificacion : INotifyPropertyChanged
{
    [PrimaryKey, AutoIncrement] // 🔥 PARA SQLITE
    public int Id { get; set; }

    private string titulo;
    public string Titulo
    {
        get => titulo;
        set { titulo = value; OnPropertyChanged(); }
    }

    private string mensaje;
    public string Mensaje
    {
        get => mensaje;
        set { mensaje = value; OnPropertyChanged(); }
    }

    private string fecha;
    public string Fecha
    {
        get => fecha;
        set { fecha = value; OnPropertyChanged(); }
    }

    private string icono;
    public string Icono
    {
        get => icono;
        set { icono = value; OnPropertyChanged(); }
    }

    private bool noLeida;
    public bool NoLeida
    {
        get => noLeida;
        set { noLeida = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}