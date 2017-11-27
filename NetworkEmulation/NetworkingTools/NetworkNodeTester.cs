using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkingTools;

namespace NetworkNode
{
    [TestClass]
    public class NetworkNodeTester
    {
        //TODO: popraw to 
        [TestMethod]
        public void testFindCloudSocketAndPort1()
        {
            //Jakies dane do pakietu
            string inscription = "Mam malo wody";
            short portNumber = 2;
            IPAddress IP_Source = IPAddress.Parse("192.168.0.1");
            IPAddress IP_Destination = IPAddress.Parse("192.168.0.10");
            short packageNumber = 5;
            short frequency = 2;
            short band = 4;
            short usableInfoLength = (short)inscription.Length;

            //adres socketa z chmury
            short cloudSocketPort = 1;
            IPAddress cloudSocketIPAddress = IPAddress.Parse("127.0.10.12");

            //stworzenie nowego pakietu
            Package P = new Package(inscription, portNumber, IP_Destination.ToString(), IP_Source.ToString(),
                (short) inscription.Length, packageNumber, frequency, band);

            //Nowy wpis do tablicy komutacji pakietow, zawierajacy czestotliwosc z powyzszego pakietu
            CommutationTableRow row = new CommutationTableRow(frequency, cloudSocketPort, cloudSocketIPAddress.ToString());

            Assert.IsNotNull(row);

            NetworkNode node = new NetworkNode();

            Assert.IsNotNull(node.borderNodeCommutationTable);
            Assert.IsNotNull(node.commutationTable);
            Assert.IsNotNull(node.eonTable);

            //dodanie do tablicy komutacji pakietow wezla sieciowego nowego rzedu
            node.commutationTable.Table.Add(row);

            //wyodrebnienie adresu IP socketa chmury i portu tego socketa
            var tuple = node.determineCloudSocketIPAndPort(P.toBytes());
           
            //sprawdzenie, czy sie zgadzaja
            Assert.AreEqual(tuple.Item1, cloudSocketIPAddress);
            Assert.AreEqual(tuple.Item2, cloudSocketPort);
        }
    }
}
