using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SkillSwap.Models;

public class Notificacion : INotifyPropertyChanged
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    private string titulo = string.Empty;
    public string Titulo
    {
        get => titulo;
        set { titulo = value; OnPropertyChanged(); }
    }

    private string mensaje = string.Empty;
    public string Mensaje
    {
        get => mensaje;
        set { mensaje = value; OnPropertyChanged(); }
    }

    public string Fecha { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty;

    private bool noLeida;
    public bool NoLeida
    {
        get => noLeida;
        set { noLeida = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}