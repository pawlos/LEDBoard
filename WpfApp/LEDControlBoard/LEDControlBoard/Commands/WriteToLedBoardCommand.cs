using System;
using System.IO.Ports;
using System.Windows.Input;
using LEDControlBoard.Logic;

namespace LEDControlBoard.Commands
{
    public class WriteToLedBoardCommand : ICommand
    {
        private readonly SerialPort _serialPort;

        public WriteToLedBoardCommand(SerialPort serialPort)
        {
            if (serialPort == null) throw new ArgumentNullException("serialPort");
            _serialPort = serialPort;
        }

        public void Execute(object parameter)
        {
            string textToWrite = parameter.ToString();
            using (var controller = new BoardController(_serialPort))
            {
                controller.WriteLine(textToWrite);
            }            
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}