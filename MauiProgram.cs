using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkillSwap.Services;
using SkillSwap.ViewModels;
using SkillSwap.Views;

namespace SkillSwap
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // ─── Registro de Servicios (Singleton) ───
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<ChatService>();

            // ─── Registro de ViewModels (Transient) ───
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<FeedViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ChatViewModel>();

            // ─── Registro de Pages (Transient) ───
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<FeedPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ChatPage>();

            // ─── Shell ───
            builder.Services.AddSingleton<AppShell>();

            return builder.Build();
        }
    }
}
