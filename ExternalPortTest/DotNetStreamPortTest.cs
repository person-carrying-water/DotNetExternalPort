using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ExternalPort;

namespace ExternalPortTest
{
    public class DotNetStreamPortTest
    {
        [Fact]
        public void ConnectTest()
        {
            using (var sock = new DotNetStreamPort())
            {
                sock.IPAddr = IPAddress.Parse("192.168.84.60");
                sock.Port = 50001;
                sock.Connect();
            }

            Assert.True(true);
        }
    }
}
