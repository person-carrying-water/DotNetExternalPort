using System;
using System.Collections.Generic;
using System.Text;

namespace ExternalPort
{
    /// <summary>外部デバイス接続スーパークラス。</summary>
    /// <remarks></remarks>
    internal interface IExternalPort
    {
        #region "列挙型"
        public enum Terminate : int
        {
            CR = 1,
            LF = 2,
            CRLF = 3
        }
        #endregion

        #region "メソッド定義"
        bool Connect();

        Task<bool> ConnectAsync();

        void Close();

        void Send(string cmd);

        Task SendAsync(string cmd);

        byte[] Receive();

        Task<byte[]> ReciveAsync();
        #endregion
    }
}
