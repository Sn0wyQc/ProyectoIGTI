using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillSwap.Models;
using SkillSwap.Services;
using SkillSwap.Views;
using System.Collections.ObjectModel;

namespace SkillSwap.ViewModels
{
    public partial class FeedViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private ObservableCollection<Post> posts = new();

        [ObservableProperty]
        private List<string> categorias = new();

        [ObservableProperty]
        private string categoriaSeleccionada = "Todas";

        [ObservableProperty]
        private bool isBusy = false;

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

            bool esEdicion = PostEditando is not null;

            if (esEdicion)
            {
                PostEditando!.Titulo = NuevoTitulo.Trim();
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

            // Popup resultado
            var popup = new ResultadoPopup(esEdicion ? "Anuncio actualizado." : "Anuncio publicado.");
            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        [RelayCommand]
        private async Task EliminarPostAsync(Post post)
        {
            var popup = new ConfirmacionPopup("Eliminar anuncio", $"¿Deseas eliminar '{post.Titulo}'?");
            bool? confirmar = await Shell.Current.CurrentPage.ShowPopupAsync(popup) as bool?;
            if (confirmar != true) return;

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
