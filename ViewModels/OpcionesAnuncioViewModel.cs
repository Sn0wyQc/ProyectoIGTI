using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace SkillSwap.ViewModels
{
    public partial class OpcionesAnuncioViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EsAjeno))]
        private bool esPropio;

        public bool EsAjeno => !EsPropio;

        
        public Action<string>? CerrarPopupAction { get; set; }

        public OpcionesAnuncioViewModel(bool esPropio)
        {
            EsPropio = esPropio;
        }

        [RelayCommand]
        private void SeleccionarOpcion(object opcionObj)
        {
            
            string opcion = opcionObj?.ToString() ?? "Cancelar";

            
            CerrarPopupAction?.Invoke(opcion);
        }
    }
}