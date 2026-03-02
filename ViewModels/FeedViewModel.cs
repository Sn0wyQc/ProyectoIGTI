using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillSwap.Models;
using SkillSwap.Services;
using System.Collections.ObjectModel;
using SkillSwap.Views; // Agregado para acceder a tus Popups
using CommunityToolkit.Maui.Views; // Agregado para usar ShowPopup y ShowPopupAsync

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
                // Popup de Error (antes era DisplayAlert)
                var popupError = new ResultadoPopup("El título es obligatorio.");
                Shell.Current.CurrentPage.ShowPopup(popupError);
                return;
            }

            var usuario = UserService.UsuarioActual;
            if (usuario is null) return;

            // Variable para saber si editamos o creamos 
            string mensajeExito = "";

            if (PostEditando is not null)
            {
                PostEditando.Titulo = NuevoTitulo.Trim();
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

            // Popup de Éxito 
            var popupExito = new ResultadoPopup(mensajeExito);
            Shell.Current.CurrentPage.ShowPopup(popupExito);
        }

        [RelayCommand]
        private async Task EliminarPostAsync(Post post)
        {
            // Popup de Confirmación de 2 botones (antes era DisplayAlert)
            var popup = new ConfirmacionPopup("Eliminar anuncio", $"¿Estás seguro de que deseas eliminar '{post.Titulo}'?");
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
    }
}