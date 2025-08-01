using ListaTareas.MVVM.Views;

namespace ListaTareas
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ListaDeTareas();
        }
    }
}
