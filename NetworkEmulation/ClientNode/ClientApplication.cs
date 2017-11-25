using NetworkingTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ClientNode
{
    /// <summary>
    /// Główna klasa związana z obsługą aplikacji klienckiej. Odpalona zostaje po zakończeniu działania okienka klasy: <class>ClientNode.StartClientApplication.cs</class>.
    /// </summary>
    public partial class ClientApplication : Form
    {
        //Zmienna, która posłuży do nasłuchiwania nadchodzących wiadomości od strony chmury
        public static SocketListener sl = new SocketListener();

        //Zmienna, która posłuży do wysyłania wiadomości od użytkownika do chmury
        public static SocketSending sS = new SocketSending();

        //Token, który posłuży do zamknięcia sesji
        public static CancellationTokenSource _cts = new CancellationTokenSource();

        //Obiekt klasy ClientApplication, posłuży do odwołań do różnych pól klasy (głównie obsługa elementów związanych z WindowsForm)
        public ClientApplication _ClientApplication;

        //Gniazdo obsługujące wysyłanie wiadomosci przez połączenie TCP
        public Socket send;

        //Gniazdo obsługujące odbieranie wiadomości przez połączenie TCP
        public Socket socket;

        //Zmienna informująca o tym, czy przycisk odpowiedzialny za wysyłanie wiadomości został naciśnięty
        bool buttonSendClicked = false;

        //Zmienna informująca o tym, czy przycisk odpowiedzialny za wysyłanie wiadomości został naciśnięty
        bool buttonSendRandomClicked = false;

        //Zmienna informująca o tym, czy przycisk odpowiedzialny za zakończenie wysyłania wiadomości został naciśnięty
        bool buttonStopSendingClicked = false;

        //Zmienna informujaca o tym, czy przycisk odpowiedzialny za połączenie z chmurą został wciśnięty
        bool buttonConnectToCloudClicked = false;

        //Task, na nim uruchomione będzie wysyłanie wiadomości
        Task t;

        //IP aplikacji klienckiej dołączającej się do sieci
        string ClientIP;

        //Port apliakcji klienckiej dołączającej się do sieci
        string ClientPort;

        //Port na którym aplikacja kliencka będzie łączyła się z chmura
        string CloudPort;

        /// <summary>
        /// Lista zawierająca IP wszystkich aplikacji klieckich 
        /// <para>IP aplikacji przechowywane są w postaci stringów</para>
        /// </summary>
        List<string> clientsiplist;

        /// <summary>
        /// Lista zawierająca IP wszystkich połączeń z chmurą
        /// <para>IP chmury przechowywane są w postaci stringów</para>
        /// </summary>
        List<string> cloudsiplist;

        //Paczka która będzie wysyłana za pośrednictwem sieci
        Package EONpackage;

        /// <summary>
        /// Konstruktor obiektu z klasy ClientApplication
        /// </summary>
        /// <param name="ClientIP">IP aplikacji klienckiej</param>
        /// <param name="ClientPort">Port aplikacji klienckiej</param>
        /// <param name="CloudPort">Port chmury</param>
        public ClientApplication(string ClientIP, string ClientPort, string CloudPort)
        {
            //Ustawienie CultureInfo na en-US spowoduje wyświetlanie się wyłapanych Exceptions w języku angielskim
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            //Zainicjowanie okna WindowsForm
            InitializeComponent();
            //Przypisanie referencji na dany obiekt
            _ClientApplication = this;
            //Przypisanie obiektowi IP aplikacji klienta, które zostało przekazane przez konstruktor z okna poziomu StartClientApplication
            this.ClientIP = ClientIP;
            //Przypisanie obiektowi Portu aplikacji klienta, które zostało przekazane przez konstruktor z okna poziomu StartClientApplication
            this.ClientPort = ClientPort;
            //Przypisanie obiektowi Portu chmury, które zostało przekazane przez konstruktor z okna poziomu StartClientApplication
            this.CloudPort = CloudPort;
            //Inicjalizacja listy zawierajacej ip klientów
            clientsiplist = new List<string>();
            //Inicjalizacja listy zawierajacej ip chmury, przez które będą łączyć się apliakcje klienckie
            cloudsiplist = new List<string>();

            //Odczytywanie z pliku konfiguracyjnego
            for (int i = 0; i < 3; i++)
            {
                //String niezbędny do porównania IP, a później odpowiedniego nazwania aplikacji
                string temp = "ClientIP" + i;
                //Dodawanie odczytanych IP aplikacjji klienckich do listy
                cloudsiplist.Add(ConfigurationManager.AppSettings[i + 3]);
                //Sprawdzenie czy wpisane IP w StartClientApplication jest takie samo IP w pliku konfiguracyjnym
                if (ConfigurationManager.AppSettings[i] == _ClientApplication.ClientIP)
                {
                    //Nadanie nazwy aplikacji - zgodnie z odczytanym IP z pliku konfiguracyjnego
                    _ClientApplication.Text = ConfigurationManager.AppSettings[temp];
                    //Dodanie IP klienckiej aplikacji do list
                    clientsiplist.Add(ConfigurationManager.AppSettings[i]);
                }
                else
                {
                    //Dodanie IP klienckiej aplikacji do listy
                    clientsiplist.Add(ConfigurationManager.AppSettings[i]);
                    //Dodanie IP aplikacji kliencjiej do listy
                    _ClientApplication.comboBoxClients.Items.Add(ConfigurationManager.AppSettings[temp]);
                }
            }

        }

        /// <summary>
        /// Funkcja uruchamiana w momencie naciśnięcia przycisku wysyłania
        /// </summary>
        /// <param name="sender">Obiekt, który odpowiedzialny jest za wysłanie eventu</param>
        /// <param name="e">Otrzymany event po naciśnięciu przycisku</param>
        private void buttonSend_Click(object sender, EventArgs e)
        {
            //Cel wysyłania naszej wiadomości, pobrany z comboBoxa
            string destination = comboBoxClients.SelectedItem.ToString();
            //Ustawienie wartości false pozwala na zatrzymanie wysyłania
            buttonStopSendingClicked = false;
            if (buttonSendClicked == false)
            {
                t = Task.Run(async () =>
                {
                    //Pętla odpowiedzialna za ciągłe wysyłanie wiadomości do wyznaczonego klienta
                    while (buttonStopSendingClicked != true)
                    {
                        //Pobranie wiadomości
                        string message = textBoxMessage.Text;
                        //Stworzenie wysyłanej paczki
                        EONpackage = new Package(message, 1, destination, _ClientApplication.ClientIP);
                        //Zamiana paczki na tablicę bajtów
                        byte[] bytemessage = EONpackage.toBytes();
                        //Stworzenie znacznika czasowego
                        string timestamp = Timestamp.generateTimestamp();
                        //Próba wysłania wiadomości odbywa sie tylko wtedy gdy jesteśmy podłaczeni do chmury
                        if (send.Connected)
                        {
                            //Wysłanie wiadomości (tablicy bajtów) za pośrednictwem gniazda
                            sS.SendingPackageBytes(send, bytemessage);
                            //Zaktualizowanie LogEventTextBoxa
                            _ClientApplication.updateLogTextBox("[" + timestamp + "] == SENDING MESSAGE ==  D_ClientIP " + destination);
                        }
                        else
                        {
                            MessageBox.Show("You need to be connected with Network Host!", "Info");
                        }

                        //Task, który służy wprowadzeniu opóźnienia między kolejnymi wysłanymi pakietami
                        var delay = await Task.Run(async () =>
                        {
                            Stopwatch sw = Stopwatch.StartNew();
                            await Task.Delay(2000);
                            sw.Stop();
                            return sw.ElapsedMilliseconds;
                        });
                    }
                });
            }

            //Zablokowanie podwójnego naciśnięcia przycisku wysyłania
            buttonSendClicked = true;
        }

        /// <summary>
        /// Funkcja uruchamiana w momencie naciśnięcia przycisku połączenie z chmura
        /// </summary>
        /// <param name="sender">Obiekt, który odpowiedzialny jest za wysłanie eventu</param>
        /// <param name="e">Otrzymany event po naciśnięciu przycisku</param>
        private void buttonConnectToCloud_Click(object sender, EventArgs e)
        {
            if (buttonConnectToCloudClicked == false)
            {
                //Pobranie indeksu na którym w liście znajduje się IP naszej klienckiej aplikacji
                int cloudipindex = clientsiplist.IndexOf(_ClientApplication.ClientIP);
                //Pobranie IP chmury z listy
                string cloudIP = cloudsiplist[cloudipindex];
                try
                {
                    //Próba połączenia się z IP chmury, z którego bedziemy nasłuchiwali wiadomości
                    send = sS.ConnectToEndPoint(cloudIP);
                    if (send.Connected)
                    {
                        //Uruchomienie nasłuchiwania w aplikacji klienckiej 
                        socket = sl.ListenAsync(_ClientApplication.ClientIP);
                        Task.Run(() =>
                        {
                            while (true)
                            {
                                //Zamienienie odebranej wiadomości na tablicę bajtów
                                byte[] messagebytes = sl.ProcessRecivedBytes(socket);
                                //Utowrzenie znacznika czasowego
                                string timestamp = Timestamp.generateTimestamp();
                                //Odpakowanie adresy nadawcy z otrzymanej wiadomości
                                string sourceIp = Package.exctractSourceIP(messagebytes).ToString();
                                //Stworzenie wiadomości, która zostanie wyświetlona na ekranie - odpakowanie treści wiadomości z paczki
                                string message2 = sourceIp + ": " + Package.extractUsableMessage(messagebytes, Package.extractUsableInfoLength(messagebytes));
                                //Pojawienie się informacji o otrzymaniu wiadomości
                                _ClientApplication.updateLogTextBox("[" + timestamp + "] == RECEIVED MESSAGE == S_ClientIP: " + sourceIp);
                                //Zauktualizowanie wiadomości w polu ReceivedMessage
                                _ClientApplication.updateReceivedMessageTextBox(message2);
                                _ClientApplication.updateReceivedMessageTextBox("\r\n");
                                message2 = null;
                                messagebytes = null;
                            }
                        });
                    }
                    else

                    {
                        throw new NullReferenceException();
                        buttonConnectToCloudClicked = true;
                    }

                }
                catch (Exception err)
                {
                    MessageBox.Show("Unable to connect to the cloud", "Attention!");
                }
            }
        }

        /// <summary>
        /// Funkcja uruchamiana w momencie naciśnięcia przycisku odpowiedzialnego za wysyłanie wiadomości o losowej długości
        /// </summary>
        /// <param name="sender">Obiekt, który odpowiedzialny jest za wysłanie eventu</param>
        /// <param name="e">Otrzymany event po naciśnięciu przycisku</param>
        private void buttonDifferentMessages_Click(object sender, EventArgs e)
        {

            string message = null;
            //Pobranie celu do którego wysłana zostanie wiadomość
            string destination = comboBoxClients.SelectedItem.ToString();
            buttonStopSendingClicked = false;
            if (buttonSendRandomClicked == false)
            {
                t = Task.Run(async () =>
                {
                    while (buttonStopSendingClicked != true)
                    {
                        message = null;
                        //Wygnenerowanie losowej wiadomości o maksymalnej długości
                        message = RandomMessageGenerator.generateRandomMessage(40);
                        //Stworzenie paczki, która bedzie wysyłana do drugiego hosta
                        EONpackage = new Package(message, 1, destination, _ClientApplication.ClientIP);
                        //Zamiana wiadomości na tablice bajtów
                        byte[] bytemessage = EONpackage.toBytes();
                        //Stworzenie znacznego czasowego
                        string timestamp = Timestamp.generateTimestamp();
                        if (send.Connected)
                        {
                            //Wysłanie wiadomości do chmury
                            sS.SendingPackageBytes(send, bytemessage);
                            //Zaktualizowanie wiadomości w polu LogEvent
                            _ClientApplication.updateLogTextBox("[" + timestamp + "] == SENDING MESSAGE ==  D_ClientIP " + destination);
                        }
                        else
                        {
                            MessageBox.Show("You need to be connected with Network Host!", "Info");
                        }

                        //Uruchomienie taska, który będzie odpowiadał za opóźnienie wysyłania kolejnych wiadomości
                        var delay = await Task.Run(async () =>
                        {
                            Stopwatch sw = Stopwatch.StartNew();
                            await Task.Delay(2000);
                            sw.Stop();
                            return sw.ElapsedMilliseconds;
                        });
                    }
                });
            }
            //Zablokowanie podwójnego naciśnięcia przycisku wysyłania
            buttonSendRandomClicked = true;
        }


        /// <summary>
        /// Funkcja uruchamiana w momencie naciśnięcia przycisku  odpowiedzialnego za zatrzymanie wysyłania
        /// </summary>
        /// <param name="sender">Obiekt, który odpowiedzialny jest za wysłanie eventu</param>
        /// <param name="e">Otrzymany event po naciśnięciu przycisku</param>
        private void buttonStopSending_Click(object sender, EventArgs e)
        {
            buttonSendClicked = false;
            buttonSendRandomClicked = false;
            buttonStopSendingClicked = true;
        }

        /// <summary>
        /// Funkcja odpowiedzialna za aktualizowanie pola LogEvent - służy przesyłaniu wiadomości miedzy wątkami
        /// </summary>
        /// <param name="message">Wiadomość o którą zostanie zaktualizowane pole LogEvent</param>
        public void updateLogTextBox(string message)
        {
            _ClientApplication.textBoxLog.Invoke(new Action(delegate ()
            {
                _ClientApplication.textBoxLog.AppendText(message + "\r\n");
            }));
        }

        /// <summary>
        /// Funkcja odpowiedzialna za aktualizowanie pola ReceivedMessage - służy przesyłaniu wiadomości miedzy wątkami
        /// </summary>
        /// <param name="message">Wiadomość o którą zostanie zaktualizowane pole ReceivedMessage</param>
        public void updateReceivedMessageTextBox(string message)
        {
            _ClientApplication.textBoxReceived.Invoke(new Action(delegate ()
            {
                _ClientApplication.textBoxReceived.AppendText(message);
            }));
        }
    }
}
