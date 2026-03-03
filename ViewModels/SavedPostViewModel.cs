using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using SkillSwap.Models;
using SkillSwap.Services;

namespace SkillSwap.ViewModels
{
    public partial class SavedPostViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        [ObservableProperty]
        private ObservableCollection<Post> postsGuardados = new();

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public SavedPostViewModel(DatabaseService database)
        {
            _database = database;
        }

        [RelayCommand]
        public async Task CargarPostsGuardadosAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var usuario = UserService.UsuarioActual;
                if (usuario is null) return;

                var posts = await _database.ObtenerAnunciosGuardadosAsync(usuario.Id);

                PostsGuardados.Clear();
                foreach (var post in posts)
                    PostsGuardados.Add(post);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}