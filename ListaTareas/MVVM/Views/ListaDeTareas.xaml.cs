using ListaTareas.MVVM.Models;
using ListaTareas.MVVM.ViewModels;
using CommunityToolkit.Maui.Views;

namespace ListaTareas.MVVM.Views;

public partial class ListaDeTareas : ContentPage
{
    private TareasViewModel _viewModel;

    public ListaDeTareas()
    {
        InitializeComponent();
        _viewModel = new TareasViewModel();
        BindingContext = _viewModel;

        // Cargar tareas autom�ticamente al inicializar - usando m�todo p�blico async
        Loaded += async (s, e) => await _viewModel.CargarTareasPublic();

        // Tambi�n cargar cuando la p�gina aparece
        Appearing += async (s, e) => await _viewModel.CargarTareasPublic();
    }

    private async void ShowAddTarea(object sender, EventArgs e)
    {
        try
        {
            var popup = new AgregarTarea();
            var result = await this.ShowPopupAsync(popup);

            // Recargar datos despu�s del popup - usando m�todo p�blico
            await _viewModel.CargarTareasPublic();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al mostrar popup: {ex.Message}", "OK");
        }
    }

    private async void ShowEditTareaPopUp(object sender, EventArgs e)
    {
        try
        {
            var tareaSeleccionada = (TareaModel)((Button)sender).BindingContext;

            var popUp = new EditTarea();
            var editViewModel = new TareasViewModel
            {
                TareaTO = new TareaModel
                {
                    Key = tareaSeleccionada.Key,
                    Nombre = tareaSeleccionada.Nombre,
                    Descripcion = tareaSeleccionada.Descripcion,
                    FechaVencimiento = tareaSeleccionada.FechaVencimiento,
                    Estado = tareaSeleccionada.Estado
                }
            };
            popUp.BindingContext = editViewModel;

            var result = await this.ShowPopupAsync(popUp);

            // Recargar datos despu�s del popup - usando m�todo p�blico
            await _viewModel.CargarTareasPublic();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al mostrar popup de edici�n: {ex.Message}", "OK");
        }
    }
}