using System;
using System.Linq;
using System.IO.Ports;

namespace LEDControlBoard.Logic
{
    public class BoardController : IDisposable
    {
        private readonly SerialPort _serialPort;
        private static int _lineNo = -1;
        public BoardController(SerialPort serialPort)
        {
            if (serialPort == null) throw new ArgumentNullException("serialPort");
            _serialPort = serialPort;
        }

        public bool WriteLine(string text)
        {
            _lineNo++;
            if (_lineNo > 8)
                _lineNo = -1;
            _serialPort.Write(new byte[] {0}, 0, 1);
            for (int packageNo = 0; packageNo < 4; packageNo++)
            {
                var data = new byte[0x45];
                _serialPort.Write(CreatePackage(data, _lineNo, packageNo, text), 0, data.Length);
            }            
            return true;
        }

        private byte[] CreatePackage(byte[] data, int lineNo, int packageNo, string text)
        {
            var start = (byte) (packageNo << 6);
            data[0] = 0x02;
            data[1] = 0x31;
            data[2] = (byte) (lineNo + 6);
            data[3] = start;            
            if (start < text.Length)
            {
                if (packageNo == 0)
                {
                    data[4] = 0x35;
                    data[5] = (byte) (lineNo + 0x31);
                    data[6] = 0x42;
                    data[7] = (byte) text.Length;
                }
                Array.Copy(text.Select(x=>(byte)x).ToArray(), start, data, packageNo == 0 ? 8 : 4, Math.Min(text.Length-start, 60));
            }
            data[data.Length - 1] = (byte) (data.Sum(x => x) - 2);
            return data;
        }

        public void Dispose()
        {
            var checkSum = (byte) (0xFF >> (8 - (_lineNo + 1)));
            _serialPort.Write(new byte[] {0x2, 0x33, checkSum}, 0, 3);
        }
    }
}