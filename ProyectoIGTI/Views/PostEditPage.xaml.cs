using SkillSwap.ViewModels;

namespace SkillSwap.Views;

public partial class PostEditPage : ContentPage
{
    public PostEditPage(FeedViewModel vm)
    {
        InitializeComponent();
        
        BindingContext = vm;
    }
}