using SkillSwap.ViewModels;

namespace SkillSwap.Views
{
    public partial class PublicarPage : ContentPage
    {
        public PublicarPage(FeedViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}