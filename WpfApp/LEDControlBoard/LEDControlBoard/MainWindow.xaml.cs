using System;
using System.IO;
using System.IO.Ports;
using System.Windows;
using LEDControlBoard.ViewModel;

namespace LEDControlBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }       
    }
}
