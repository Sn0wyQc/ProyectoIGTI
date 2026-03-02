using CommunityToolkit.Maui.Views;
using System.Windows.Input;

namespace SkillSwap.Views;

public partial class ResultadoPopup : Popup
{
    public string Mensaje { get; set; }
    public ICommand CerrarCommand { get; }

    public ResultadoPopup(string mensajePersonalizado)
    {
        InitializeComponent();
        Mensaje = mensajePersonalizado;

        CerrarCommand = new Command(() => Close());

        
        BindingContext = this;
    }
}