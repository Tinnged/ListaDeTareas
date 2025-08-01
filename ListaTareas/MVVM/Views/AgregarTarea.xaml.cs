using ListaTareas.MVVM.ViewModels;
using CommunityToolkit.Maui.Views;

namespace ListaTareas.MVVM.Views;

public partial class AgregarTarea : Popup
{
    public AgregarTarea()
    {
        InitializeComponent();
        BindingContext = new TareasViewModel();
    }

    private void CloseAddPopup(object sender, EventArgs e)
    {
        this.Close();
    }
}