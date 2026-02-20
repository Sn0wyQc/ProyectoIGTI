using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using SkillSwap.Models;
using SkillSwap.Services;

namespace SkillSwap.ViewModels
{
    public partial class FeedViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private ObservableCollection<Post> posts = new();

        [ObservableProperty]
        private List<string> categorias;

        [ObservableProperty]
        private string categoriaSeleccionada;

        [ObservableProperty]
        private bool isBusy = false;

        // Campos para nuevo/editar anuncio
        [ObservableProperty]
        private string nuevoTitulo = string.Empty;

        [ObservableProperty]
        private string nuevaDescripcion = string.Empty;

        [ObservableProperty]
        private string nuevaCategoria = "Tecnología";

        [ObservableProperty]
        private TipoAnuncio nuevoTipo = TipoAnuncio.Ofrezco;

        [ObservableProperty]
        private bool mostrandoFormulario = false;

        [ObservableProperty]
        private Post? postEditando = null;

        public FeedViewModel(DatabaseService db)
        {
            _db = db;

            Categorias = SkillSwap.Models.Categorias.Lista;
            CategoriaSeleccionada = "Todas";
        
        }

        public async Task CargarPostsAsync()
        {
            IsBusy = true;
            try
            {
                var lista = await _db.GetPostsByCategoriaAsync(CategoriaSeleccionada);
                Posts = new ObservableCollection<Post>(lista);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task FiltrarPorCategoriaAsync()
        {
            await CargarPostsAsync();
        }

        [RelayCommand]
        private void MostrarFormulario()
        {
            PostEditando = null;
            NuevoTitulo = string.Empty;
            NuevaDescripcion = string.Empty;
            NuevaCategoria = "Tecnología";
            NuevoTipo = TipoAnuncio.Ofrezco;
            MostrandoFormulario = true;
        }

        [RelayCommand]
        private void EditarPost(Post post)
        {
            PostEditando = post;
            NuevoTitulo = post.Titulo;
            NuevaDescripcion = post.Descripcion;
            NuevaCategoria = post.Categoria;
            NuevoTipo = post.Tipo;
            MostrandoFormulario = true;
        }

        [RelayCommand]
        private async Task GuardarPostAsync()
        {
            if (string.IsNullOrWhiteSpace(NuevoTitulo))
            {
                await Shell.Current.DisplayAlert("Error", "El título es obligatorio.", "OK");
                return;
            }

            var usuario = UserService.UsuarioActual;
            if (usuario is null) return;

            if (PostEditando is not null)
            {
                PostEditando.Titulo = NuevoTitulo.Trim();
                PostEditando.Descripcion = NuevaDescripcion;
                PostEditando.Categoria = NuevaCategoria;
                PostEditando.Tipo = NuevoTipo;
                await _db.SavePostAsync(PostEditando);
            }
            else
            {
                var post = new Post
                {
                    Titulo = NuevoTitulo.Trim(),
                    Descripcion = NuevaDescripcion,
                    Categoria = NuevaCategoria,
                    Tipo = NuevoTipo,
                    UsuarioId = usuario.Id,
                    NombreUsuario = usuario.Nombre,
                    FechaPublicacion = DateTime.Now
                };
                await _db.SavePostAsync(post);
            }

            MostrandoFormulario = false;
            await CargarPostsAsync();
        }

        [RelayCommand]
        private async Task EliminarPostAsync(Post post)
        {
            bool confirmar = await Shell.Current.DisplayAlert("Eliminar", $"¿Eliminar '{post.Titulo}'?", "Sí", "No");
            if (!confirmar) return;

            await _db.DeletePostAsync(post);
            await CargarPostsAsync();
        }

        [RelayCommand]
        private void CancelarFormulario()
        {
            MostrandoFormulario = false;
        }

        public bool EsDelUsuarioActual(Post post)
        {
            return UserService.UsuarioActual?.Id == post.UsuarioId;
        }
    }
}
