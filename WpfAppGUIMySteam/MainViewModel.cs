using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfAppGUIMySteam
{
    public class MainViewModel
    {
        public ICommand OpenLab1Command { get; }
        public ICommand OpenLab2Command { get; }
        public ICommand OpenLab3Command { get; }
        public ICommand OpenLab4Command { get; }

        public MainViewModel()
        {
            OpenLab1Command = new RelayCommand(OpenLab1);
            OpenLab2Command = new RelayCommand(OpenLab2);
            OpenLab3Command = new RelayCommand(OpenLab3);
            OpenLab4Command = new RelayCommand(OpenLab4);
        }

        private void OpenLab1()
        {
            // Простой вариант без Application
            var lab1Window = new Lab1Window();
            lab1Window.Show();

            // Закрываем текущее окно
            foreach (Window window in App.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void OpenLab2()
        {
            var lab2Window = new Lab2Window();
            lab2Window.Show();

            foreach (Window window in App.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void OpenLab3()
        {
            var lab3Window = new Lab3Window();
            lab3Window.Show();

            foreach (Window window in App.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void OpenLab4()
        {
            var lab4Window = new Lab4Window();
            lab4Window.Show();

            foreach (Window window in App.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute?.Invoke();
    }
}
