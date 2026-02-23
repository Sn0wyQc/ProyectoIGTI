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
        private readonly HashSet<int> _postsOcultosIds = new();

        [ObservableProperty]
        private ObservableCollection<Post> posts = new();

        [ObservableProperty]
        private List<string> categorias = new();

        [ObservableProperty]
        private string categoriaSeleccionada = "Todas";

        [ObservableProperty] private string nuevoTitulo = string.Empty;
        [ObservableProperty] private string nuevaDescripcion = string.Empty;
        [ObservableProperty] private string nuevaCategoria = "Tecnología";
        [ObservableProperty] private Post? postEditando = null;

        public FeedViewModel(DatabaseService db)
        {
            _db = db;
            Categorias = SkillSwap.Models.Categorias.Lista;
        }

        public async Task CargarPostsAsync()
        {
            List<Post> lista;
            if (CategoriaSeleccionada == "Todas")
            {
                lista = await _db.GetPostsAsync();
            }
            else
            {
                lista = await _db.GetPostsByCategoriaAsync(CategoriaSeleccionada);
            }
            var filtrados = lista.Where(p => !_postsOcultosIds.Contains(p.Id)).ToList();
            Posts = new ObservableCollection<Post>(filtrados);
        }

        [RelayCommand]
        private async Task MostrarFormulario()
        {
            PostEditando = null;
            NuevoTitulo = string.Empty;
            NuevaDescripcion = string.Empty;
            NuevaCategoria = "Tecnología";
            await Shell.Current.GoToAsync("PublicarPage");
        }

        [RelayCommand]
        private async Task GuardarPostAsync()
        {
            if (string.IsNullOrWhiteSpace(NuevoTitulo))
            {
                await Shell.Current.DisplayAlert("Error", "El título es obligatorio", "OK");
                return;
            }

            var usuario = UserService.UsuarioActual;
            if (usuario == null) return;

            var post = new Post
            {
                Titulo = NuevoTitulo.Trim(),
                Descripcion = NuevaDescripcion,
                Categoria = NuevaCategoria,
                UsuarioId = usuario.Id,
                NombreUsuario = usuario.Nombre,
                FechaPublicacion = DateTime.Now
            };

            // Guardar en la base de datos
            await _db.SavePostAsync(post);

            // Limpiar el formulario para la próxima vez
            NuevoTitulo = string.Empty;
            NuevaDescripcion = string.Empty;

            // Regresar al Feed
            await Shell.Current.GoToAsync("..");

            // FORZAR RECARGA: Esto es lo que hace que aparezca de inmediato
            await CargarPostsAsync();
        }

        [RelayCommand]
        private async Task CancelarFormulario() => await Shell.Current.GoToAsync("..");

        [RelayCommand]

        private void OcultarAnuncio(Post post)
        {
            if (post == null) return;
            _postsOcultosIds.Add(post.Id);
            Posts.Remove(post);
        }

    }
}