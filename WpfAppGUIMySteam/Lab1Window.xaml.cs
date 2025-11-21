using System.Windows;
using System.Windows.Input;

namespace WpfAppGUIMySteam
{
    public partial class Lab1Window : Window
    {
        public Lab1Window()
        {
            InitializeComponent();
            this.DataContext = new Lab1ViewModel();
        }
    }
}