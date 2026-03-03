using CommunityToolkit.Maui.Views;
using System.Windows.Input;

namespace SkillSwap.Views;

// 1. LA CLASE PRINCIPAL DEBE IR PRIMERO
public partial class ReportePopup : Popup
{
    public ICommand CancelarCommand { get; }
    public ICommand EnviarCommand { get; }

    public ReportePopup()
    {
        InitializeComponent();

        CancelarCommand = new Command(() => Close(null));

        EnviarCommand = new Command(() =>
        {
            var motivo = MotivoPicker.SelectedItem as string;
            var detalles = DetallesEntry.Text;

            if (string.IsNullOrWhiteSpace(motivo))
            {
                return;
            }

            Close(new DatosReporte { Motivo = motivo, Detalles = detalles ?? string.Empty });
        });

        BindingContext = this;
    }
}

// 2. LA CLASE AUXILIAR DEBE IR ABAJO
public class DatosReporte
{
    public string Motivo { get; set; } = string.Empty;
    public string Detalles { get; set; } = string.Empty;
}