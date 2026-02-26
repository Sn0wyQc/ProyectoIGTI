namespace SkillSwap
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(FlyoutIsPresented) && !FlyoutIsPresented)
                    FlyoutBehavior = FlyoutBehavior.Disabled;
            };

        }
    }

}
