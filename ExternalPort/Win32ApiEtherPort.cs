using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::System.Runtime.InteropServices;

namespace ExternalPort
{
    internal class Win32ApiEtherPort
    {
        private const string WSOCK32_DLL = "wsock32.dll";
        private const string WS2_32_DLL = "Ws2_32.dll";

        #region "WindowsAPI構造体・P/Invoke呼び出し、宣言"
        [DllImport(WSOCK32_DLL, SetLastError = true, EntryPoint = "WSAStartup")]
        private static extern int WSAStartup([In] short wVersionRequested, [Out] out WSAData lpWSAData);

        [DllImport(WSOCK32_DLL, SetLastError = true, EntryPoint = "WSACleanup")]
        private static extern int WSACleanup();

        [DllImport(WS2_32_DLL, SetLastError = true, EntryPoint = "WSAGetLastError")]
        private static extern int WSAGetLastError();

        [DllImport(WS2_32_DLL, SetLastError = true, EntryPoint = "getservbyname")]
        private static extern IntPtr getservbyname(string strName, string strProto);

        //API to get list of connections 
        [DllImport("iphlpapi.dll")]
        private static extern int GetTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder);

        //API to change status of connection 
        [DllImport("iphlpapi.dll")]
        private static extern int SetTcpEntry(IntPtr pTcprow);

        //Convert 16-bit value from network to host byte order 
        [DllImport("wsock32.dll")]
        private static extern int ntohs(int netshort);

        //Convert 16-bit value back again 
        [DllImport("wsock32.dll")]
        private static extern int htons(int netshort);

        [DllImport("WSOCK32.DLL", SetLastError = true)]
        internal static extern long gethostname(string name, int nameLen);
        #endregion

        #region "構造体"
        [StructLayout(LayoutKind.Sequential)]
        private struct WSAData
        {
            public short wVersion;
            public short wHighVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)] public string szDescription;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)] public string szSystemStatus;
            public short iMaxSockets;
            public short iMaxUdpDg;
            public IntPtr lpVenderInfo;
        }
        #endregion
    }
}
