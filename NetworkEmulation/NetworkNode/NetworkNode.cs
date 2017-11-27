using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkingTools;

namespace NetworkNode
{
    /// <summary>
    /// Klasa reprezentująca węzeł sieciowy
    /// </summary>
    public class NetworkNode
    {
        /// <summary>
        /// Tablica z zajetymi pasmami
        /// </summary>
        public EONTable eonTable;

        /// <summary>
        /// Tablica komutacji - dla wszystkich rodzajow wezlow sieciowych
        /// </summary>
        public CommutationTable commutationTable;

        /// <summary>
        /// Tablica komutacji dla wezlow sieciowych na brzegu sieci. Najpierw router zaglada w pakiet
        /// i sprawdza, czy jest tam jakas czestotliwosc tunelu. Jak nie ma, to wlasnie w tej tablicy
        /// bedzie napisane, co dalej robic. Jak jest - to zwykla commutationTable.
        /// </summary>
        public BorderNodeCommutationTable borderNodeCommutationTable;

        public NetworkNode()
        {
            this.commutationTable = new CommutationTable();
            this.borderNodeCommutationTable = new BorderNodeCommutationTable();
            this.eonTable = new EONTable();
        }

        /// <summary>
        /// Wydostaje z pakietu, na jaki adres IP i nr portu w chmurze przeslac pakiet
        /// </summary>
        /// <param name="packageBytes"></param>
        /// <returns></returns>
        public Tuple<IPAddress, short> determineCloudSocketIPAndPort(byte[] packageBytes)
        {
            try
            {
                short frequency = Package.extractFrequency(packageBytes);

                //Znajdz taki rzad, dla ktorego wartosc czestotliwosci jest rowna czestotliwosci wejsciowej.
                var row = commutationTable.Table.Find(r => r.frequency_in == frequency);

                IPAddress IPCloudSocket = row.IP_OUT;
                short port = row.port_out;

                return new Tuple<IPAddress, short>(IPCloudSocket, port);
            }
            catch (Exception E)
            {
                Console.WriteLine("NetworkNode.determineCloudSocketIPAndPort(): failed to get " +
                                  "Cloud's socket IP and port. " + E.Message);
                return null;
            }
        }
    }
}
