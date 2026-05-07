using SkillSwap.Models;
using SkillSwap.Services;

namespace SkillSwap.Views;

public partial class NotificationDetailPage : ContentPage
{
    private Notificacion _notificacion;
    private DatabaseService _db;

    public NotificationDetailPage(Notificacion notificacion, DatabaseService db)
    {
        InitializeComponent();

        _notificacion = notificacion;
        _db = db;

        BindingContext = _notificacion;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        this.Content.Opacity = 0;
        this.Content.TranslationY = 20;

        await Task.WhenAll(
            this.Content.FadeTo(1, 300),
            this.Content.TranslateTo(0, 0, 300, Easing.SinOut)
        );
    }

    // MARCAR COMO LEÍDA
    private async void OnMarcarLeidaClicked(object sender, EventArgs e)
    {
        if (_notificacion == null) return;

        await BtnLeida.ScaleTo(0.9, 100);
        await BtnLeida.ScaleTo(1, 100);

        await _db.MarcarComoLeidaAsync(_notificacion);

        BtnLeida.IsVisible = false;

        await DisplayAlert("Listo", "Notificación marcada como leída", "OK");
    }

    //  ELIMINAR 
    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        if (_notificacion == null) return;

        bool confirmar = await DisplayAlert("Eliminar",
                                            "¿Seguro que quieres eliminar esta notificación?",
                                            "Sí",
                                            "Cancelar");

        if (!confirmar) return;


        await this.Content.FadeTo(0, 200);
        await this.Content.TranslateTo(0, 50, 200);

        await _db.DeleteNotificacionAsync(_notificacion);

        await Navigation.PopAsync();
    }
}