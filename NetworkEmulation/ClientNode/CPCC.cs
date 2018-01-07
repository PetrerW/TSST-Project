using NetworkingTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using SharedMessageNames;

namespace ClientNode
{

    /// <summary>
    /// Klasa, która będzię wykorzystywana do połączenia z NCC w sieci operatorskiej. Zawierać będzie funkcje niezbędne do inicjacji zestawiania połączenia na trasie między klientami.
    /// </summary>
    public class CPCC
    {
        public delegate Socket SocketDelegate(Socket socket);

        public static SocketDelegate sd;

        //Zmienna, która posłuży do nasłuchiwania nadchodzących wiadomości od strony chmury
        public static SocketListener sl = new SocketListener();

        //Zmienna, która posłuży do wysyłania wiadomości od użytkownika do chmury
        public static SocketSending sS = new SocketSending();

        //Token, który posłuży do zamknięcia sesji
        public static CancellationTokenSource _cts = new CancellationTokenSource();

        //Obiekt klasy StringBuidler służy do tworzenia zawartosci zmiennych typu string - zawiera szereg funkcji, które to ułatwiają.
        public StringBuilder sb;

        //Obiekt klasy StreamReader służy do oczytu linii ze standardowego pliku tekstowego. 
        public StreamReader sr;

        //Zmienna służąca do odczytu SqlStringConnection z pliku.
        public string line;

        //Zmienna służąca do przechowania informacji zawartych w konfigu
        public List<string> CPCCConfig;

        //Lista przechowujaca IP i porty NCC
        public List<string> NCCConnectionInfo;

        //IP na którym łączymy się z NCC;
        public string NCCip;

        //Port na którym łaczymy się z NCC;
        public string NCCPort;

        //Gniazdo obsługujące wysyłanie wiadomosci przez połączenie TCP
        public Socket send;

        //Gniazdo obsługujące odbieranie wiadomości przez połączenie TCP
        public Socket socket;

        //Task, na nim uruchomione będzie wysyłanie wiadomości
        Task t;

        //Zmienna przechowująca IP na którym będzie nasłuchiwał CPCC
        public static string CPCCListenerIP;

        //Zmienna przechowująca IP aplikacji klienckiej, z którą połączony jest moduł CPCC
        public static string ClientIP;

        //Zmienna pozwalająca na dostęp do funkcji/obiektów/zmiennych w obiekcie WindowsForm związanym z aplikacją kliencką
        public static ClientApplication _clientApplication;

        //Zmienna przechowujaca nazwę klienta
        public static string ClientName;

        //Zmienna pozwalajaca na odwołania do funkcji które nie są statyczne
        public CPCC _cpcc;

        //Konstruktor obiektu klasy CPCC
        public CPCC(string OurClientIP, string name, ref ClientApplication ap)
        {
            ClientIP = OurClientIP;
            _clientApplication = ap;
            ClientName = name;
            _cpcc = this;
        }

        /// <summary>
        /// Funkcja odpowiedzialna za uruchomienie CPCC w momencie startu klienta (zawiera np. odczytanie wartości IP NCC z pliku konfiguracyjnego)
        /// </summary>
        public void runCPCC()
        {
            try
            {
                CPCCConfig = new List<string>();
                NCCConnectionInfo = new List<string>();

                using (sr = new StreamReader("CPCCconfig.txt"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        CPCCConfig.Add(line);
                    }
                }

                for (int i = 0; i < CPCCConfig.Count(); i++)
                {
                    string tmp = CPCCConfig[i];

                    //Znaki oddzielające poszczególne części żądania klienta.
                    char[] delimiterChars = { '#' };
                    //Podzielenie żądania na tablicę stringów.
                    string[] words = tmp.Split(delimiterChars);
                    //Dodanie do listy odpowiednich wpisów - klucz/wartość
                    NCCConnectionInfo.Add(words[0]);
                    NCCConnectionInfo.Add(words[1]);
                }

                if (ClientIP == "127.0.0.2")
                {
                    NCCip = NCCConnectionInfo[NCCConnectionInfo.IndexOf("127.0.0.40")];
                    NCCPort = NCCConnectionInfo[NCCConnectionInfo.IndexOf("127.0.0.40") + 1];
                    CPCCListenerIP = "127.0.0.53";
                }
                else if (ClientIP == "127.0.0.4")
                {
                    NCCip = NCCConnectionInfo[NCCConnectionInfo.IndexOf("127.0.0.40")];
                    NCCPort = NCCConnectionInfo[NCCConnectionInfo.IndexOf("127.0.0.40") + 1];
                    CPCCListenerIP = "127.0.0.54";
                }
                else
                {
                    NCCip = NCCConnectionInfo[NCCConnectionInfo.IndexOf("127.0.0.50")];
                    NCCPort = NCCConnectionInfo[NCCConnectionInfo.IndexOf("127.0.0.50") + 1];
                    CPCCListenerIP = "127.0.0.55";
                }

                Listen(CPCCListenerIP, 11000);

            }
            catch (Exception err)
            {
                updateCPCClogs(err.ToString());
                updateCPCClogs("\r\n");
                //TRZEBA TU RZUCIĆ JAKĄŚ ZAWARTOŚĆ
            }
        }


        private void Listen(string CPCC_IP, int port)
        {
            Task.Run(() =>
            {
                byte[] data = new byte[64];
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(CPCC_IP), port);
                UdpClient newsock = new UdpClient(ipep);

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    while (true)
                    {
                        data = newsock.Receive(ref sender);
                        if (data.Length > 0)
                        {
                            string timestamp = Timestamp.generateTimestamp();
                            // updateCPCClogs("[" + timestamp + "]" + " received message ");
                            _cpcc.processMessageFromNCC(timestamp, data);
                        }
                        data = null;
                    }
                }
                catch (Exception ex)
                {
                    updateCPCClogs(ex.ToString());
                    updateCPCClogs("\r\n");
                }
            });
        }


        //Funkcja odpowiedzialna za wysłanie wiadomości po naciśnięciu przycisku - wysyła CallRequest
        public void sendCallRequest(string senderid, string receiverid, string demandedCapacity)
        {
            try
            {
                string message = null;
                message = MessageNames.CALL_REQUEST + "#" + senderid + "#" + receiverid + "#" + demandedCapacity + "#" + ClientIP + '#' ;
                //Stworzenie znacznika czasowego
                string timestamp = Timestamp.generateTimestamp();

                byte[] messagebyte = new byte[message.Length];
                messagebyte = Encoding.ASCII.GetBytes(message);
                //Wysłanie wiadomości (tablicy bajtów) za pośrednictwem gniazda

                //Wyslłanie wiadomości UDP
                UdpClient client = new UdpClient();
                client.Send(messagebyte, messagebyte.Length, NCCip, 11000);

                //Zaktualizowanie LogEventTextBoxa
                updateCPCClogs("[" + timestamp + "]" + " Sending UDP message: " + message);
                updateCPCClogs("\r\n");
            }
            catch (Exception err)
            {
                updateCPCClogs(err.ToString());
                updateCPCClogs("\r\n");
            }

        }

        public void sendCallConfirmed(string senderip, string receiverip, string demandedCapacity)
        {
            try
            {
                string message = null;
                message = MessageNames.CALL_CONFIRMED + "#" + senderip + "#" + receiverip + "#" + demandedCapacity + "#" + ClientIP + "#" + "CPCC" + "#"; ;
                //Stworzenie znacznika czasowego
                string timestamp = Timestamp.generateTimestamp();

                byte[] messagebyte = new byte[message.Length];
                messagebyte = Encoding.ASCII.GetBytes(message);

                UdpClient client = new UdpClient();
                client.Send(messagebyte, messagebyte.Length, NCCip, 11000);

                //Zaktualizowanie LogEventTextBoxa
                updateCPCClogs("[" + timestamp + "]" + " Sending UPD message: " + message);
                updateCPCClogs("\r\n");
            }
            catch (Exception err)
            {
                updateCPCClogs(err.ToString());
                updateCPCClogs("\r\n");
            }
        }

        //Funkcja obrabiająca przychodzącye wiadomości ze strony NCC - otrzymane mogą być wiadomości -> CallIndication (odpowiedzią na to 
        //będzie wysłanie CallAccept) lub wiadomość zwrotna od NCC o powodzeniu w zestawianiu połączenia na trasie między klientami
        public void processMessageFromNCC(string timestamp, byte[] messagebyte)
        {

            //Znaki oddzielające poszczególne części żądania klienta.
            char[] delimiterChars = { '#' };

            string message = Encoding.ASCII.GetString(messagebyte);
            //Podzielenie żądania na tablicę stringów.
            string[] words = message.Split(delimiterChars);
            //lista zawierająca kolejne wyrazy z otrzymanego ciagu
            List<string> messageParts = new List<string>();

            string temporarymessage;

            foreach (string element in words)

            {
                messageParts.Add(element);
            }

            updateCPCClogs(message);
            string cr = MessageNames.CONNECTION_REQUEST;
            switch (messageParts[0])
            {
                /*
                To nie będzie potrzebne, bo CallRequest tylko sie wysyła, ale nie można go otrzymać


                case "CALLREQUEST":
                    temporarymessage = null;
                    temporarymessage = "[" + timestamp + "]" + " Connection requested from: " + messageParts[1];
                    updateCPCClogs(temporarymessage);
                    updateCPCClogs("\r\n");
                    _cpcc.sendCallConfirmed(words[1], words[2], words[3]);
                    timestamp = Timestamp.generateTimestamp();
                    temporarymessage = "[" + timestamp + "]" + " Sending message Call Confirmed.";
                    updateCPCClogs(temporarymessage);
                    updateCPCClogs("\r\n");
                    break;*/
                case "CALLCONFIRMED":
                    temporarymessage = null;
                    temporarymessage = "[" + timestamp + "]" + " Requested connection with: " + messageParts[2] + " is established";
                    updateCPCClogs(temporarymessage);
                    updateCPCClogs("\r\n");
                    break;
                case "CALLINDICATION":
                    temporarymessage = null;
                    temporarymessage = "[" + timestamp + "]" + " Connection requested from: " + messageParts[1];
                    updateCPCClogs(temporarymessage);
                    updateCPCClogs("\r\n");
                    _cpcc.sendCallConfirmed(words[1], words[2], words[3]);
                    timestamp = Timestamp.generateTimestamp();
                    temporarymessage = "[" + timestamp + "]" + " Sending message Call Confirmed.";
                    updateCPCClogs(temporarymessage);
                    updateCPCClogs("\r\n");
                    break;
                default:
                    break;

            }
        }

        //Otrzymanie lub wysłanie wiadomości skutkuje aktualizacją logów, np w osobnej części aplikacji klienckiej               
        public static void updateCPCClogs(string message)
        {
            _clientApplication.updateCPCCLog(message);
        }

    }
}
