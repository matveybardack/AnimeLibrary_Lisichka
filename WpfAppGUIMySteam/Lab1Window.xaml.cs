using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfAppGUIMySteam
{
    /// <summary>
    /// Логика взаимодействия для Lab1Window.xaml
    /// </summary>
    public partial class Lab1Window : Window
    {
        public Lab1Window()
        {
            InitializeComponent();
            this.DataContext = new Lab1ViewModel();
        }
    }

    public class Lab1ViewModel
    {
        public ICommand BackCommand { get; }

        public Lab1ViewModel()
        {
            BackCommand = new RelayCommand(BackToMain);
        }

        private void BackToMain()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = mainWindow;
        }
    }
}
