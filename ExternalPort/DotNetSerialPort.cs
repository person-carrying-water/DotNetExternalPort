using global::System.IO.Ports;
using global::System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ExternalPort
{
    /// <summary>.NET シリアル通信クラス</summary>
    /// <remarks>Install-Package System.IO.Ports -Version 10.0.0</remarks>
    internal class DotNetSerialPort : IExternalPort
    {
        #region "フィールド"
        protected SerialPort _serialPort = new SerialPort();
        protected string _portName = string.Empty; //WinNT系はCOM1～COM255、Win9x系はCOM1～COM127、MSComm.ocxは数値入力で1～9、Win32APIはCOM10以上は"\\\\.\\COM10"とする必要がある。
        protected BaudRateA _BaudRate;
        protected DataBitsA _DataBits;
        protected Parity _Parity;
        protected StopBits _StopBits;
        protected int _ReceiveBufferSize;
        protected int _SendBufferSize;
        protected int _ReadTimeout;
        protected int _WriteTimeout;
        protected bool _DiscardNull;
        protected bool _DtrEnable;
        protected bool _RtsEnable;
        protected Handshake _HandShake;
        protected string _sendTerminate = "\r";
        protected string _receiveTerminate = "\r\n";
        #endregion

        #region "列挙型"
        public enum BaudRateA : int
        {
            BPS_75 = 1,
            BPS_110 = 2,
            BPS_300 = 3,
            BPS_1200 = 4,
            BPS_2400 = 5,
            BPS_4800 = 6,
            BPS_9600 = 7,
            BPS_19200 = 8,
            BPS_38400 = 9,
            BPS_57600 = 10,
            BPS_115200 = 11
        }

        public enum DataBitsA : int
        {
            Bit7 = 0,
            Bit8 = 1
        }

        public enum ParityA
        {
            None = 0,
            Odd = 1,
            Even = 2,
            Mark = 3,
            Space = 4
        }

        public enum StopBitsA : int
        {
            Bit0 = 0,
            Bit1 = 1,
            Bit1p5 = 2,
            Bit2 = 3
        }

        public enum HandShakeA : int
        {
            None = 0,
            OnOff = 1,
            Rts = 2,
            RtsOnOff = 3
        }
        #endregion

        #region "メソッド"
        protected void ConecSetting()
        {

            _serialPort.PortName = _portName;
            _serialPort.BaudRate = (int)_BaudRate;
            _serialPort.DataBits = (int)_DataBits;
            _serialPort.Parity = _Parity;
            _serialPort.StopBits = _StopBits;
            _serialPort.ReadBufferSize = _ReceiveBufferSize;
            _serialPort.WriteBufferSize = _SendBufferSize;
            _serialPort.ReadTimeout = _ReadTimeout;
            _serialPort.WriteTimeout = _WriteTimeout;
            _serialPort.DiscardNull = _DiscardNull;
            _serialPort.DtrEnable = _DtrEnable;
            _serialPort.RtsEnable = _RtsEnable;
            _serialPort.Handshake = _HandShake;
        }
        public bool Connect()
        {
            var success = true;

            ConecSetting();
            try
            {
                if (_serialPort.IsOpen == false)
                    _serialPort.Open();
            }
            catch (Exception ex)
            {
                success = false;
                throw;
            }

            return success;
        }

        public async Task<bool> ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        public void BreakSignal(ushort milli)
        {
            _serialPort.BreakState = true;
            Thread.Sleep(milli);
            _serialPort.BreakState = false;
        }

        public string Send(string cmd)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SendAsync(string cmd)
        {
            throw new NotImplementedException();
        }

        public string Receive()
        {
            throw new NotImplementedException();
        }

        public async Task<string> ReciveAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region "プロパティ"
        /// <summary>シリアルポート</summary>
        /// <returns></returns>
        /// <remarks>System.IO.Ports.SerialPort</remarks>
        public SerialPort SerialPort
        {
            get => _serialPort;
            set => _serialPort = value;
        }

        /// <summary>ボーレート</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public BaudRateA BaudRate
        {
            get => _BaudRate;
            set => _BaudRate = value;
        }

        /// <summary>データビット</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataBitsA DataBits
        {
            get => _DataBits;
            set => _DataBits = value;
        }

        /// <summary>パリティビット</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public Parity Parity
        {
            get => _Parity;
            set => _Parity = value;
        }

        /// <summary>ストップビット</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public StopBits StopBits
        {
            get => StopBits;
            set => StopBits = value;
        }

        /// <summary>受信バッファサイズ</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int ReceiveBufferSize
        {
            get => _ReceiveBufferSize;
            set => _ReceiveBufferSize = value;
        }

        /// <summary>送信バッファサイズ</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int SendBufferSize
        {
            get => _SendBufferSize;
            set => _SendBufferSize = value;
        }

        /// <summary>読み取りタイムアウト</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int ReadTimeout
        {
            get => _ReadTimeout;
            set => _ReadTimeout = value;
        }

        /// <summary>書き込みタイムアウト</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int WriteTimeout
        {
            get => _WriteTimeout;
            set => _WriteTimeout = value;
        }

        public bool DiscardNull
        {
            get => _DiscardNull;
            set => _DiscardNull = value;
        }

        public bool DtrEnable
        {
            get => _DtrEnable;
            set => _DtrEnable = value;
        }

        public bool RtsEnable
        {
            get => _RtsEnable;
            set => _RtsEnable = value;
        }

        public Handshake HandShake
        {
            get => _HandShake;
            set => _HandShake = value;
        }
        #endregion
    }
}
