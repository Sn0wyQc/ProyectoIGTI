using CommunityToolkit.Maui.Views;
using System.Windows.Input;

namespace SkillSwap.Views;

public partial class ConfirmacionPopup : Popup
{
    public string Titulo { get; set; }
    public string Mensaje { get; set; }
    public ICommand SiCommand { get; }
    public ICommand NoCommand { get; }

    public ConfirmacionPopup(string titulo, string mensaje)
    {
        InitializeComponent();

        Titulo = titulo;
        Mensaje = mensaje;

        SiCommand = new Command(() => Close(true));
        NoCommand = new Command(() => Close(false));

        // ¡Magia!
        BindingContext = this;
    }
}