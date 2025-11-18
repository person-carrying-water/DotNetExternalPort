using General;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ExternalPort
{
    /// <summary>TcpClientクラスを使いイーサネット通信を行うクラス</summary>
    /// <remarks></remarks>
    public class DotNetStreamPort : IExternalPort, IDisposable
    {
        #region "フィールド"
        protected TcpClient _tcpSock;
        protected NetworkStream? _netStm;
        protected IPAddress? _ipAddr;
        protected ushort _port;
        protected bool _openFlag;
        protected ushort _openTimeout;
        protected int _receiveSize;
        protected int _sendSize;
        protected int _receiveTimeout;
        protected int _sendTimeout;
        protected string _sendTerminate = "\r";
        protected string _receiveTerminate = "\r\n";

        #endregion

        #region "コンストラクタ"
        public DotNetStreamPort()
        {
            _tcpSock = new TcpClient();
        }
        #endregion

        #region "メソッド"
        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool Connect()
        {
            if (_tcpSock.Connected == false)
            {
                _tcpSock.Connect(_ipAddr, _port);
                _netStm = _tcpSock.GetStream();
                return true;
            }

            return false;

        }

        public async Task<bool> ConnectAsync()
        {
            if (_tcpSock.Connected == false)
            {
                await _tcpSock.ConnectAsync(_ipAddr, _port);
                _netStm = _tcpSock.GetStream();
                return true;
            }

            return false;
        }


        public void Dispose()
        {
            if (_tcpSock != null)
            {
                _tcpSock.Close();
                _tcpSock.Dispose();
                _tcpSock = null;
            }
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
            string res = "";
            var encode = Encoding.UTF8;

            using (var bs = new BufferedStream(_netStm))
            using (var sr = new StreamReader(bs, encode))
            {
                res = sr.ReadToEnd();
            }

            return res;
        }

        public async Task<string> ReciveAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region "プロパティ"
        public IPAddress IPAddr
        {
            get => _ipAddr;
            set => _ipAddr = value;
        }

        public ushort Port
        {
            get => _port;
            set => _port = value;
        }
        #endregion
    }
}
