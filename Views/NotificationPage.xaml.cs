using SkillSwap.ViewModels;
using SkillSwap.Services;

namespace SkillSwap.Views;

public partial class NotificationPage : ContentPage
{
    private NotificationViewModel _viewModel;

    public NotificationPage(DatabaseService databaseService)
    {
        InitializeComponent();

        _viewModel = new NotificationViewModel(databaseService);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarNotificacionesAsync();
    }
}