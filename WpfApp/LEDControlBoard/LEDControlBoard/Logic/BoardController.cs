using System;
using System.Linq;
using System.IO.Ports;

namespace LEDControlBoard.Logic
{
    public class BoardController : IDisposable
    {
        private enum Speed : byte
        {
            Fast = 0x35,
        }

        private enum Effect : byte
        {
            Scroll = 0x42
        }
        private readonly SerialPort _serialPort;
        private static int _lineNo = -1;
        private const int PackageSize = 69;
        private const int MaxDataSize = 60;
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
                _serialPort.Write(CreatePackage(_lineNo, packageNo, text), 0, PackageSize);
            }            
            return true;
        }

        /// <remarks>
        /// Index - Value
        /// 0     -  0x02 (const)
        /// 1     -  0x31 (const)
        /// 2     -  line no + 6
        /// 3     -  package no * 64; 0x00, 0x40, 0x80, 0xC0
        /// 4     -  speed / data byte
        /// 5     -  line no + 0x31 / data byte
        /// 6     -  effect / data byte
        /// 7     -  length / data byte
        /// 8-68  -  data bytes
        /// 69    -  checksum
        /// </remarks>        
        private static byte[] CreatePackage(int lineNo, int packageNo, string text)
        {
            byte[] data = new byte[PackageSize];
            var startIndex = (byte) (packageNo << 6);
            data[0] = 0x02;
            data[1] = 0x31;
            data[2] = (byte) (lineNo + 6);
            data[3] = startIndex;
            if (IncludeLineInfo(packageNo))
            {
                data[4] = (byte)Speed.Fast;
                data[5] = (byte)(lineNo + 0x31);
                data[6] = (byte)Effect.Scroll;
                data[7] = (byte)text.Length;
            }
            if (HasMoreData(startIndex, text))
            {
                Array.Copy(text.Select(x => (byte) x).ToArray(), startIndex, data, packageNo == 0 ? 8 : 4,
                           Math.Min(text.Length - startIndex, MaxDataSize));
            }
            data[data.Length - 1] = (byte) (data.Sum(x => x) - 2);
            return data;
        }

        private static bool HasMoreData(byte startIndex, string text)
        {
            return startIndex < text.Length;
        }

        private static bool IncludeLineInfo(int packageNo)
        {
            return packageNo == 0;
        }

        public void Dispose()
        {
            var checkSum = (byte) (0xFF >> (8 - (_lineNo + 1)));
            _serialPort.Write(new byte[] {0x2, 0x33, checkSum}, 0, 3);
        }
    }
}