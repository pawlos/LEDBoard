using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Windows.Input;
using LEDControlBoard.Commands;

namespace LEDControlBoard.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private SerialPort _serialPort;
        public MainWindowViewModel()
        {
            ComDetected = DetectSerialCom();
        }

        public ICommand ProgramBoard
        {
            get { return new WriteToLedBoardCommand(_serialPort); }
        }

        private string _textToBeWritten;
        public string TextToBeWritten
        {
            get { return _textToBeWritten; }
            set
            {
                _textToBeWritten = value;
                OnNotifyPropertyChanged("TextToBeWritten");
            }
        }

        private void OnNotifyPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler == null) return;
            handler(this, new PropertyChangedEventArgs(property));
        }


        public bool ComDetected { get; private set; }

        private bool DetectSerialCom()
        {
            foreach (var portName in SerialPort.GetPortNames())
            {
                _serialPort = new SerialPort(portName, 38400, Parity.None, 8, StopBits.One) { ReadTimeout = 10 };
                try
                {
                    _serialPort.Open();
                }
                catch (IOException)
                {
                    continue;
                }
                try
                {
                    _serialPort.Write("T");
                    int read = _serialPort.ReadChar();
                    if ((char)read == 'T')
                        return true;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}