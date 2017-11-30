using NetworkingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Collections.Specialized;
using System.IO;

namespace NewNMS
{
    public partial class Application :Form
    {
        //Zmienna, która posłuży do wysyłania wiadomości od użytkownika do chmury
        public static SocketSending sS = new SocketSending();

        // public Application _Application;

        public static CancellationTokenSource _cts = new CancellationTokenSource();

        public static Listening ls = new Listening();

        private OperationConfiguration oc = new OperationConfiguration();

        //odwoływanie się do funkcji czytających z pliku konfiguracyjnego
        public static Reading rd = new Reading();

        // socket służacy do dodawania go do listy socketów w celu dalszej komunikacji
        public Socket send;

        // socket służący do nasłuchiwania
        public Socket listener;

        //socket służący do przekazywania go do funkcji wysyłającej w zależności od wyboru do którego routera ma pójść wiadomość
        public Socket sending;

        public Application _Application;

        //lista socketów, które będą łączone w momencie przyjścia wiadomości Node is up
        public List<Socket> sends_socket;

        //lista socketów z zestawionym polaczeniem 
        public List<Socket> listening_socket = new List<Socket>();

        private object _syncRoot = new object();

        // lista zawierająca adresy IP network nodów, pochodzące z wiadomości keep alive
        public List<string> routers_IP;

        // tablica bajtów żądania przesyłanego od NMSa 
        public byte[] message_bytes, bytes_to_send;

        private static bool boolListening = true;


        //lista portów  do agentów 
        private List<string> interfaces;

        private List<string> paths;

        //wiersz tablicy komutacji do wysłania do agenta
        private List<string> CommutationTableRow;

        public List<string> list;

        private int constPort;

        private int numbersOfParameter;





        public Application()
        {
            //Ustawienie CultureInfo na en-US spowoduje wyświetlanie się wyłapanych Exceptions w języku angielskim
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            _Application = this;
            InitializeComponent();
            routers_IP = new List<string>();
            sends_socket = new List<Socket>();
            //dodawanie opcji do comboboxów
            ShowComboBox();

            readConfiguration();

        }

        


        /// <summary>
        ///  metoda zwracająca socket z listy socketów w zalezności od wybrania adresu w comboBocRouters
        /// </summary>
        /// <returns> Socket</returns>
        private Socket returnSocket()
        {
            int i;

            //pętla po wszystkich wartościach w comboBoxRouters
            for (i = 0; i < comboBoxRouters.Items.Count; i++)
            {
                if (comboBoxRouters.SelectedIndex == i)
                {
                    sending = sends_socket[i];
                }
            }
            return sending;
        }

        // tworzenie wiadomości w zalezności od zaznaczonego typu wiadomości w comboBocActions
        /// <summary>
        /// tworzenie wiadomości w zalezności od zaznaczonego typu wiadomości w comboBocActions
        /// </summary>
        /// <returns> byte[] </returns>
        private byte[] returnBytes()
        {
            
            byte[] table_in_bytes = new byte[128];
            if (this.comboBoxTables.GetItemText(this.comboBoxTables.SelectedItem) == "Commutation Table")
            {

                if (this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem) == "DELETE")
                {
                    string builder;
                    string command = this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem);
                    string freq_in = (textBoxfrequency_IN.Text).ToString();
                    string port_in = (textBoxPort_in.Text).ToString();
                    builder = command + "#" + "3" + "#" + freq_in + "#" + port_in;
                    short length = (Int16)builder.Length;
                    NMSPackage commutation_table = new NMSPackage(interfaces.ElementAt(1 - 1), builder, length);
                    table_in_bytes = commutation_table.toBytes();

                }
                if (this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem) == "ADD")
                {
                    string builder;
                    string command = this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem);
                    string freq_in = (textBoxfrequency_IN.Text).ToString();
                    string port_in = (textBoxPort_in.Text).ToString();
                    builder = command + "#" + "3" + freq_in + "#" + port_in;
                    //package = Encoding.ASCII.GetBytes(builder);
                }
            }
            if (this.comboBoxTables.GetItemText(this.comboBoxTables.SelectedItem) == "Border Node Commutation Table")
            {

                if (this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem) == "ADD")
                {
                    string builder;
                    string command = this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem);
                    string IP_IN = (textBox_IP_IN.Text).ToString();
                    string Port_IN = (textBox_Port_IN).ToString();
                    string Band = (textBoxBand).ToString();
                    string Frequency = (textBoxFrequency).ToString();
                    string Modulation = (textBoxModulation).ToString();
                    string BitRate = (textBoxBitrate).ToString();
                    string Cloud_IP = (textBoxCloud_IP).ToString();
                    string Port_OUT = (textBoxPort_OUT).ToString();
                    string Hops = (textBoxHops).ToString();

                    builder = command + "#" + "1" + "#" + IP_IN + "#" + Port_IN + "#" + Band + "#" + Frequency + "#" +
                        Modulation + "#" + BitRate + "#" + Cloud_IP + "#" + Port_OUT + "#" + Hops;
                    short length = (Int16)builder.Length;
                    NMSPackage commutation_table = new NMSPackage(interfaces.ElementAt(1 - 1), builder, length);
                    table_in_bytes = commutation_table.toBytes();

                }
                if (this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem) == "DELETE")
                {
                    string builder;
                    string command = this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem);
                    string freq_in = (textBoxfrequency_IN.Text).ToString();
                    string port_in = (textBoxPort_in.Text).ToString();
                    builder = command + "#" + "3" + freq_in + "#" + port_in;
                    //package = Encoding.ASCII.GetBytes(builder);
                }
            }

            return table_in_bytes;
        }


        /// <summary>
        /// wysłanie wiadomości do wybranego routera, po naciśnięciu klawisza, można wybrac nowy cel wiadomości i zrobic to ponownie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        /// <summary>
        /// funkcja odpalana po naciśnięciu przycisku Run
        /// </summary>
        private void ListenForConnections()
        {
            Task.Run(() =>
            {

                // petla po wszystkich wartościach portów agentów appconfig
                foreach (var address in interfaces)
                {
                    Socket socketClient = null;
                    Socket listener = null;

                    try
                    {
                        byte[] bytes = new Byte[64];

                        //taki troche zatrzask tylko jeden watek moze wykonywac ten kod w danym momencie
                      
                        

                            IPAddress ipAddress = IPAddress.Parse(address);
                            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, constPort);

                            // Create a TCP/IP socket.  
                            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            listener.Bind(localEndPoint);
                            listener.Listen(100);

                        
                        try
                        {
                            Task.Run(() =>
                            {
                                while (true)
                                {
                                    socketClient = listener.Accept();
                                    listening_socket.Add(socketClient);
                                    ReceivedMessage(socketClient);
                                }
                            });



                        }
                        catch (OperationCanceledException)
                        {

                            //UpdateListBoxSending("Exception during connecting with network node");
                        }

                    }
                    catch (SocketException se)
                    {
                        //UpdateListBoxSending("Exception during connecting with network node");
                    }

                }

            });
        }

        // funkcja dodajaca wiadomości do ListBoxa wraz z invokiem 
        public void UpdateListBoxReceived(string message)
        {
            _Application.listBoxReceived.Invoke(new Action(delegate ()
            {
                _Application.listBoxReceived.Items.Add(message);
            }));
        }
        /// <summary>
        /// funkcja dodająca statyczne elementy do comboboxów, nie zmieniające się w czasie
        /// </summary>
        private void ShowComboBox()
        {
            // elementy comboboxe'a wybierające operacje do wykonania
            comboBoxActions.Items.Add("DELETE");
            comboBoxActions.Items.Add("ADD");
            comboBoxActions.Items.Add("SHOW");

            comboBoxTables.Items.Add("Commutation Table");
            comboBoxTables.Items.Add("EON Table");
            comboBoxTables.Items.Add("Border Node Commutation Table");
        }


        /// <summary>
        /// funkcja pobierająca adres IP z socketa 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static string getIPAddressRemote(Socket socket)
        {
            //string address;
            IPEndPoint ipend = socket.RemoteEndPoint as IPEndPoint;

            return ipend.Address.ToString();
        }

        /// <summary>
        /// funkcja obsługująca przychodzącą wiadomość
        /// </summary>
        /// <param name="listener"></param>
        private void ReceivedMessage(Socket listener)
        {
            string usableMessage = "";
         
                while (true)
                {
                    // wyczyszczenie paczki przed jej wysłaniem oraz przed ponownym wysyłaniem 
                    usableMessage = string.Empty;

                    //przypisywanie do paczki wiadomości przesłanej przez klienta, w tym przypadku przez agenta
                    byte[] nmspackage = new byte[64];

                    // tylko jeden wątek moze wykonywac ten kod w danym czasie
                 //  lock (_syncRoot)
                 //   {
                        nmspackage = ls.ProcessRecivedByteMessage(listener);
                 //   }
                    // wykonuje się tylko wtedy jeśli agent nie  jest podłączony
                    if (!listener.Connected)
                    {
                        // pobranie indeksu socketu z listy soecketów
                        int index = listening_socket.IndexOf(listener);
                        UpdateListBoxReceived("Network Node" + getIPAddressRemote(sends_socket.ElementAt(index)) + " is disconnected");
                        // rozłączanie obu socketów
                        listener.Disconnect(true);
                        //send.Disconnect(true);

                        //usuwanie socketów, adresów z list by przy ponownym połączeniu dodawać je na ponownie
                        listening_socket.RemoveAt(index);
                        sends_socket.RemoveAt(index);
                        routers_IP.RemoveAt(index);

                        // wyświetlanie pobranego adresu IP z list podłączonych agentów
                       /* _Application.comboBoxNode1IP.Invoke(new Action(delegate ()
                        {
                           // comboBoxNode1IP.Items.RemoveAt(index);
                           // comboBoxNode2IP.Items.RemoveAt(index);
                            comboBoxRouters.Items.RemoveAt(index);

                        }));*/
                        break;
                    }
                    // wykonuje się tylko wtedy jeśli agent jest podłączony
                    else
                    {
                        string sourceip;
                        //tylko jesli paczka nie jest nullem
                        if (nmspackage != null)
                        {
                            usableMessage = NMSPackage.extractUsableMessage(nmspackage, NMSPackage.extractUsableInfoLength(nmspackage));
                            //tylko w przypadku pierwszej wiadomości od danego agenta
                            if (usableMessage == "Network node is up")
                            {
                                sourceip = NMSPackage.exctractSourceIP(nmspackage).ToString();
                                //jesli lista z adresami IP routerów nie zawiera danego IP to je dodaje a następnie wyśwuietlam komunikat 
                                if (!routers_IP.Contains(sourceip))
                                {
                                    routers_IP.Add(sourceip);
                                    UpdateListBoxReceived("Network Node: " + sourceip + " is up");
                                    UpdateListBoxReceived(listening_socket.Count.ToString());
                                    
                                }
                                //tworze połączenie z socketem routera, który wysłał do mnie wiadomość
                                send = sS.ConnectToEndPoint(NMSPackage.exctractSourceIP(nmspackage).ToString());
                            // a następnie dodaje ten socket do listy socketów, by potem móc z nich korzystać
                            sends_socket.Add(send);

                            short numberOfRouter = NMSPackage.extractNumberOfRouterNumber(nmspackage);

                           List<string> configurationRouter= ReadingFromFile(paths.ElementAt(numberOfRouter - 1));
                            foreach (var line in configurationRouter)
                            {
                                short length = (Int16)line.Length;
                                NMSPackage tablePackage = new NMSPackage(interfaces.ElementAt(numberOfRouter-1), line, length);
                                byte[] tablePackageInBytes = tablePackage.toBytes();
                                send.Send(tablePackageInBytes);
                                Task.Delay(10);
                            }

                               

                                /* z kazdą wiadomością "Network node is up" dodaje IP routera do checkboca w celu mozliwości wybrania
                                 docelwoego punktu komunikacji */
                                _Application.comboBoxRouters.Invoke(new Action(delegate ()
                                {
                                    //_Application.comboBoxNode1IP.Items.Add(sourceip);
                                   // _Application.comboBoxNode2IP.Items.Add(sourceip);
                                    _Application.comboBoxRouters.Items.Add(sourceip);

                                }));

                            }
                            //jesli wiadmośc keep alive
                            else if (usableMessage == "Keep Alive")
                            {
                                //UpdateListBoxReceived(usableMessage);

                            }
                        }
                        //jesli paczka jest nullem
                        else
                        {
                            int index = listening_socket.IndexOf(listener);
                            // stwierdzam, że agent nie odpowiada, a potem go rozłączam
                            UpdateListBoxReceived("Network Node" + getIPAddressRemote(sends_socket.ElementAt(index)) + " is not responding");
                            UpdateListBoxReceived("Network Node" + getIPAddressRemote(sends_socket.ElementAt(index)) + " is disconnected");

                            listening_socket.RemoveAt(index);
                            sends_socket.RemoveAt(index);
                            routers_IP.RemoveAt(index);
                           /* _Application.comboBoxNode1IP.Invoke(new Action(delegate ()
                            {
                                //comboBoxNode1IP.Items.RemoveAt(index);
                                //comboBoxNode2IP.Items.RemoveAt(index);
                                comboBoxRouters.Items.RemoveAt(index);

                            }));*/
                            //odłączanie
                            listener.Disconnect(true);
                            //send.Disconnect(true);
                            //usuwanie socketów, adresów z list by przy ponownym połączeniu dodawać je na ponownie

                            break;

                        }

                    }

                }
           
        }

        private void buttonListen_Click_1(object sender, EventArgs e)
        {
            _Application.listBoxReceived.Items.Add("Running");
            ListenForConnections();
            
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            //SendingMessages.SendingPackageBytes(returnSocket(), returnBytes());
            Socket socket = returnSocket();
            byte[] bytes = returnBytes();
            string bajty = Encoding.ASCII.GetString(bytes);
            socket.Send(bytes);
            UpdateListBoxReceived(bajty);
        }

        private void readConfiguration()
        {
            List<Data> data = new List<Data>();
            interfaces = new List<string>();
            paths = new List<string>();
            NameValueCollection readSettings = ConfigurationManager.AppSettings;
            data = OperationConfiguration.ReadAllSettings(readSettings);
            foreach (var key in data)
            {
                if (key.Keysettings.StartsWith("interface"))
                {
                    interfaces.Add((key.SettingsValue));
                }
                else if(key.Keysettings.StartsWith("port"))
                {
                    constPort = Int32.Parse(key.SettingsValue);
                }
                else if (key.Keysettings.StartsWith("number"))
                {
                    numbersOfParameter = Int32.Parse(key.SettingsValue);
                }
                else if (key.Keysettings.StartsWith("path"))
                {
                    paths.Add((key.SettingsValue));
                }
            }

        }

       
            
        

        private void SendingConnection(Socket socket, string path)
        {
           //string line = ReadingFromFile(path);



        }

        public List<string> ReadingFromFile(string path)
        {
            List<string> listLine= new List<string>();
            string line;
            try
            {
                using (StreamReader file = new StreamReader(path))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        //takeParameterFromFile(line);
                        listLine.Add(line);
                       // MessageBox.Show(line);
                    }
                }

               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return listLine;

        }

        private void comboBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {

            if(comboBoxTables.SelectedItem== "Commutation Table")
            {



            }else if(comboBoxTables.SelectedItem=="EON Table")
            {

            }else if(comboBoxTables.SelectedItem== "Border Node Commutation Table")
            {

            }

                comboBoxTables.Items.Add("Commutation Table");
            comboBoxTables.Items.Add("EON Table");
            comboBoxTables.Items.Add("Border Node Commutation Table");
        }

        public void takeParameterFromFile(string line)
        {
            string a, b, c, d;

            Char delimiter = '#';
            String[] parameters = line.Split(delimiter);
            if (parameters.Length == numbersOfParameter)
                foreach (var substring in parameters)
                {
                    Console.WriteLine(substring);
                }
        }

        
    }
}







