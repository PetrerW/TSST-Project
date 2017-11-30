using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkingTools;
using NetworkCalbleCloud;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading;
using System.Net.NetworkInformation;

namespace NetworkNode
{
    /// <summary>
    /// Klasa reprezentująca węzeł sieciowy
    /// </summary>
    public class NetworkNode : CableCloud
    {
        public delegate Socket SocketDelegate(Socket socket);


        public static SocketDelegate sd;


        private static NameValueCollection mySettings = System.Configuration.ConfigurationManager.AppSettings;


        private static SocketListener sl = new SocketListener();


        private static SocketSending sS = new SocketSending();


        private static CancellationTokenSource _cts = new CancellationTokenSource();


        private static List<Socket> socketListenerList = new List<Socket>();


        private static List<Socket> socketSendingList = new List<Socket>();


        private static List<Data> tmp = new List<Data>();


        private static List<string> tableFrom = new List<string>();


        private static List<string> tableTo = new List<string>();

        // private static List<List<byte[]>> listOfList = new List<List<byte[]>>();


        public static byte[] msg;


        private static short portNumber;

        private static bool Listening = true;

        private static bool Last = true;

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

        /// <summary>
        /// Pole komutacyjne wraz z buforami wejsciowym i wyjsciowymi.
        /// </summary>
        public CommutationField commutationField;

        public static object ConfigurationManager { get; private set; }

        public NetworkNode()
        {
            this.commutationTable = new CommutationTable();
            this.borderNodeCommutationTable = new BorderNodeCommutationTable();
            this.eonTable = new EONTable();
            this.commutationField = new CommutationField();
        }

        /// <summary>
        /// Wydostaje z pakietu, z jaka czestotliwoscia i nrem portu przeslac pakiet
        /// </summary>
        /// <param name="packageBytes"></param>
        /// <returns></returns>
        public Tuple<short, short> determineFrequencyAndPort(byte[] packageBytes)
        {
            try
            {
                short frequency = Package.extractFrequency(packageBytes);

                //Znajdz taki rzad, dla ktorego wartosc czestotliwosci jest rowna czestotliwosci wejsciowej.
                var row = commutationTable.Table.Find(r => r.frequency_in == frequency);

                return new Tuple<short, short>(row.frequency_out, row.port_out);
            }
            catch (Exception E)
            {
                Console.WriteLine("NetworkNode.determineCloudSocketIPAndPort(): failed to get " +
                                  "Cloud's socket IP and port. " + E.Message);
                return null;
            }
        }

        public void Run()
        {

            sd = new SocketDelegate(CallbackSocket);
            string numberOfRouter = Console.ReadLine().ToString();

            //Zmienna do przechowywania klucza na adres wychodzacy powiazany z socketem sluchaczem
            string settingsString = "";




            //pobranie wlasnosci zapisanych w pliku konfiguracyjnym
            tmp = OperationConfiguration.ReadAllSettings(mySettings);


            //przeszukanie wszystkich kluczy w liscie 
            foreach (var key in tmp)
            {

                if (key.Keysettings.StartsWith(numberOfRouter + "Sending"))
                {
                    //Uruchamiamy watek na kazdym z tworzonych sluchaczy
                    var task = Task.Run(() =>
                    {

                        CreateConnect(key.SettingsValue, key.Keysettings);

                    });



                }

                /*  //jezeli klucz zaczyna sie od TableFrom to uzupelniamy liste
                  else if (key.Keysettings.StartsWith("TableFrom"))
                      tableFrom.Add(key.SettingsValue);

                  //jezeli klucz zaczyna sie od TableTo to uzupelniamy liste
                  else if (key.Keysettings.StartsWith("TableTo"))
                      tableTo.Add(key.SettingsValue);*/
            }
            ConsoleKeyInfo cki;
            while (true)
            {
                cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Escape)
                {

                    _cts.Cancel();
                    break;
                }
            }

        }
        /// <summary>
        /// Funkcja sluży do 
        /// Zwraca socket
        /// </summary>      
        /// <param name="adresIPListener">Parametrem jest adres IP na ktorym nasluchujemy  </param>
        ///  /// <param name="key">Parametrem jest warotsc klucza wlasnosci z pliku config  </param>
        public async void CreateConnect(string addressConnectIP, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            Socket socketClient = null;
            Socket listener = null;


            try
            {
                byte[] bytes = new Byte[128];



                //Znajac dlugosc slowa "Sending" pobieram z calej nazwy klucza tylko index, ktory wykorzystam aby dopasowac do socketu IN
                ///1-Router
                ///2-Client
                ///3-NMS
                string typeOfSocket = key.Substring(8, key.Length - 8);
                string numberOfRouter = key.Substring(0, 1);

                //Sklejenie czesci wspolnej klucza dla socketu OUT oraz indeksu 
                string settingsString = numberOfRouter + "Listener" + typeOfSocket;

                IPAddress ipAddress =
                         ipAddress = IPAddress.Parse(OperationConfiguration.getSetting(settingsString, mySettings));
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP socket.  
                listener = new Socket(ipAddress.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);

                if (!listener.IsBound)
                {
                    //zabindowanie na sokecie punktu koncowego
                    listener.Bind(localEndPoint);
                    listener.Listen(100);
                }

                //Nasluchujemy bez przerwy
                while (Last)
                {

                    if (Listening)
                    {
                        //Dodanie socketu do listy socketow OUT
                        socketSendingList.Add(sS.ConnectToEndPoint(addressConnectIP));
                        //oczekiwanie na polaczenie
                        socketClient = listener.Accept();
                        //dodanie do listy sluchaczy po przez delegata
                        sd(socketClient);
                        Socket send = null;

                        Listening = false;
                        /* LingerOption myOpts = new LingerOption(true, 1);
                         socketClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, myOpts);
                         socketClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
                         socketClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);*/

                        Console.WriteLine("Polaczenie na  " + takingAddresListenerSocket(socketClient));



                        //Oczekiwanie w petli na przyjscie danych
                        while (true)
                        {
                            string from = string.Empty;
                            //Odebranie tablicy bajtow na obslugiwanym w watku sluchaczu

                            msg = sl.ProcessRecivedBytes(socketClient);
                            // Package.extractHowManyPackages(msg);
                            // listByte.Add(msg);

                            //Wykonuje jezeli nadal zestawione jest polaczenie
                            if (socketClient.Connected)
                            {
                                //Uzyskanie czestotliwosci zawartej w naglowku- potrzebna do okreslenia ktorym laczem idzie wiadomosc
                                portNumber = Package.extractPortNumber(msg);
                                from = takingAddresListenerSocket(socketClient) + " " + portNumber;
                                Package p = new Package(msg);



                                string tmp = string.Empty;
                                //wyznaczenie socketu przez ktory wyslana zostanie wiadomosc
                                if (takingAddresListenerSocket(socketClient) == "127.0.0.3" && Package.extractPortNumber(msg) == Convert.ToInt16(1))
                                {
                                    tmp = "127.0.0.12";
                                    p.changePort(Convert.ToInt16(3));

                                }
                                else if (takingAddresListenerSocket(socketClient) == "127.0.0.5" && Package.extractPortNumber(msg) == Convert.ToInt16(3))
                                {
                                    tmp = "127.0.0.10";
                                    p.changePort(Convert.ToInt16(1));

                                }
                                else if (takingAddresListenerSocket(socketClient) == "127.0.0.7" && Package.extractPortNumber(msg) == Convert.ToInt16(1))
                                {
                                    tmp = "127.0.0.8";
                                    p.changePort(Convert.ToInt16(2));
                                }
                                else
                                {
                                    tmp = "127.0.0.10";
                                    p.changePort(Convert.ToInt16(1));
                                }
                                foreach (var socket in socketSendingList)
                                {
                                    //zwracamy socket jeśli host z ktorym sie laczy spelnia warunek zgodnosci adresow z wynikiem kierowania lacza
                                    if (takingAddresSendingSocket(socket) == tmp)
                                    {
                                        send = socket;
                                    }

                                }

                                List<Queue<byte[]>> listOfQueues = await commutationField.processPackage(msg);

                                //Jak zwrocila null to jeszcze bufor nie jest pelny
                                if (listOfQueues == null)
                                    continue;
                                //w przeciwnym razie zwraca nam liste kolejek
                                else
                                {
                                    for (int i = 0; i < listOfQueues.Count; i++)
                                    {
                                        //Dopoki cos jest w podkolejce
                                        while (listOfQueues[i].Count != 0)
                                        {
                                            //Zdjecie z kolejki pakietu i wyslanie go
                                            sS.SendingPackageBytes(send, listOfQueues[i].Dequeue());
                                        }
                                    }
                                }

                                

                                msg = p.toBytes();
                                //wyslanei tablicy bajtow
                                sS.SendingPackageBytes(send, msg);
                            }
                            else
                            {
                                //Jezeli host zerwie polaczneie to usuwamy go z listy przetrzymywanych socketow, aby rozpoczac proces nowego polaczenia
                                int numberRemove = socketListenerList.IndexOf(socketClient);
                                socketListenerList.RemoveAt(numberRemove);
                                socketSendingList.RemoveAt(numberRemove);
                                break;


                            }
                        }
                        Listening = true;

                    }
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($"Socket Exception: {se}");
            }
            finally
            {
                // StopListening();
            }
            if (socketClient == null)
            {

            }


        }
    }
}
