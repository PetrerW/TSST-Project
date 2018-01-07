using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SubNetwork
{
    [TestClass]
    public class RCTester
    {
        [TestMethod]
        public void testSNPPsToString()
        {
            List<SubNetworkPointPool> snpps = new List<SubNetworkPointPool>();
            SubNetworkPoint snp1 = new SubNetworkPoint(IPAddress.Parse("127.0.0.1"), 1, 2);
            SubNetworkPoint snp2 = new SubNetworkPoint(IPAddress.Parse("127.0.0.2"), 3, 4);
            SubNetworkPoint snp3 = new SubNetworkPoint(IPAddress.Parse("127.0.0.3"), 5, 6);
            snpps.Add(new SubNetworkPointPool(snp1));
            snpps.Add(new SubNetworkPointPool(snp2));
            snpps.Add(new SubNetworkPointPool(snp3));

            RoutingController RC = new RoutingController();

            string message = RC.snppsToString(snpps);

            Assert.AreEqual("127.0.0.1#2#3#127.0.0.2#4#5#127.0.0.3", message);
        }

        [TestMethod]
        public void testGenerateGetPathFromRCMessage()
        {
            RoutingController RC = new RoutingController();
            SubNetworkPointPool Source = new SubNetworkPointPool(new SubNetworkPoint(IPAddress.Parse("127.0.0.1")));
            SubNetworkPointPool Destination = new SubNetworkPointPool(new SubNetworkPoint(IPAddress.Parse("127.0.0.9")));

            string message = RC.generateGetPathFromRCMessage(Source, Destination, 8, 5);
            
            Assert.AreEqual("ROUTE_PATH\n127.0.0.1\n127.0.0.9\n8\n5", message);
        }

    }
}
