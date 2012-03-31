using System;
using System.IO;
using System.IO.Ports;
using System.Windows;

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
            DataContext = DetectSerialCom();
        }

        private string DetectSerialCom()
        {
            for (int i = 1; i <= 8; i++)
            {
                var serialPort = new SerialPort("COM" + i, 38400, Parity.None, 8, StopBits.One) {ReadTimeout = 10};
                try
                {
                    serialPort.Open();
                }
                catch (IOException)
                {
                    continue;                    
                }
                try
                {
                    serialPort.Write("T");
                    int read = serialPort.ReadChar();
                    if ((char)read == 'T')
                        return serialPort.PortName;
                }
                catch (Exception)
                {                    
                    continue;
                }
            }
            return "None";
        }
    }
}
