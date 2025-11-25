using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::System.Runtime.InteropServices;
using global::General;
using static ExternalPort.Win32ApiSerialPort;

namespace ExternalPort
{
    internal class Win32ApiSerialPort : IWin32ApiFile
    {
        #region "フィールド"
        private IntPtr hSerialPort;
        private Encoding oEnc = ASCIIEncoding.Default;
        private uint _WriteBufferSize;
        private uint _ReadBufferSize;
        #endregion

        #region "P/Invoke構造体、宣言、呼び出し"
        /// <summary>指定した通信デバイスの現在の制御設定をデバイス制御ブロック（DCB 構造体）に格納します。</summary>
        /// <param name="hFile">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="lpDCB">DCB 構造体へのポインタを指定します。制御設定の情報が格納されます。</param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL GetCommState(HANDLE hFile, LPDCB lpDCB);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "GetCommState")]
        private static extern bool GetCommState(IntPtr hFile, DCB lpDCB);

        /// <summary>デバイス制御ブロック（DCB 構造体）の指定に従って通信デバイスを構成します。
        /// ハードウェアと制御の設定をすべて初期化しますが、出力待ち行列と入力待ち行列は空にしません。</summary>
        /// <param name="hFile">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="lpDCB">hFile で指定する通信デバイスの構成情報が入った DCB 構造体へのポインタを指定します。</param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL SetCommState(HANDLE hFile, LPDCB lpDCB);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "SetCommState")]
        private static extern bool SetCommState(IntPtr hFile, DCB lpDCB);

        /// <summary>指定した通信デバイスの通信パラメータを初期化します。</summary>
        /// <param name="hCommDev">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="cbInQueue">デバイスの内部入力バッファの推奨サイズ（バイト数）を指定します。</param>
        /// <param name="cbOutQueue">デバイスの内部出力バッファの推奨サイズ（バイト数）を指定します。 </param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL SetupComm(HANDLE hFile, DWORD dwInQueue, DWORD dwOutQueue);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "SetupComm")]
        private static extern bool SetupComm(IntPtr hCommDev, uint cbInQueue, uint cbOutQueue);

        /// <summary>指定した通信デバイスで実行されるすべての読み書き操作のタイムアウトパラメータを設定します。</summary>
        /// <param name="hFile">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="lpCommTimeouts">新しいタイムアウト値が入った COMMTIMEOUTS 構造体へのポインタを指定します。</param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL SetCommTimeouts(HANDLE hFile, LPCOMMTIMEOUTS lpCommTimeouts);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "SetCommTimeouts")]
        private static extern bool SetCommTimeouts(IntPtr hFile, COMMTIMEOUTS lpCommTimeouts);

        /// <summary>指定した通信デバイスで実行されるすべての読み書き操作のタイムアウトパラメータを取得します。</summary>
        /// <param name="hFile">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="lpCommTimeouts">COMMTIMEOUTS 構造体へのポインタを指定します。タイムアウトの情報が格納されます。</param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL GetCommTimeouts(HANDLE hFile, LPCOMMTIMEOUTS lpCommTimeouts);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "GetCommTimeouts")]
        private static extern bool GetCommTimeouts(IntPtr hFile, COMMTIMEOUTS lpCommTimeouts);

        /// <summary>指定した通信デバイスに、拡張機能を実行するよう指示します。</summary>
        /// <param name="hFile">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="dwFunc">実行する拡張機能のコードを指定します。</param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL EscapeCommFunction(HANDLE hFile, DWORD dwFunc);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "EscapeCommFunction")]
        private static extern bool EscapeCommFunction(IntPtr hFile, uint dwFunc);

        /// <summary>モデムの制御レジスタ値を取得します。</summary>
        /// <param name="hFile">通信資源のハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="lpModemStat">2 ビット変数へのポインタを指定します。変数には、モデム制御レジスタの現在の状態を示す値が格納されます。</param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL GetCommModemStatus(HANDLE hFile, LPDWORD lpModemStat);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "GetCommModemStatus")]
        private static extern bool GetCommModemStatus(IntPtr hFile, UIntPtr lpModemStat);

        /// <summary>指定した通信資源の出力バッファまたは入力バッファにあるすべての文字を破棄します。
        /// 未処理の読み取り操作または書き込み操作を中止することもできます。</summary>
        /// <param name="hFile">通信資源のハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="dwFlags">実行する操作を指定します。次の定数を組み合わせて渡すことができます。 </param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL PurgeComm(HANDLE hFile, DWORD dwFlags);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "PurgeComm")]
        private static extern bool PurgeComm(IntPtr hFile, Purge dwFlags);

        /// <summary>指定した通信デバイスの文字送信を中断し、ClearCommBreak 関数が呼び出されるまで送信回線を切断状態にします。</summary>
        /// <param name="hFile">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL SetCommBreak(HANDLE hFile);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "SetCommBreak")]
        private static extern bool SetCommBreak(IntPtr hFile);

        /// <summary>指定した通信デバイスの回線切断状態を解除し、文字送信を再開します。</summary>
        /// <param name="hFile">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL ClearCommBreak(HANDLE hFile);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "ClearCommBreak")]
        private static extern bool ClearCommBreak(IntPtr hFile);

        /// <summary>特定の通信デバイスで監視する一連のイベントを指定します。</summary>
        /// <param name="hFile">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="dwEvtMask">監視するイベントを指定します。0 を渡すと、どのイベントも監視しません。１つ以上の定数を組み合わせて渡すことができます。 </param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL SetCommMask(HANDLE hFile, DWORD dwEvtMask);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "SetCommMask")]
        private static extern bool SetCommMask(IntPtr hFile, uint dwEvtMask); //シリアルポートよりの割り込み信号を全て不許可にする

        /// <summary>通信エラーの情報を取得して、通信デバイスの現在の状態を通知します。
        /// 通信エラーが発生した場合に呼び出し、デバイスのエラーフラグをクリアして次の入出力（I/O）操作を可能にします。</summary>
        /// <param name="hFile">通信デバイスのハンドルを指定します。CreateFile 関数が返すハンドルを使います。</param>
        /// <param name="lpErrors">エラーの種類を示すマスクを受け取る 32 ビット変数へのポインタを指定します。１つ以上のエラーコードが格納されます。</param>
        /// <param name="lpStat">COMSTAT 構造体へのポインタを指定します。この構造体を使って、デバイスの状態情報を受け取ります。NULL を指定した場合、状態情報を受け取りません。</param>
        /// <returns>True=成功、False=失敗</returns>
        /// <remarks>BOOL ClearCommError(HANDLE hFile, LPDWORD lpErrors, LPCOMSTAT lpStat);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "ClearCommError")]
        private static extern bool ClearCommError(IntPtr hFile, UIntPtr lpErrors, int lpStat);

        /// <summary>呼び出し側のスレッドが持つ最新のエラーコードを取得します。エラーコードは、スレッドごとに保持されるため、複数のスレッドが互いの最新のエラーコードを上書きすることはありません。</summary>
        /// <returns>呼び出し側のスレッドが持つ最新のエラーコードが返ります。</returns>
        /// <remarks>DWORD GetLastError(VOID);</remarks>
        [DllImport(IWin32ApiFile.KERNEL32_DLL, SetLastError = true, EntryPoint = "GetLastError")]
        private static extern uint GetLastError();

        #endregion

        #region "構造体"
        /// <summary>DCB構造体</summary>
        /// <remarks></remarks>
        private struct DCB
        {
            public uint DCBlength;      //DCB構造体のサイズ(バイト数)
            public uint BaudRate;       //ボーレート設定。CBR_19200など。
            public FDCB fBitFields;     //32ﾋﾞｯﾄ(下位16ﾋﾞｯﾄ)のﾋﾞｯﾄ単位設定。FDCB参照
            public short wReserved;     //予約(0をセット)
            public short XonLim;        //受信バッファ中のデータが何バイトになったらXon文字を送るかを指定
            public short XoffLim;       //受信バッファの空きが何バイトになったらXoff文字を送るかを指定
            public byte ByteSize;       //7bitまたは8bit
            public byte Parity;         //パリティ
            public byte StopBits;       //ストップビット
            public char XonChar;        //Xon文字を指定
            public char XoffChar;       //Xoff文字を指定
            public char ErrorChar;      //パリティエラーの場合に使う文字を指定
            public char EofChar;        //非バイナリモードの場合のデータ終了文字の指定。\nなど
            public char EvtChar;        //イベントを生成する文字を指定
            public short wReserved1;    //予約。使用NG。
        }

        /// <summary>構造体DCBのfBitFieldに入る詳細設定</summary>
        /// <remarks></remarks>
        private struct FDCB
        {
            public bool fBinary;            //FALSE
            public bool fParity;            //FALSE
            public bool fOutxCtsFlow;       //FALSE
            public bool fOutxDsrFlow;       //FALSE
            public bool fDtrControl;        //DTR_CONTROL_DISABLE
            public bool fDtrControl2;       //(2ﾋﾞｯﾄ使う)
            public bool fDsrSensitivity;    //FALSE
            public bool fTXContinueOnXoff;  //FALSE
            public bool fOutX;              //FALSE
            public bool fInX;               //FALSE
            public bool fErrorChar;         //FALSE 
            public bool fNull;              //FALSE
            public bool fRtsControl;        //RTS_CONTROL_DISABLE
            public bool fRtsControl2;       //(2ﾋﾞｯﾄ使う)
            public bool fAbortOnError;      //FALSE
            public bool fDummy2;            //未使用
        }

        /// <summary>タイムアウト構造体</summary>
        /// <remarks></remarks>
        private struct COMMTIMEOUTS
        {
            public uint ReadIntervalTimeout;            //文字の読み込み待ち時間(ms)
            public uint ReadTotalTimeoutMultiplier;     //読み込みの１文字あたりの時間(ms)
            public uint uintReadTotalTimeoutConstant;   //読み込みの定数時間(ms)
            public uint WriteTotalTimeoutMultiplier;    //書き込みの１文字あたりの時間
            public uint WriteTotalTimeoutConstant;      //書き込みの定数時間
        }
        #endregion

        #region "列挙型"
        public enum Parity : int
        {
            NOPARITY = 0,   //パリティなし: NOPARITY
            ODDPARITY = 1,  //奇数パリティ: ODDPARITY
            EVENPARITY = 2  //偶数パリティ: EVENPARITY
        }

        private enum StopBits : int
        {
            ONESTOPBIT = 0,     //1ビット: ONESTOPBIT
            ONE5STOPBITS = 1,   //1.5ビット: ONE5STOPBITS
            TWOSTOPBITS = 2     //2ビット: TWOSTOPBITS
        }

        public enum DtrControl : int
        {
            Disable = 0,
            Enable = 1,
            Handshake = 2
        }

        public enum RtsControl : int
        {
            Disable = 0,
            Enable = 1,
            Handshake = 2,
            Toggle = 3
        }

        /// <summary>EscapeFuncFlag関数、拡張機能コード</summary>
        /// <remarks></remarks>
        public enum EscapeFuncFlag : uint
        {
            SETXOFF = 1,     //XOFF 文字を受信したときのように送信を行います。
            SETXON = 2,      //XON 文字を受信したときのように送信を行います。
            SETRTS = 3,      //RTS（ 送信要求）信号を送信します。
            CLRRTS = 4,      //RTS（送信要求）信号を消去します。
            SETDTR = 5,      //DTR（データ端末準備完了）信号を送信します。
            CLRDTR = 6,      //DTR（データ端末準備完了）信号を消去します。
            SETBREAK = 8,    //文字送信を中断し、送信回線を切断状態にします。SetCommBreak関数と同じです。
            CLRBREAK = 9    //送信回線の切断状態を解除して、文字送信を再開します。ClearCommBreak関数と同じです。
        }

        /// <summary>送受信バッファのクリア</summary>
        /// <remarks></remarks>
        public enum Purge : uint
        {
            TXABORT = 1, //操作が完了していないものも含め、未処理の書き込みのすべてを中止し、ただちに制御を返します。
            RXABORT = 2, //操作が完了していないものも含め、未処理の読み取りのすべてを中止し、ただちに制御を返します。
            TXCLEAR = 4, //出力バッファの内容を消去します（ デバイスドライバの出力バッファが存在する場合）。
            RXCLEAR = 8 //入力バッファの内容を消去します（ デバイスドライバの入力バッファが存在する場合）。
        }
        #endregion

        #region "メソッド"
        /// <summary>シリアルポートに接続します</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /*private bool Function Open()
        {
            DCB lDCB;
            COMMTIMEOUTS MyCommTimeouts;

            //シリアル ポートへのハンドルを取得します。
            hSerialPort = IWin32ApiFile.CreateFileA("COM3", DesiredAccess.GENERIC_READ Or DesiredAccess.GENERIC_WRITE, 0, IntPtr.Zero,
                CreationDisposition.OPEN_EXISTING, FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            //取得したハンドルが有効かどうか確認します。
            if (hSerialPort.ToInt32 = -1)
            {
                return false;
            }

            //現在のコントロールの設定を取得します。
            if (Not(GetCommState(hSerialPort, lDCB)))
            {
                return false;
            }

            lDCB.BaudRate = 115000;
            lDCB.ByteSize = 8;
            lDCB.Parity = NOPARITY;
            lDCB.StopBits = ONESTOPBIT;
            lDCB.fBitFields.fParity = true;

            //MyDCB のプロパティに基づいて COM? を再構成します。
            if (Not(SetCommState(hSerialPort, lDCB)))
            {
                return false;
            }

            //現在のタイムアウトの設定を取得します。
            if (Not(GetCommTimeouts(hSerialPort, MyCommTimeouts)))
            {
                return false;
            }

            // MyCommTimeouts のプロパティを必要に応じて変更します。
            // 警告 : プロパティでサポートされている値に応じて変更を行うようにしてください。
            MyCommTimeouts.ReadIntervalTimeout = -1;
            MyCommTimeouts.ReadTotalTimeoutConstant = 0;
            MyCommTimeouts.ReadTotalTimeoutMultiplier = 0;
            MyCommTimeouts.WriteTotalTimeoutConstant = 0;
            MyCommTimeouts.WriteTotalTimeoutMultiplier = 0;

            // MyCommTimeouts のプロパティに基づいてタイムアウトの設定を再構成します。
            if (Not(SetCommTimeouts(hSerialPort, MyCommTimeouts)))
            {
                return false;
            }

            return true;
        }*/

        /// <summary>シリアルポートを閉じます。</summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public bool Close()
        {
            bool success = true;
            if (hSerialPort != IntPtr.Zero)
            {
                PurgeComm(hSerialPort, Purge.TXABORT | Purge.RXABORT | Purge.TXCLEAR | Purge.RXCLEAR);
                if (IWin32ApiFile.CloseHandle(hSerialPort) == false)
                {
                    //err
                }
                hSerialPort = IntPtr.Zero;
            }

            return success;
        }

        /// <summary>受信バッファをクリアします。</summary>
        /// <remarks></remarks>
        public void DiscardInBuffer()
        {
            PurgeComm(hSerialPort, Purge.RXCLEAR);
        }

        /// <summary>送信バッファをクリアします。</summary>
        /// <remarks></remarks>
        public void DiscardOutBuffer()
        {
            PurgeComm(hSerialPort, Purge.TXCLEAR);
        }

        /// <summary>ブレークシグナルをセットします。</summary>
        /// <remarks></remarks>
        public void BreakSignalSet()
        {
            SetCommBreak(hSerialPort);
        }

        /// <summary>ブレークシグナルを解除します。</summary>
        /// <remarks></remarks>
        public void BreakSignalClear()
        {
            ClearCommBreak(hSerialPort);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public bool Send(string strSend)
        {
            //ClearCommErrorのCommStatのinQueueで受信バッファにあるバイト数を調べてからとる。
            byte[] Buffer;
            uint BytesWritten;
            Buffer = oEnc.GetBytes(strSend);

            if (IWin32ApiFile.WriteFile(hSerialPort, Buffer, (uint)Buffer.Length, out BytesWritten, IntPtr.Zero) == false)
            {

            }

            return true;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public string Receive()
        {
            uint BytesWritten, BytesRead = 0;
            BytesWritten = 1024;
            byte[] Buffer = new byte[BytesWritten];
            string str;

            if(IWin32ApiFile.ReadFile(hSerialPort, Buffer, BytesWritten, out BytesRead, IntPtr.Zero))
            {
                str = oEnc.GetString(Buffer);
            }
            else
            {
                str = "";
            }

            return str;
        }
        #endregion

        #region "プロパティ"
        public uint ReadBufferSize
        {
            get => _ReadBufferSize;
            set => _ReadBufferSize = value;
        }

        public uint WriteBufferSize
        {
            get => _WriteBufferSize;
            set => _WriteBufferSize = value;
        }
        #endregion
    }
}
