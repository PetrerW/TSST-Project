using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//Project->add reference->assemblies->extensions->Mircosoft.VisualStudio.QualityTools.UnitTestFramework, version 10.1.0.0
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingTools.cs
{
    /// <summary>
    /// Klasa do testowania NetworkingTools
    /// </summary>
    [TestClass]
    public class NetworkingToolsTester
    {

        /// <summary>
        /// Testuje konstruktor Package()
        /// </summary>
        [TestMethod]
        public void packageConstructorTest1()
        {
            Package P = new Package();

            //Czy adresy IP maja po 4 Bajty?
            Assert.AreEqual(P.IP_Destination.GetAddressBytes().Length, 4);
            Assert.AreEqual(P.IP_Source.GetAddressBytes().Length, 4);

            //Czy adresy IP sa dobrze wpisane?
            Assert.AreEqual(P.IP_Destination.ToString(), "127.0.0.1");
            Assert.AreEqual(P.IP_Source.ToString(), "127.0.0.1");
        }

        /// <summary>
        /// Testuje konstruktor Package(string usableMessage)
        /// </summary>
        [TestMethod]
        public void packageStringConstructorTest1()
        {
            //jakis napis
            string inscription = "Ala wcale nie ma zadnego kora!";

            //ten napis bedzie wiadomoscia uzytkowa pakietu
            Package P = new Package(inscription);

            //Czy te napisy sie zgadzaja?
            Assert.AreEqual(inscription, P.usableMessage);

            //Czy adresy IP maja po 4 Bajty?
            Assert.AreEqual(P.IP_Destination.GetAddressBytes().Length, 4);
            Assert.AreEqual(P.IP_Source.GetAddressBytes().Length, 4);

            //czy port jest dobrze wpisany?
            Assert.AreEqual(P.portNumber, 1);

            //Czy adresy IP sa dobrze wpisane?
            Assert.AreEqual(P.IP_Destination.ToString(), "127.0.0.1");
            Assert.AreEqual(P.IP_Source.ToString(), "127.0.0.1");

            IPAddress IP_Source = IPAddress.Parse("127.0.0.1");
            IPAddress IP_Destination = IPAddress.Parse("127.0.0.1");

            byte[] bytes = P.toBytes();

            //czy adres IP to domyslnie ustawiony 127.0.0.1?
            Assert.AreEqual(Package.exctractDestinationIP(bytes), IP_Destination);

            //czy adres IP to domyslnie ustawiony 127.0.0.1?
            Assert.AreEqual(Package.exctractSourceIP(bytes), IP_Source);

            //czy port wynikajacy z bajtow to domyslnie ustawiony port (1)? 
            Assert.AreEqual(Package.exctractPort(bytes), 1);
        }

        /// <summary>
        /// Testuje konstruktor Package(string usableMessage, short port, string IP_Source, string IP_Destination)
        /// </summary>
        [TestMethod]
        public void packageAllParamsConstructorTest()
        {
            //jakies wartosci moje
            string inscription = "Lubie placki";
            short portNumber = 3;
            IPAddress IP_Source = IPAddress.Parse("127.10.10.10");
            IPAddress IP_Destination = IPAddress.Parse("127.10.20.30");

            Package P = new Package(inscription, portNumber, IP_Source.ToString(),
                IP_Destination.ToString());

            //Czy te napisy sie zgadzaja?
            Assert.AreEqual(inscription, P.usableMessage);

            //Czy adresy IP maja po 4 Bajty?
            Assert.AreEqual(P.IP_Destination.GetAddressBytes().Length, 4);
            Assert.AreEqual(P.IP_Source.GetAddressBytes().Length, 4);

            //czy port jest dobrze wpisany?
            Assert.AreEqual(P.portNumber, portNumber);

            //Czy adresy IP sa dobrze wpisane?
            Assert.AreEqual(P.IP_Destination, IP_Destination);
            Assert.AreEqual(P.IP_Source, IP_Source);

            byte[] bytes = P.toBytes();

            //czy adres IP to domyslnie ustawiony 127.0.0.1?
            Assert.AreEqual(Package.exctractDestinationIP(bytes), IP_Destination);

            //czy adres IP to domyslnie ustawiony 127.0.0.1?
            Assert.AreEqual(Package.exctractSourceIP(bytes), IP_Source);

            //czy port wynikajacy z bajtow to ustawiony w testach port?
            Assert.AreEqual(Package.exctractPort(bytes), portNumber);
        }

        /// <summary>
        /// Testuje zamienianie pakietu na tablice bajtow
        /// </summary>
        [TestMethod]
        public void packageToByteArrayTest()
        {
            Package P = new Package();

            //Czy pakiet ma dobra dlugosc?
            Assert.AreEqual(P.toBytes().Length, 64);

            //Czy naglowek ma dobra dlugosc?
            Assert.AreEqual(24, P.headerBytes.Count);

            //Sprawdzenie dlugosci pola wiadomosci uzytkowej
            Assert.AreEqual(40, P.usableInfoBytes.Count);

        }

        /// <summary>
        /// Testuje funkcje extractPort
        /// </summary>
        [TestMethod]
        public void extractPortTest()
        {
            Package P = new Package();
            byte[] bytes = P.toBytes();

            //czy nr portu to domylnie ustawiony 1?
            Assert.AreEqual(Package.exctractPort(bytes), 1);
        }

        /// <summary>
        /// Testuje funkcje extractDestinationIP
        /// </summary>
        [TestMethod]
        public void extractDestinationIPTest()
        {
            Package P = new Package();
            byte[] bytes = P.toBytes();

            IPAddress IP = IPAddress.Parse("127.0.0.1");

            //czy adres IP to domyslnie ustawiony 127.0.0.1?
            Assert.AreEqual(Package.exctractDestinationIP(bytes), IP);
        }

        /// <summary>
        /// Testuje funkcje Package.extract()
        /// </summary>
        [TestMethod]
        public void extractSourceIPTest()
        {
            Package P = new Package();
            byte[] bytes = P.toBytes();

            IPAddress IP = IPAddress.Parse("127.0.0.1");

            //czy adres IP to domyslnie ustawiony 127.0.0.1?
            Assert.AreEqual(Package.exctractSourceIP(bytes), IP);
        }

        /// <summary>
        /// Testuje wycinanie wiadomosci z tablicy bajtow.
        /// </summary>
        [TestMethod]
        public void extractUsableMessageTest()
        {
            string inscription = "nowa, zmieniona wiadomosc";
            Package P = new Package();

            //zamien wiadomosc na tablice bajtow
            byte[] bytes = Encoding.ASCII.GetBytes(inscription);

            //zmienienie wiadomosci w postaci bajtow w pakiecie
            P.changeMessage(bytes);

            Assert.AreEqual(inscription, P.usableMessage);

            Assert.AreEqual(bytes, P.usableInfoBytes.ToArray());
        }
    }

}
