using System.Windows;
using System.Windows.Input;

namespace WpfAppGUIMySteam
{
    public partial class Lab2Window : Window
    {
        public Lab2Window()
        {
            InitializeComponent();
            this.DataContext = new Lab2ViewModel();
        }
    }

    public class Lab2ViewModel
    {
        public ICommand BackCommand { get; }

        public Lab2ViewModel()
        {
            BackCommand = new RelayCommand(BackToMain);
        }

        private void BackToMain()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Закрываем текущее окно
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Lab2Window)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}