using SkillSwap.ViewModels;
using SkillSwap.Services;

namespace SkillSwap.Views;

public partial class NotificationPage : ContentPage
{
    private readonly NotificationViewModel _vm;
    private bool _isLoading = false;

    public NotificationPage()
    {
        InitializeComponent();

        var db = new DatabaseService();

        _vm = new NotificationViewModel(db);

        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();


        if (_isLoading) return;

        _isLoading = true;

        try
        {
            await _vm.CargarNotificacionesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            _isLoading = false;
        }
    }
}