using SkillSwap.ViewModels;

namespace SkillSwap.Views;

public partial class NotificationPage : ContentPage
{
    private readonly NotificacionesViewModel _viewModel;

    public NotificationPage(NotificacionesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarNotificacionesAsync();
    }
}