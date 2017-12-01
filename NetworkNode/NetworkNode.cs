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
using System.Diagnostics;

namespace NetworkNode
{
    /// <summary>
    /// Klasa reprezentująca węzeł sieciowy
    /// </summary>
    public class NetworkNode : CableCloud
    {
        public delegate Socket SocketDelegate(Socket socket);


        public static SocketDelegate sd;


        private NameValueCollection mySettings = System.Configuration.ConfigurationManager.AppSettings;


        private SocketListener sl = new SocketListener();


        private SocketSending sS = new SocketSending();


        private static CancellationTokenSource _cts = new CancellationTokenSource();


        private static List<Socket> socketListenerList = new List<Socket>();


        private static List<Socket> socketSendingList = new List<Socket>();


        private static List<Data> tmp = new List<Data>();

        // private static List<List<byte[]>> listOfList = new List<List<byte[]>>();


        public static byte[] msg;


        private static short portNumber;

        private static bool Listening = true;

        private static bool Last = true;

        private string numberOfRouter;

        public string NumberOfRouter { get { return numberOfRouter; } }

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

        private OperationConfiguration oc = new OperationConfiguration();

        public static object ConfigurationManager { get; private set; }

        public volatile bool canIClearMyBuffers = false;

        public volatile bool zeroTimer = false;

        public NetworkNode()
        {
            this.commutationTable = new CommutationTable();
            this.borderNodeCommutationTable = new BorderNodeCommutationTable();
            this.eonTable = new EONTable();
            this.commutationField = new CommutationField(ref borderNodeCommutationTable, ref commutationTable, ref eonTable, 1);

        }

        /// <summary>
        /// Wydostaje z pakietu, na jaki adres IP i nr portu w chmurze przeslac pakiet
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

            // sd = new SocketDelegate(CallbackSocket);

            Console.WriteLine("ID router:");
            numberOfRouter = Console.ReadLine().ToString();



            ManagmentAgent agent = new ManagmentAgent();
            Task.Run(() => agent.Run(numberOfRouter, ref commutationTable, ref borderNodeCommutationTable, ref eonTable));

            //pobranie wlasnosci zapisanych w pliku konfiguracyjnym
            tmp = OperationConfiguration.ReadAllSettings(mySettings);


            //przeszukanie wszystkich kluczy w liscie 
            foreach (var key in tmp)
            {

                if (key.Keysettings.StartsWith(numberOfRouter + "Sending"))
                {
                    //Uruchamiamy watek na kazdym z tworzonych sluchaczy
                    //Task.Run(()=>
                    CreateConnect(key.SettingsValue, key.Keysettings);//);

                }
            }
            /*  //jezeli klucz zaczyna sie od TableFrom to uzupelniamy liste
              else if (key.Keysettings.StartsWith("TableFrom"))
                  tableFrom.Add(key.SettingsValue);

              //jezeli klucz zaczyna sie od TableTo to uzupelniamy liste
              else if (key.Keysettings.StartsWith("TableTo"))
                  tableTo.Add(key.SettingsValue);*/
            ConsoleKeyInfo cki;

            //Petla wykonuje sie poki nie nacisniemy klawisza "esc"
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


        /// </summary>      
        /// <param name="adresIPListener">Parametrem jest adres IP na ktorym nasluchujemy  </param>
        ///  /// <param name="key">Parametrem jest warotsc klucza wlasnosci z pliku config  </param>
        public void CreateConnect(string addressConnectIP, string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            Socket socketClient = null;
            Socket listener = null;


            try
            {
                byte[] bytes = new Byte[128];


                Task.Run(async () =>
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    while (true)
                    {
                        if (zeroTimer == true)
                        {
                            sw = Stopwatch.StartNew();
                            canIClearMyBuffers = false;
                            zeroTimer = false;
                        }

                        var wait = await Task.Run(async () =>
                        {
                            await Task.Delay(500);
                            sw.Stop();
                            return sw.ElapsedMilliseconds;
                        });

                        canIClearMyBuffers = true;

                    }
                });

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
                        socketListenerList.Add(socketClient);
                        Socket send = null;

                        Listening = false;

                        string tmp = string.Empty;
                        //wyznaczenie socketu przez ktory wyslana zostanie wiadomosc
                        if (numberOfRouter == "1")
                        {
                            tmp = "127.0.0.12";

                        }
                        else if (numberOfRouter == "2")
                        {
                            tmp = "127.0.0.10";

                        }
                        else if (numberOfRouter == "3")
                        {
                            tmp = "127.0.0.8";

                        }

                        //Oczekiwanie w petli na przyjscie danych
                        while (true)
                        {
                            byte[] msg;
                            string from = string.Empty;
                            //Odebranie tablicy bajtow na obslugiwanym w watku sluchaczu

                            msg = sl.ProcessRecivedBytes(socketClient);
                            stateReceivedMessageFromCableCloud(msg, socketClient);

                            // Package.extractHowManyPackages(msg);
                            // listByte.Add(msg);

                            //Wykonuje jezeli nadal zestawione jest polaczenie
                            if (socketClient.Connected)
                            {
                                //Uzyskanie czestotliwosci zawartej w naglowku- potrzebna do okreslenia ktorym laczem idzie wiadomosc
                                portNumber = Package.extractPortNumber(msg);
                                from = takingAddresListenerSocket(socketClient) + " " + portNumber;
                                Package p = new Package(msg);

                                foreach (var socket in socketSendingList)
                                {
                                    //zwracamy socket jeśli host z ktorym sie laczy spelnia warunek zgodnosci adresow z wynikiem kierowania lacza
                                    if (takingAddresSendingSocket(socket) == tmp)
                                    {
                                        send = socket;
                                    }

                                }

                                List<Queue<byte[]>> listOfQueue = new List<Queue<byte[]>>();
                                
                                List<Queue<byte[]>> listOfQueues = commutationField.processPackage(msg);

                                //Jak zwrocila null to jeszcze bufor nie jest pelny
                                if (listOfQueues == null)
                                {
                                    /* if (canIClearMyBuffers == true)
                                     {
                                         byte[] tempBufferIn;
                                         for (int z = 0; z < commutationField.bufferIn.queue.Count; z++)
                                         {
                                             while (commutationField.bufferIn.queue.Count > 0)
                                             {
                                                 tempBufferIn = commutationField.bufferIn.queue.Dequeue();
                                                 byte[] obytes = commutationField.borderNodeCommutationTable.changePackageHeader2(tempBufferIn, ref commutationField);

                                                 stateSendingMessageToCableCloud(obytes, send);
                                                 sS.SendingPackageBytes(send, obytes);
                                             }
                                         }
                                         canIClearMyBuffers = false;
                                         zeroTimer = true;

                                     }*/
                                    continue;
                                }//w przeciwnym razie zwraca nam liste kolejek
                                else
                                {
                                    for (int i = 0; i < listOfQueues.Count; i++)
                                    {
                                        //Dopoki cos jest w podkolejce
                                        while (listOfQueues[i].Count != 0)
                                        {
                                            //Element z listy kolejek moze byc nullem
                                            if (listOfQueues[i].Count != 0)
                                            {

                                                stateSendingMessageToCableCloud(listOfQueues[i].Peek(), send);
                                                //Zdjecie z kolejki pakietu i wyslanie go
                                                sS.SendingPackageBytes(send, listOfQueues[i].Dequeue());

                                            }
                                            //A jak jest nullem to nic nie robimy
                                        }
                                    }
                                }
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
        public NameValueCollection getAppSetting { get { return mySettings; } }


        public static void stateReceivedMessageFromCableCloud(byte[] bytes, Socket socket)
        {
            if (bytes != null)
            {
                int length = Package.extractUsableMessage(bytes, Package.extractUsableInfoLength(bytes)).Length;
                short numberOfLink = Package.extractPortNumber(bytes);
                short ID = Package.extractID(bytes);
                short messageNumber = Package.extractPackageNumber(bytes);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[" + DateTime.Now + "] Message about ID: {0,5} and number of package {1,2} / " + Package.extractHowManyPackages(bytes) + " received on port: " + numberOfLink, ID, messageNumber);
            }
        }


        public static void stateSendingMessageToCableCloud(byte[] bytes, Socket socket)
        {
            if (socket != null)
            {
                int length = Package.extractUsableMessage(bytes, Package.extractUsableInfoLength(bytes)).Length;
                short numberOfLink = Package.extractPortNumber(bytes);
                short ID = Package.extractID(bytes);
                short messageNumber = Package.extractPackageNumber(bytes);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("[" + DateTime.Now + "] Message about ID: {0,5} and number of package {1,2} / " + Package.extractHowManyPackages(bytes) + " sent on port: " + numberOfLink, ID, messageNumber);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("CableCloud is not responding - link is not available");
                Console.ResetColor();
            }

        }
    }
}
