using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillSwap.Models;
using SkillSwap.Services;
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
        private async Task MostrarFormulario()
        {
            PostEditando = null;
            NuevoTitulo = string.Empty;
            NuevaDescripcion = string.Empty;
            NuevaCategoria = "Tecnología";
            NuevoTipo = TipoAnuncio.Ofrezco;

            // Reiniciar campos nuevos
            ModalidadSeleccionada = "";
            EsGratis = true;
            NuevoPrecio = "0";

            await Shell.Current.GoToAsync("PostEditPage");
        }

        [RelayCommand]
        private async Task EditarPost(Post post)
        {
            PostEditando = post;
            NuevoTitulo = post.Titulo;
            NuevaDescripcion = post.Descripcion;
            NuevaCategoria = post.Categoria;
            NuevoTipo = post.Tipo;

            await Shell.Current.GoToAsync("PostEditPage");
        }

        [RelayCommand]
        private async Task GuardarPost()
        {
            if (string.IsNullOrWhiteSpace(NuevoTitulo))
            {
                await Shell.Current.DisplayAlert("Atención", "Escribe un título para tu anuncio", "OK");
                return;
            }

            var usuario = UserService.UsuarioActual;
            if (usuario is null) return;

            decimal precioFinal = 0;
            decimal.TryParse(NuevoPrecio, out precioFinal);

            string mensajeExito = "";

            if (PostEditando is not null)
            {
                PostEditando.Titulo = NuevoTitulo.Trim();
                PostEditando.Descripcion = NuevaDescripcion;
                PostEditando.Categoria = NuevaCategoria;
                PostEditando.Tipo = NuevoTipo;

                // ACTIVADO: Guardamos los nuevos valores
                PostEditando.Modalidad = ModalidadSeleccionada;
                PostEditando.Precio = precioFinal;

                await _db.SavePostAsync(PostEditando);
                mensajeExito = "Anuncio actualizado correctamente";
            }
            else
            {
                // --- CREAR ANUNCIO NUEVO ---
                var nuevoPost = new Post
                {
                    Titulo = NuevoTitulo.Trim(),
                    Descripcion = NuevaDescripcion,
                    Categoria = NuevaCategoria,
                    Tipo = NuevoTipo,
                    UsuarioId = usuario.Id,
                    NombreUsuario = usuario.Nombre,
                    FechaPublicacion = DateTime.Now,

                    Modalidad = ModalidadSeleccionada,
                    Precio = precioFinal
                };
                Console.WriteLine($"GUARDANDO: {nuevoPost.Titulo} - Mod: {nuevoPost.Modalidad} - $: {nuevoPost.Precio}");  
                await _db.SavePostAsync(nuevoPost);
                mensajeExito = "¡Tu anuncio ha sido publicado!";
            }

            await Shell.Current.DisplayAlert("Éxito", mensajeExito, "OK");

            await Shell.Current.GoToAsync("..");
            await CargarPostsAsync();
        }

        // --- SECCIÓN DE MODALIDAD ---
        private string _modalidadSeleccionada = "";
        public string ModalidadSeleccionada
        {
            get => _modalidadSeleccionada;
            set
            {
                if (_modalidadSeleccionada != value)
                {
                    _modalidadSeleccionada = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ModalidadColorPresencial));
                    OnPropertyChanged(nameof(ModalidadTextoPresencial));
                    OnPropertyChanged(nameof(ModalidadColorVirtual));
                    OnPropertyChanged(nameof(ModalidadTextoVirtual));
                }
            }
        }

        public Color ModalidadColorPresencial => ModalidadSeleccionada == "Presencial" ? Color.FromArgb("#512BD4") : Color.FromArgb("#E0E0E0");
        public Color ModalidadTextoPresencial => ModalidadSeleccionada == "Presencial" ? Colors.White : Colors.Black;
        public Color ModalidadColorVirtual => ModalidadSeleccionada == "Virtual" ? Color.FromArgb("#512BD4") : Color.FromArgb("#E0E0E0");
        public Color ModalidadTextoVirtual => ModalidadSeleccionada == "Virtual" ? Colors.White : Colors.Black;

        // --- SECCIÓN DE PRECIO ---
        private string _nuevoPrecio = "0";
        public string NuevoPrecio
        {
            get => _nuevoPrecio;
            set { _nuevoPrecio = value; OnPropertyChanged(); }
        }

        private bool _esGratis = true;
        public bool EsGratis
        {
            get => _esGratis;
            set
            {
                _esGratis = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PuedeEditarPrecio));
                OnPropertyChanged(nameof(ColorBotonGratis));
            }
        }

        public bool PuedeEditarPrecio => !EsGratis;
        public Color ColorBotonGratis => EsGratis ? Color.FromArgb("#2ECC71") : Color.FromArgb("#E0E0E0");

        // --- COMANDOS ---
        [RelayCommand]
        private void SeleccionarModalidad(string modalidad)
        {
            ModalidadSeleccionada = (ModalidadSeleccionada == modalidad) ? "" : modalidad;
        }

        [RelayCommand]
        private void AlternarGratis()
        {
            EsGratis = !EsGratis;
            if (EsGratis) NuevoPrecio = "0";
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
        private async Task CancelarFormulario()
        {
            await Shell.Current.GoToAsync("..");
        }

        public bool EsDelUsuarioActual(Post post)
        {
            return UserService.UsuarioActual?.Id == post.UsuarioId;
        }
    }
}