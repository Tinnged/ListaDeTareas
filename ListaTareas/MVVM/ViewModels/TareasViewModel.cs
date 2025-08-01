using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ListaTareas.MVVM.Models;
using System.ComponentModel;

namespace ListaTareas.MVVM.ViewModels
{
    public class TareasViewModel : INotifyPropertyChanged
    {
        public readonly TareasRepository _repository; // Cambiado a public
        private TareaModel _tareaTO = new TareaModel();

        public TareaModel TareaTO
        {
            get => _tareaTO;
            set
            {
                _tareaTO = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TareaModel> lstTareas { get; set; } =
           new ObservableCollection<TareaModel>();

        public ICommand CreateCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand ToggleEstadoCommand { get; set; }

        // Propiedades para mostrar estadísticas
        public int TotalTareas => lstTareas.Count;
        public int TareasCompletadas => lstTareas.Count(t => t.Estado);

        public event PropertyChangedEventHandler PropertyChanged;

        public TareasViewModel()
        {
            _repository = new TareasRepository();
            CreateCommand = new Command(async () => await CrearTarea());
            LoadCommand = new Command(async () => await LoadTarea());
            DeleteCommand = new Command<String>(async (Key) => await DeleteTarea(Key));
            EditCommand = new Command(async () => await UpdateTarea(TareaTO));
            ToggleEstadoCommand = new Command<TareaModel>(async (tarea) => await ToggleEstado(tarea));

            // ✅ Inicializar con fecha actual
            TareaTO = new TareaModel
            {
                FechaVencimiento = DateTime.Today
            };
        }

        private async Task ToggleEstado(TareaModel tarea)
        {
            try
            {
                tarea.Estado = !tarea.Estado;
                await _repository.UpdateDocumentAsync(tarea, tarea.Key);
                await LoadTarea(); // ✅ Recargar para actualizar estadísticas y ordenamiento
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar estado: {ex.ToString()}");
                // Revertir el cambio si hay error
                tarea.Estado = !tarea.Estado;
            }
        }

        private async Task UpdateTarea(TareaModel newTarea)
        {
            try
            {
                if (Validar())
                {
                    await _repository.UpdateDocumentAsync(newTarea, newTarea.Key);
                    await ShowMessage("Tarea modificada exitosamente!", true);
                    Console.WriteLine("Tarea modificada exitosamente!");
                    Clean();
                    await LoadTarea(); // Recargar la lista
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al modificar la Tarea. Error: {ex.ToString()}");
            }
        }

        private async Task DeleteTarea(string key)
        {
            try
            {
                bool isConfirmed = await App.Current.MainPage.DisplayAlert(
                    "Alerta",
                    "¿Desea borrar la Tarea?",
                    "Si",
                    "No");

                if (!isConfirmed) return;

                await _repository.DeleteDocumentAsync(key);
                await LoadTarea();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al borrar la Tarea. Error: {ex.ToString()}");
            }
        }

        private async Task LoadTarea()
        {
            try
            {
                var TareasFromDB = await _repository.GetAllAsync();

                lstTareas.Clear();

                // ✅ Ordenar por fecha de vencimiento (más próxima primero) y luego por nombre
                var tareasOrdenadas = TareasFromDB
                    .OrderBy(t => t.Value.FechaVencimiento.Date) // Ordenar por fecha sin hora
                    .ThenBy(t => t.Value.Nombre); // Luego por nombre para consistencia

                foreach (var Tarea in tareasOrdenadas)
                {
                    var tareaModel = new TareaModel
                    {
                        Key = Tarea.Key, // Importante: asignar la Key
                        Nombre = Tarea.Value.Nombre,
                        Descripcion = Tarea.Value.Descripcion,
                        FechaVencimiento = Tarea.Value.FechaVencimiento,
                        Estado = Tarea.Value.Estado,
                    };

                    lstTareas.Add(tareaModel);

                    // 🔍 Debug: Mostrar el orden de las tareas
                    Console.WriteLine($"Tarea: {tareaModel.Nombre} - Fecha: {tareaModel.FechaVencimiento:dd/MM/yyyy}");
                }

                // Notificar cambios en las propiedades de estadísticas
                OnPropertyChanged(nameof(TotalTareas));
                OnPropertyChanged(nameof(TareasCompletadas));

                Console.WriteLine($"Total de tareas cargadas: {lstTareas.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al cargar las Tareas. Error: {ex.ToString()}");
            }
        }

        private async Task CrearTarea()
        {
            try
            {
                if (Validar())
                {
                    await _repository.CreateDocumentAsync(TareaTO);
                    await ShowMessage("Tarea agregada exitosamente!", true);
                    Console.WriteLine("Tarea agregada exitosamente!");
                    Clean();
                    await LoadTarea(); // Recargar la lista
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al crear la Tarea. Error: {ex.ToString()}");
            }
        }

        private bool Validar()
        {
            bool respuesta = true;
            if (string.IsNullOrWhiteSpace(TareaTO.Nombre) ||
               string.IsNullOrWhiteSpace(TareaTO.Descripcion) ||
               TareaTO.FechaVencimiento == default(DateTime))
            {
                ShowMessage("Por favor completar todos los campos", false);
                respuesta = false;
            }
            return respuesta;
        }

        private void Clean()
        {
            TareaTO = new TareaModel
            {
                FechaVencimiento = DateTime.Today // ✅ Asegurar que siempre inicie con fecha actual
            };
        }

        private async Task ShowMessage(string message, bool isSuccess)
        {
            var snackbar = Snackbar.Make(
                                          message,
                                          duration: TimeSpan.FromSeconds(3),
                                          visualOptions: new SnackbarOptions
                                          {
                                              BackgroundColor = isSuccess ? Colors.Green : Colors.DarkRed,
                                              TextColor = Colors.White,
                                          });
            await snackbar.Show();
        }

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Métodos públicos para usar desde la vista
        public async Task<bool> CrearTareaPublic(TareaModel tarea)
        {
            try
            {
                await _repository.CreateDocumentAsync(tarea);
                await LoadTarea();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear tarea: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarTareaPublic(TareaModel tarea)
        {
            try
            {
                await _repository.UpdateDocumentAsync(tarea, tarea.Key);
                await LoadTarea();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar tarea: {ex.Message}");
                return false;
            }
        }

        // ✅ Método público para cargar tareas
        public async Task CargarTareasPublic()
        {
            await LoadTarea();
        }
    }
}