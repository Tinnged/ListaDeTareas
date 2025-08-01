using ListaTareas.MVVM.ViewModels;
using CommunityToolkit.Maui.Views;

namespace ListaTareas.MVVM.Views;

public partial class EditTarea : Popup
{
    public EditTarea()
    {
        InitializeComponent();
        BindingContext = new TareasViewModel();
    }

    private void CloseEditPopUp(object sender, EventArgs e)
    {
        this.Close();
    }
}