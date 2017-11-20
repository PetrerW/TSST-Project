using ClientNode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworkingTools;
using System.Threading;
using System.Configuration;
using System.Globalization;

namespace ClientNode
{
    public partial class ClientApplication : Form
    {
        //Niezbędne zmienne
        public static SocketListener sl = new SocketListener();
        public static SocketSending sS = new SocketSending();
        public static CancellationTokenSource _cts = new CancellationTokenSource();
        //Obiekt związany z pobraniem aktualnego czasu
        private DateTime localtime;
        //Obiekt klasy Form1 - niezbędny w celu odwoływania się do pracy naszego ubiektu
        public ClientApplication _ClientApplication;
        public Socket send;
        public Socket socket;
        bool buttonSendClicked = false;
        bool buttonStopSendingClicked = false;
        Task t;
        string ClientIP;
        string ClientPort;
        string CloudPort;
        List<string> clientslist;
        List<string> clientsiplist;
        Package EONpackage;

        public ClientApplication(string ClientIP, string ClientPort, string CloudPort)
        {
            InitializeComponent();
            _ClientApplication = this;
            this.ClientIP = ClientIP;
            this.ClientPort = ClientPort;
            this.CloudPort = CloudPort;
            clientslist = new List<string>();
            clientsiplist = new List<string>();

            int numberofclients = int.Parse(ConfigurationManager.AppSettings["NumberOfClients"]);
            for (int i = 1; i <= numberofclients; i++)
            {
                string temp = "ClientName" + i;
                if (ConfigurationManager.AppSettings[i + numberofclients] != _ClientApplication.ClientIP)
                {
                    clientslist.Add(ConfigurationManager.AppSettings[temp]);
                    clientsiplist.Add(ConfigurationManager.AppSettings[i + numberofclients]);
                    _ClientApplication.comboBoxClients.Items.Add(ConfigurationManager.AppSettings[temp]);
                }
                else
                {
                    _ClientApplication.Text = ConfigurationManager.AppSettings[temp];
                }
            }

        }

        //Akcja po naciśnięciu przycisku wyślij
        private void buttonSend_Click(object sender, EventArgs e)
        {
            //Cel wysyłania naszej wiadomości, pobrany z comboBoxa
            string destination = comboBoxClients.SelectedItem.ToString();
            int destinationIPindex = clientslist.IndexOf(destination);
            string destinationIP = clientsiplist[destinationIPindex];
            buttonStopSendingClicked = false;
            if (buttonSendClicked == false)
            {
                t = Task.Run(async () =>
                {
                    while (buttonStopSendingClicked != true)
                    {
                        string message = textBoxMessage.Text;
                        EONpackage = new Package(message, 1, destinationIP, _ClientApplication.ClientIP, 1, 1, 1, Convert.ToInt16(message.Length));
                        byte[] bytemessage = EONpackage.toBytes();
                        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture);
                        if (send.Connected)
                        {
                            sS.SendingPackageBytes(send, bytemessage);
                            _ClientApplication.updateLogTextBox("[" + timestamp + "] [TRYING TO SEND A MESSAGE] [ClientName]: " + destination + " " + destinationIP);
                        }
                        var delay = await Task.Run(async () =>
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        await Task.Delay(3000);
                        sw.Stop();
                        return sw.ElapsedMilliseconds;
                    });
                    }
                });
            }
            buttonSendClicked = true;
        }

        public void updateLogTextBox(string message)
        {
            _ClientApplication.textBoxLog.Invoke(new Action(delegate ()
            {
                _ClientApplication.textBoxLog.AppendText(message + "\n");
            }));
        }

        public void updateReceivedMessageTextBox(string message)
        {
            _ClientApplication.textBoxReceived.Invoke(new Action(delegate ()
            {
                _ClientApplication.textBoxReceived.AppendText(message + "\r\n");
            }));
        }

        private void buttonConnectToCloud_Click(object sender, EventArgs e)
        {
            send = sS.ConnectToEndPoint("127.0.0.13");
            socket = sl.ListenAsync(_ClientApplication.ClientIP);
            Task.Run(() =>
            {
                while (true)
                {
                    byte[] messagebytes = sl.ProcessRecivedBytes(socket);
                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture);
                    string sourceIp = Package.exctractDestinationIP(messagebytes).ToString();
                    int clientsourceIPindex = clientsiplist.IndexOf(sourceIp);
                    string clientsourcename = clientslist[clientsourceIPindex];
                    string message2 = clientsourcename + ": " + Package.extractUsableMessage(messagebytes, Package.extractUsableInfoLength(messagebytes));
                    _ClientApplication.updateLogTextBox("[" + timestamp + "] [RECEIVED MESSAGE] [ClientName]: " + clientsourcename);
                    _ClientApplication.updateReceivedMessageTextBox(message2);
                    message2 = null;
                    messagebytes = null;
                }
            });

        }

        private void buttonStopSending_Click(object sender, EventArgs e)
        {
            buttonSendClicked = false;
            buttonStopSendingClicked = true;
        }

        private void buttonDifferentMessages_Click(object sender, EventArgs e)
        {
            string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
            string message = null;
            int num;
            string destination = comboBoxClients.SelectedItem.ToString();
            int destinationIPindex = clientslist.IndexOf(destination);
            string destinationIP = clientsiplist[destinationIPindex];
            buttonStopSendingClicked = false;
            if (buttonSendClicked == false)
            {
                t = Task.Run(async () =>
                {
                    while (buttonStopSendingClicked != true)
                    {
                        message = null;
                        localtime = DateTime.Now;
                        Random randomstrlength = new Random();
                        Random rand = new Random();
                        int strlength = randomstrlength.Next(0, 39);
                        for (int i = 0; i < strlength; i++)
                        {
                            num = rand.Next(0, chars.Length - 1);
                            message = message + chars[num];
                        }
                        EONpackage = new Package(message, 1, destinationIP, _ClientApplication.ClientIP, 1, 1, 1, Convert.ToInt16(message.Length));
                        byte[] bytemessage = EONpackage.toBytes();
                        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                           CultureInfo.InvariantCulture);
                        if (send.Connected)
                        {
                            sS.SendingPackageBytes(send, bytemessage);
                            _ClientApplication.updateLogTextBox("[" + timestamp + "] [TRYING TO SEND A MESSAGE] [ClientName]: " + destination + " " + destinationIP);
                        }
                        var delay = await Task.Run(async () =>
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        await Task.Delay(3000);
                        sw.Stop();
                        return sw.ElapsedMilliseconds;
                    });
                    }
                });
            }
            buttonSendClicked = true;
        }
    }
}
