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

namespace NewNMS
{
    public partial class Application : Form
    {
        //Zmienna, która posłuży do wysyłania wiadomości od użytkownika do chmury
        public static SocketSending sS = new SocketSending();

        // public Application _Application;

        public static CancellationTokenSource _cts = new CancellationTokenSource();

        public static Listening ls = new Listening();

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
        private List<int> portslist;

        //wiersz tablicy komutacji do wysłania do agenta
        private List<string> CommutationTableRow;


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
            NMSPackage nmspackage;
            byte[] bytenmspackage = new byte[1024];

            if (this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem) == "CREATE LINK")
            {

                string usablemessage = this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem);
                string IP_out = this.comboBoxNode2IP.GetItemText(this.comboBoxActions.SelectedItem);
                int freq_in = Int32.Parse(textBoxfrequency.Text);
                short port_out = Int16.Parse(textBoxPort_out.Text);
                nmspackage = new NMSPackage(freq_in, IP_out, (short)port_out,  usablemessage, (short)usablemessage.Length);
                bytenmspackage = nmspackage.toBytes();
            }
            else
            {
                
            }
            //string message = this.comboBoxActions.GetItemText(this.comboBoxActions.SelectedItem);
            //short length = (Int16)message.Length;
           // NMSPackage nmspackage = new NMSPackage(ConfigurationManager.AppSettings["NMS_IP"], message, length);
           

            return bytenmspackage;
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
                foreach (var port in portslist)
                {
                    Socket socketClient = null;
                    Socket listener = null;

                    try
                    {
                        byte[] bytes = new Byte[64];

                        //taki troche zatrzask tylko jeden watek moze wykonywac ten kod w danym momencie
                        lock (_syncRoot)
                        {

                            IPAddress ipAddress = IPAddress.Parse(ConfigurationManager.AppSettings["NMS_IP"]);
                            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

                            // Create a TCP/IP socket.  
                            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            listener.Bind(localEndPoint);
                            listener.Listen(100);

                        }
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
            // elementy comboboxe'a służaące do zidentyfikowania połączenia(lambdy)
            comboBoxLambdas.Items.Add("uraaa");
            comboBoxLambdas.Items.Add("jghdthg");

            // elementy comboboxe'a wybierające operacje do wykonania
            comboBoxActions.Items.Add("DELETE LINK");
            comboBoxActions.Items.Add("ADD LINK");
            comboBoxActions.Items.Add("SHOW LINK");
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
            try
            {
                while (true)
                {
                    // wyczyszczenie paczki przed jej wysłaniem oraz przed ponownym wysyłaniem 
                    usableMessage = string.Empty;

                    //przypisywanie do paczki wiadomości przesłanej przez klienta, w tym przypadku przez agenta
                    byte[] nmspackage = new byte[64];

                    // tylko jeden wątek moze wykonywac ten kod w danym czasie
                    lock (_syncRoot)
                    {
                        nmspackage = ls.ProcessRecivedByteMessage(listener);
                    }
                    // wykonuje się tylko wtedy jeśli agent nie  jest podłączony
                    if (!listener.Connected)
                    {
                        // pobranie indeksu socketu z listy soecketów
                        int index = listening_socket.IndexOf(listener);
                        UpdateListBoxReceived("Network Node" + getIPAddressRemote(sends_socket.ElementAt(index)) + " is disconnected");
                        // rozłączanie obu socketów
                        listener.Disconnect(true);
                        send.Disconnect(true);

                        //usuwanie socketów, adresów z list by przy ponownym połączeniu dodawać je na ponownie
                        listening_socket.RemoveAt(index);
                        sends_socket.RemoveAt(index);
                        routers_IP.RemoveAt(index);

                        // wyświetlanie pobranego adresu IP z list podłączonych agentów
                        _Application.comboBoxNode1IP.Invoke(new Action(delegate ()
                        {
                            comboBoxNode1IP.Items.RemoveAt(index);
                            comboBoxNode2IP.Items.RemoveAt(index);
                            comboBoxRouters.Items.RemoveAt(index);

                        }));
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

                                /* z kazdą wiadomością "Network node is up" dodaje IP routera do checkboca w celu mozliwości wybrania
                                 docelwoego punktu komunikacji */
                                _Application.comboBoxNode1IP.Invoke(new Action(delegate ()
                                {
                                    _Application.comboBoxNode1IP.Items.Add(sourceip);
                                    _Application.comboBoxNode2IP.Items.Add(sourceip);
                                    _Application.comboBoxRouters.Items.Add(sourceip);

                                }));

                            }
                            //jesli wiadmośc keep alive
                            else if (usableMessage == "Keep Alive")
                            {
                                UpdateListBoxReceived(usableMessage);

                            }
                        }
                        //jesli paczka jest nullem
                        else
                        {
                            int index = listening_socket.IndexOf(listener);
                            // stwierdzam, że agent nie odpowiada, a potem go rozłączam
                            UpdateListBoxReceived("Network Node" + getIPAddressRemote(sends_socket.ElementAt(index)) + " is not responding");
                            UpdateListBoxReceived("Network Node" + getIPAddressRemote(sends_socket.ElementAt(index)) + " is disconnected");

                            //odłączanie
                            listener.Disconnect(true);
                            send.Disconnect(true);
                            //usuwanie socketów, adresów z list by przy ponownym połączeniu dodawać je na ponownie
                            listening_socket.RemoveAt(index);
                            sends_socket.RemoveAt(index);
                            routers_IP.RemoveAt(index);
                            _Application.comboBoxNode1IP.Invoke(new Action(delegate ()
                            {
                                comboBoxNode1IP.Items.RemoveAt(index);
                                comboBoxNode2IP.Items.RemoveAt(index);
                                comboBoxRouters.Items.RemoveAt(index);

                            }));
                            break;

                        }



                    }

                }
            }
            catch (SocketException)
            {

            }
            catch (Exception)
            {

            }
        }

        private void buttonListen_Click_1(object sender, EventArgs e)
        {
            ListenForConnections();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            //SendingMessages.SendingPackageBytes(returnSocket(), returnBytes());
            Socket socket = returnSocket();
             byte[] bytes = returnBytes();
             socket.Send(bytes);
        }

        private void readConfiguration()
        {
            List<Data> data = new List<Data>();
            portslist = new List<int>();
            NameValueCollection readSettings = ConfigurationManager.AppSettings;
            data = OperationConfiguration.ReadAllSettings(readSettings);
            foreach (var key in data)
            {
                if (key.Keysettings.StartsWith("router"))
                {
                    portslist.Add(Int32.Parse(key.SettingsValue));
                }
            }

        }

        
           private byte[] connectionsTable()
           {
            byte[] conntable = new byte[1024];

            while ()
               {
                   
                   CommutationTableRow = new List<string>();
                   List<Data> data = new List<Data>();
                   NameValueCollection readSettings = ConfigurationManager.AppSettings;
                   data = OperationConfiguration.ReadAllSettings(readSettings);
                   foreach(var key in data)
                   {
                       if(key.Keysettings.StartsWith("frequency"))
                       {
                           while (key.Keysettings.StartsWith("frequency"))
                           {
                               CommutationTableRow.Add(((key.SettingsValue).ToString()));
                           }
                       }
                   }

               }



               return conntable;
           }



    }
}







