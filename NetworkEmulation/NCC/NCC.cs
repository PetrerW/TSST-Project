using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Sockets;
using System.Net;
using NetworkingTools;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Text;
//using SharedMessageNames;

namespace NCC
{
    public class NCC
    {
        // obiekty słuzace wykorzystaniu niestatycznych metod tych klas
        private static Directory dr = new Directory();
        private static PolicyController pc = new PolicyController();
        private static Configuration conf = new Configuration();

        // lista z adresami IP i socketami interfejsów służących do słuchania
        public static Dictionary<string, string> InterfaceToListen = new Dictionary<string, string>();

        // lista z adresami IP interfejsów służących do wysyłania
        public static Dictionary<string, string> InterfaceToSend = new Dictionary<string, string>();

        //numer wpisywany przez uzytkownika okreslajacy numer domeny, wynosi 1 lub 2
        private static string number;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of domain");
            number = Console.ReadLine();

            dr.ReadingFromDirectoryFile(ConfigurationManager.AppSettings["directory" + number + "_path"]);
            pc.ReadingFromPolicyFile(ConfigurationManager.AppSettings["policy" + number + "_path"]);
            conf.ReadingFromConfigFile(ConfigurationManager.AppSettings["config" + number + "_path"]);
            Console.Clear();
            NCC new_NCC = new NCC();
            new_NCC.ListenForConnections();
            Console.ReadKey();
        }

        public NCC()
        {

        }

        /// <summary>
        /// funkca zajmująca się rozróżnianiem otrzymywanych wiadomości
        /// </summary>
        /// <param name="data"></param>
        private void ReceivedMessage(byte[] data)
        {

            string message = Encoding.ASCII.GetString(data);
            char[] delimeter = { '#' };
            string[] words = message.Split(delimeter);

            //if rozróżniający wiadomość na podstawie pierwszego wyrazu, a w przypadku CALL CONFIRMED
            // również osttaniego, który pokazuje od kogo wiadomość idzie
            if (words[0] == MessageNames.CALL_REQUEST)
            {
                ProcessCallRequest(words[1], words[2], words[3], words[4]);
            }
            else if (words[0] == MessageNames.CALL_COORDINATION)
            {
                ProcessCallCoordination(words[1], words[2], words[3], words[4]);
            }
            else if (words[0] == MessageNames.CALL_CONFIRMED && words[5] == "CPCC")
            {
                ProcessCallConfirmedFromCPCC(words[1], words[2], words[3], words[4]);
            }
            else if (words[0] == MessageNames.CALL_CONFIRMED && words[5] == "NCC")
            {
                ProcessCallConfirmedFromNCC(words[1], words[2], words[3], words[4]);
            }

        }

        /// <summary>
        /// funcjja wywolywana jesli NCC otrzymało wiadomść CALL REQUEST, procesująca tę wiadomość
        /// </summary>
        /// <param name="OriginID"></param>
        /// <param name="DestinationID"></param>
        /// <param name="demandedCapacity"></param>
        /// <param name="CPCC_IP"></param>
        public static void ProcessCallRequest(string OriginID, string DestinationID, string demandedCapacity, string CPCC_IP)
        {
            // powrót do defaultowego koloru czcionki na konsoli i wypisanie logów
            Console.ResetColor();
            Console.WriteLine("[" + Timestamp.generateTimestamp() + "]" + MessageNames.CALL_REQUEST + "from CPCC with address:" + CPCC_IP + "(ID,ID):" + OriginID + DestinationID);

            //translacja adresów odbywająca się w klasie Directory i funkcji AddressTranslation
            bool directoryaccess = dr.AddressTranslation(OriginID, DestinationID);
            // sprawdzenie dostępu w klasie PolicyController i funkcji Policy
            bool policyaccess = pc.Policy(ConfigurationManager.AppSettings["policy" + number + "_path"], OriginID);

            //pobranie wartości adresu IP tego NCC w celu przesłania go w kolejnej wiadomości
            string NCC_address;
            InterfaceToListen.TryGetValue("interface_listen", out NCC_address);

            //jeśli oba warunki spełnione to wywołujemy funkcję SendCallCoordination
            // 
            if (policyaccess == true && directoryaccess == true && dr.OneDomain == true)
            {

                SendCallIndication(dr.OriginAddress, dr.DestinationAdddress, demandedCapacity, NCC_address);
            }
            else if (policyaccess == true && directoryaccess == true)
            {
                SendCallCoordination(dr.OriginAddress, DestinationID, demandedCapacity, NCC_address);
            }
        }
        /// <summary>
        /// Funkcja przesyłająca Call Coordinationj do kolejengo NCC
        /// </summary>
        /// <param name="OriginIP"></param>
        /// <param name="DestinationID"></param>
        /// <param name="demandedCapacity"></param>
        /// <param name="NCC_address"></param>
        public static void SendCallCoordination(string OriginIP, String DestinationID, string demandedCapacity, string NCC_address)
        {

            // wiadomość która będzie wysłana do kolejengo NCC
            string message = null;
            message = MessageNames.CALL_COORDINATION + "#" + OriginIP + "#" + DestinationID + "#" + demandedCapacity + "#" + NCC_address + "#";

            byte[] messagebyte = new byte[message.Length];
            messagebyte = Encoding.ASCII.GetBytes(message);

            // pobranie adresu IP kolejnego NCC ze słownika adresów IP do wysyłania
            string ip;
            InterfaceToSend.TryGetValue("interface_to_NCC", out ip);

            // stworzenie klienta UDp i wysłanie wiadomości
            UdpClient client = new UdpClient();
            client.Send(messagebyte, messagebyte.Length, ip, Int32.Parse(ConfigurationManager.AppSettings["port"]));

        }
        /// <summary>
        /// funkcja wywoływana wtedy, gdy NCC otrzymało wiadomość CallCoordination
        /// </summary>
        /// <param name="OriginIP"></param>
        /// <param name="DestinationID"></param>
        /// <param name="demandedCapacity"></param>
        /// <param name="NCC_address"></param>
        public static void ProcessCallCoordination(string OriginIP, string DestinationID, string demandedCapacity, string NCC_address)
        {


            //sprawdzenie w Directory  i policy pojedynczego adresu docelowego 
            bool directoryaccess = dr.SingleAddressTranslation(DestinationID);
            bool policyaccess = pc.Policy(ConfigurationManager.AppSettings["policy" + number + "_path"], DestinationID);

            Console.ResetColor();
            Console.WriteLine("[" + Timestamp.generateTimestamp() + "]" + MessageNames.CALL_COORDINATION + " from NCC with address: " + NCC_address + "(IP,IP): " + OriginIP + ", " + dr.address);

            //pobranie ze słownika adresu IP do danego CPCC( sytuacja gdy tylko z pierwszej doemny wysyłamy do drugiej)
            string IP;
            InterfaceToSend.TryGetValue("interface_to_CPCC", out IP);

            if (policyaccess == true && directoryaccess == true)
            {
                SendCallIndication(OriginIP, dr.address, demandedCapacity, IP);
            }
        }

        public static void SendCallIndication(string OriginIP, string DestinationIP, string demandedCapacity, string NCC_IP)
        {
            string message = null;
            message = MessageNames.CALL_INDICATION + "#" + OriginIP + "#" + DestinationIP + "#" + demandedCapacity + "#" + NCC_IP + "#"; ;

            byte[] messagebyte = new byte[message.Length];
            messagebyte = Encoding.ASCII.GetBytes(message);

            string ip = null;

            if (DestinationIP == "127.0.0.2")
            {
                InterfaceToSend.TryGetValue("interface_to_CPCC_Franek", out ip);
            }
            else if (DestinationIP == "127.0.0.4")
            {
                InterfaceToSend.TryGetValue("interface_to_CPCC_Janek", out ip);
            }
            else if (DestinationIP == "127.0.0.6")
            {
                InterfaceToSend.TryGetValue("interface_to_CPCC", out ip);
            }

            UdpClient client = new UdpClient();
            client.Send(messagebyte, messagebyte.Length, ip, Int32.Parse(ConfigurationManager.AppSettings["port"]));
        }

        public static void ProcessCallConfirmedFromCPCC(string OriginIP, string DestinationIP, string demandedCapacity, string CPCC_IP)
        {
            Console.ResetColor();
            Console.WriteLine("[" + Timestamp.generateTimestamp() + "]" + MessageNames.CALL_CONFIRMED + " from CPCC with address: " + CPCC_IP + "(IP,IP): " + OriginIP + ", " + DestinationIP);

            string IP;
            InterfaceToSend.TryGetValue("interface_listen", out IP);
            if (CPCC_IP == "127.0.0.6")
            {
                SendCallConfirmed(OriginIP, DestinationIP, demandedCapacity, IP);
            }

        }

        public static void SendCallConfirmed(string OriginIP, string DestinationIP, string demandedCapacity, string NCC2_IP)
        {
            string message = null;
            message = MessageNames.CALL_CONFIRMED + "#" + OriginIP + "#" + DestinationIP + "#" + demandedCapacity + "#" + NCC2_IP + "#" + "NCC" + "#";

            byte[] messagebyte = new byte[message.Length];
            messagebyte = Encoding.ASCII.GetBytes(message);

            string ip;
            InterfaceToSend.TryGetValue("interface_to_NCC", out ip);

            UdpClient client = new UdpClient();
            client.Send(messagebyte, messagebyte.Length, ip, Int32.Parse(ConfigurationManager.AppSettings["port"]));
        }

        public static void ProcessCallConfirmedFromNCC(string OriginIP, string DestinationIP, string demandedCapacity, string NCC2_IP)
        {
            Console.ResetColor();
            Console.WriteLine("[" + Timestamp.generateTimestamp() + "]" + MessageNames.CALL_CONFIRMED + " from NCC with address: " + NCC2_IP + "(IP,IP): " + OriginIP + ", " + DestinationIP);

            string IP;
            InterfaceToSend.TryGetValue("interface_to_CC", out IP);

            SendConnectionRequest(OriginIP, DestinationIP,demandedCapacity, IP);

        }

        private static void SendConnectionRequest(string OriginIP, string DestinationIP, string demandedCapacity, string NCC_IP)
        {
            string message = null;
            string hops = dr.ChoosingHopsNumber(OriginIP, DestinationIP);
            message = NCC_IP + "#" + MessageNames.CONNECTION_REQUEST + "#" + "PUT" + "#" + "NCC" + "#" + OriginIP + "#" + DestinationIP + "#" + demandedCapacity + "#" + hops + "#";

            byte[] messagebyte = new byte[message.Length];
            messagebyte = Encoding.ASCII.GetBytes(message);

            string ip;
            InterfaceToSend.TryGetValue("interface_to_CC", out ip);

            UdpClient client = new UdpClient();
            client.Send(messagebyte, messagebyte.Length, ip, Int32.Parse(ConfigurationManager.AppSettings["port"]));
        }



        private void ListenForConnections()
        {
            byte[] data = new byte[64];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            string iptolisten;
            InterfaceToListen.TryGetValue("interface_listen", out iptolisten);

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(iptolisten), Int32.Parse(ConfigurationManager.AppSettings["port"]));
            UdpClient newsock = new UdpClient(ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (true)
                {
                    data = newsock.Receive(ref sender);
                    if (data.Length > 0)
                    {
                        ReceivedMessage(data);
                    }
                    data = null;
                }
            }
            catch (Exception ex)
            {

            }

        }


    }


}

