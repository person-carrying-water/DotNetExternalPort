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

        /*Public Function TcpReceive() As String

        ReadStr = ""
        Dim UEncode As System.Text.Encoding
        UEncode = System.Text.Encoding.UTF8
        Dim ReceiveBuffer As Byte()
        Dim oks As Boolean = False
        Dim rsw As New System.Diagnostics.Stopwatch

        rsw.Reset()
        rsw.Start()
        Do
            If TcpSock.Available > 0 Then
                '受信文字数による配列の確保
                ReceiveBuffer = New Byte(TcpSock.Available - 1) {}
                '読取
                NetStm.Read(ReceiveBuffer, 0, ReceiveBuffer.GetLength(0))
                '受信したByteをUTF-8で文字列に置き換え
                ReadStr = UEncode.GetString(ReceiveBuffer) 'System.Text.Encoding.GetEncoding("Shift-JIS").GetString(ReceiveBuffer)
                oks = True
            End If
        Loop Until oks = True Or rsw.ElapsedMilliseconds > Me.receivetimeout
        rsw.Stop()

        '受信タイムアウト
        If rsw.ElapsedMilliseconds > Me.ReceiveTimeout Then
            ReadStr = "E11"
        Else
            'キャリッジリターンの除去
            ReadStr = Replace(ReadStr, ReceiveTerminate, "")
            ReadStr = Replace(ReadStr, ",", "")
        End If

        Return ReadStr

    End Function

    ''' <summary>
    ''' コマンドを送信しTcpReceiveメソッドを実行し受信バッファの内容を返します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TcpSend(ByVal SendCommand As String) As String

        Dim UEncode As System.Text.Encoding
        UEncode = System.Text.Encoding.UTF8

        '前のレスポンスが残っているか？
        ReadStr = ""
        If TcpSock.Available > 0 Then
            Dim ReceiveBuffer As Byte()
            '受信文字数による配列の確保
            ReceiveBuffer = New Byte(TcpSock.Available - 1) {}
            '読取
            NetStm.Read(ReceiveBuffer, 0, ReceiveBuffer.GetLength(0))
            '受信したByteをUTF-8で文字列に置き換え
            ReadStr = UEncode.GetString(ReceiveBuffer)
            MakerCom.PlcErrCheck("E10", Maker, ReadStr)
        End If

        Dim SendBuffer As Byte()

        '引数のコマンド文字列をUTF-8ｺｰﾄﾞでByteに置き換え
        SendBuffer = UEncode.GetBytes(SendCommand & SendTerminate)
        Try
            'Socketの接続状態確認
            If TcpSock.Client.Poll(-1, Net.Sockets.SelectMode.SelectWrite) Then
                '送信
                NetStm.Write(SendBuffer, 0, SendBuffer.GetLength(0))
            Else
                '切断
                Return "E9"
            End If
        Catch ex As IO.IOException
            'Writeタイムアウト
            Return "E8"
        End Try

        Return TcpReceive()

    End Function*/

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
