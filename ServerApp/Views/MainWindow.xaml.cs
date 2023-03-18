using System.Windows;
using ServerApp.ViewModels;



namespace ServerApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel(wrapPanel);
            this.DataContext = viewModel;
        }
    }
}
