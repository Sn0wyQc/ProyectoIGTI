using CommunityToolkit.Maui.Views;
using SkillSwap.ViewModels;

namespace SkillSwap.Views;

// Importante: Debe heredar de Popup
public partial class OpcionesAnuncioPopup : Popup
{
    // Importante: El constructor ahora recibe el ViewModel
    public OpcionesAnuncioPopup(OpcionesAnuncioViewModel vm)
    {
        InitializeComponent();

        vm.CerrarPopupAction = (resultado) => Close(resultado);

        BindingContext = vm;
    }
}