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
                // Obtén todos los posts para poder filtrar en memoria
                var lista = await _db.GetAllPostsAsync();
                _allPosts = lista.ToList();
                ApplyFilter();
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Filtra usando la categoría seleccionada y la búsqueda
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

        // Ejecutado cuando el usuario escribe en la SearchBar (se genera automáticamente)
        partial void OnSearchQueryChanged(string value)
        {
            ApplyFilter();
        }

        // Comando invocado por la SearchBar al pulsar buscar (Enter)
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
                // Popup de Error (antes era DisplayAlert)
                var popupError = new ResultadoPopup("El título es obligatorio.");
                Shell.Current.CurrentPage.ShowPopup(popupError);
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
                mensajeExito = "Anuncio actualizado exitosamente";
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
                mensajeExito = "Anuncio creado exitosamente";
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

            // Avisar que se eliminó correctamente
            var popupEliminado = new ResultadoPopup("Anuncio eliminado");
            Shell.Current.CurrentPage.ShowPopup(popupEliminado);
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

        [RelayCommand]
        private async Task MostrarOpcionesAsync(Post post)
        {
            if (post == null) return;

            //  Determina si es suyo o no
            bool esSuyo = EsDelUsuarioActual(post);

            var viewModelMenu = new OpcionesAnuncioViewModel(esSuyo);
            var popupMenu = new OpcionesAnuncioPopup(viewModelMenu);
            var accion = await Shell.Current.CurrentPage.ShowPopupAsync(popupMenu) as string;

            // Evalua qué boton presionó
            if (accion == "Editar")
            {
                EditarPost(post);
            }
            else if (accion == "Eliminar")
            {
                await EliminarPostAsync(post);
            }
            else if (accion == "Guardar")
            {
                var usuario = UserService.UsuarioActual;
                if (usuario is null) return;

                await _db.GuardarAnuncioAsync(usuario.Id, post.Id);

                var popupExito = new ResultadoPopup("Anuncio guardado correctamente.");
                Shell.Current.CurrentPage.ShowPopup(popupExito);
            }
            else if (accion == "Reportar")
            {
                var popupReporte = new ReportePopup();
                var resultado = await Shell.Current.CurrentPage.ShowPopupAsync(popupReporte) as DatosReporte;

                if (resultado != null)
                {
                    var popupExito = new ResultadoPopup("Reporte enviado. Revisaremos el anuncio.");
                    Shell.Current.CurrentPage.ShowPopup(popupExito);
                }
            }
        }
    }
}