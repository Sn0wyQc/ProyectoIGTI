using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillSwap.Models;
using SkillSwap.Services;
using SkillSwap.Views;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillSwap.ViewModels
{
    public partial class FeedViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        // Lista expuesta al UI
        [ObservableProperty]
        private ObservableCollection<Post> posts = new();

        // Lista completa en memoria para filtrar
        private List<Post> _allPosts = new();

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

        // Barra de búsqueda
        [ObservableProperty]
        private string searchQuery = string.Empty;

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
                var lista = await _db.GetAllPostsAsync();
                _allPosts = lista.ToList();
                ApplyFilter();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilter()
        {
            IEnumerable<Post> filtered = _allPosts;

            if (!string.IsNullOrWhiteSpace(CategoriaSeleccionada) && CategoriaSeleccionada != "Todas")
                filtered = filtered.Where(p => p.Categoria == CategoriaSeleccionada);

            var q = SearchQuery?.Trim();
            if (!string.IsNullOrEmpty(q))
            {
                q = q.ToLowerInvariant();
                filtered = filtered.Where(p =>
                    (!string.IsNullOrEmpty(p.Titulo) && p.Titulo.ToLowerInvariant().Contains(q)) ||
                    (!string.IsNullOrEmpty(p.Descripcion) && p.Descripcion.ToLowerInvariant().Contains(q)) ||
                    (!string.IsNullOrEmpty(p.NombreUsuario) && p.NombreUsuario.ToLowerInvariant().Contains(q))
                );
            }

            Posts = new ObservableCollection<Post>(filtered);
        }

        partial void OnSearchQueryChanged(string value)
        {
            ApplyFilter();
        }

        [RelayCommand]
        private void Search()
        {
            ApplyFilter();
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

        // ——— Popup de opciones al tocar un anuncio ———
        [RelayCommand]
        private async Task MostrarOpcionesAsync(Post post)
        {
            bool esPropio = EsDelUsuarioActual(post);
            var vm = new OpcionesAnuncioViewModel(esPropio);
            var popup = new OpcionesAnuncioPopup(vm);

            var resultado = await Shell.Current.CurrentPage.ShowPopupAsync(popup) as string;

            switch (resultado)
            {
                case "Editar":
                    EditarPost(post);
                    break;
                case "Eliminar":
                    await EliminarPostAsync(post);
                    break;
                case "Guardar":
                    var usuario = UserService.UsuarioActual;
                    if (usuario is not null)
                    {
                        await _db.GuardarAnuncioGuardadoAsync(usuario.Id, post.Id);
                        var confirmacion = new ResultadoPopup("Anuncio guardado.");
                        Shell.Current.CurrentPage.ShowPopup(confirmacion);
                    }
                    break;
            }
        }

        public bool EsDelUsuarioActual(Post post)
        {
            return UserService.UsuarioActual?.Id == post.UsuarioId;
        }
    }
}