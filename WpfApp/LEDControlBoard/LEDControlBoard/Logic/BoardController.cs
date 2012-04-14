using System;
using System.Linq;
using System.IO.Ports;

namespace LEDControlBoard.Logic
{
    public class BoardController : IDisposable
    {
        private readonly SerialPort _serialPort;

        public BoardController(SerialPort serialPort)
        {
            if (serialPort == null) throw new ArgumentNullException("serialPort");
            _serialPort = serialPort;
        }

        public bool Write(string text)
        {
            _serialPort.Write(new byte[] {0}, 0, 1);
            for (int i = 0; i < 4; i++)
            {
                var data = new byte[0x45];
                _serialPort.Write(CreatePackage(data, i, text), 0, data.Length);
            }
            return true;
        }

        private byte[] CreatePackage(byte[] data, int i, string text)
        {
            var start = (byte) (i << 6);
            data[0] = 0x02;
            data[1] = 0x31;
            data[2] = 0x06;
            data[3] = start;            
            if (start < text.Length)
            {
                data[4] = 0x35;
                data[5] = 0x31;
                data[6] = 0x42;
                data[7] = (byte) text.Length;
                Array.Copy(text.Select(x=>(byte)x).ToArray(), 0, data, 8, text.Length);
            }
            data[data.Length - 1] = (byte) (data.Sum(x => (byte) x) - 2);
            return data;
        }

        public void Dispose()
        {
            _serialPort.Write(new byte[] {0x2, 0x33, 1}, 0, 3);
        }
    }
}