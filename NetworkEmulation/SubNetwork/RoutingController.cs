using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkingTools;
using NetworkNode;

namespace SubNetwork
{
    /// <summary>
    /// Klasaz funkcjonalnoscia RC.
    /// </summary>
    public class RoutingController
    {
        /// <summary>
        /// Numer RC z całej topologii
        /// </summary>
        public int numberRC;

        //Topologia sieci
        private Topology topology;

        /// <summary>
        /// Lista par: SNPP z najwyższym adresem IP podsieci, IP Routing Controllera z tej podsieci
        /// </summary>
        public List<SNandRC> SN_RCs;

        //Lista tablic zajetosci EONowej. Jeden wpis do listy to jeden interfejs EONowy danego routera.
        //public List<EONTable> EonTables;

        public RoutingController()
        {
            this.topology = new Topology();
            this.mySubNetwork = new SubNetwork();
            OtherSubNetworks = new List<SubNetwork>();
            SN_RCs = new List<SNandRC>();
            this.numberRC = 1;
            //this.EonTables = new List<EONTable>();
        }

        /// <summary>
        /// Konstruktor, określający numer RC z ogólnej topologii
        /// </summary>
        /// <param name="numberRc"></param>
        public RoutingController(int numberRc) : this()
        {
            this.numberRC = numberRc;
        }

        public Topology GetTopology
        {
            get { return this.topology; }
            set { this.topology = value; }
        }

        /// <summary>
        /// Podsieć, w której znajduje się RC
        /// </summary>
        public SubNetwork mySubNetwork;

        /// <summary>
        /// Lista innych podsieci z topologii
        /// </summary>
        public List<SubNetwork> OtherSubNetworks;

        /// <summary>
        /// Wysyłanie wiadomości do 
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="message"></param>
        private void SendMessage(string ipaddress, string message)
        {
            byte[] data = new byte[64];

            UdpClient newsock = new UdpClient();

            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(ipaddress), 11000);

            try
            {
                data = Encoding.ASCII.GetBytes(message);
                newsock.Send(data, data.Length, sender);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Odbieranie wiadomości
        /// </summary>
        private void ReceiveMessage()
        {
            byte[] data = new byte[64];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["RC" + numberRC]), 11000);
            UdpClient newsock = new UdpClient(ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            //IPEndPoint sender = new IPEndPoint(IPAddress.Parse("xxx.xxx.xxx.xxx", 0);

            try
            {
                while (true)
                {
                    data = newsock.Receive(ref sender);

                    string receivedMesage = Encoding.ASCII.GetString(data);

                    char separator = '#';
                    string[] words = receivedMesage.Split(separator);

                    Task.Run(() => chooseAction(words));
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("RoutingController.ReceiveMessage(): " + E.Message);
            }
        }

        /// <summary>
        /// Funkcja uruchamiajaca odpowiednie funkcje na podsawie otrzymanej wiadomości
        /// 
        /// </summary>
        /// <param name="words"></param>
        private void chooseAction(string[] words)
        {
            string message = words[1];

            if (message == MessageNames.LOCAL_TOPOLOGY)
            {
                handleLocalTopology(words);
            }
            else if (message == MessageNames.NETWORK_TOPOLOGY)
            {
                handleNetworkTopology(words);
            }
            else if (message == MessageNames.ROUTE_TABLE_QUERY)
            {
                handleRouteTableQuery(words);
            }
            else
            {
                //Ostrzezenie ze zly format wiadomosci
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="words"></param>
        public void handleLocalTopology(string[] words)
        {
            //TODO:
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="words"></param>
        public void handleNetworkTopology(string[] words)
        {
            //TODO:
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="words"></param>
        public void handleRouteTableQuery(string[] words)
        {
            string pathFrom = words[0]; //TODO:= words[x]
            string pathTo = words[0]; //TODO:= words[x]
            short band = Int16.Parse(words[0]); //TODO:= Int16.Parse(words[x]); Czy to na pewno jest potrzebne? 

            var snpps = topology.getPathOfSNPPs(new SubNetworkPointPool(new SubNetworkPoint(IPAddress.Parse(pathFrom))),
                new SubNetworkPointPool(new SubNetworkPoint(IPAddress.Parse(pathTo))), band);

            foreach (var snpp in snpps)
            {
                SNandRC sNandRc = SN_RCs.Find(x => x.snpp == snpp);
                if (sNandRc != null)
                {
                    //TODO: powiadom tego RC i każ mu wyznaczyć ścieżkę
                    //TODO: dodaj ścieżkę wyznaczoną przez RC do obecnej ścieżki
                }
            }

        }

        public List<SubNetworkPointPool> getPathFromRC(string rc_ip, SubNetworkPointPool Source,
            SubNetworkPointPool Destnation, short widestBand, short frequency)
        {
            //TODO
            byte[] data = new byte[64];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SendTo(data, new IPEndPoint(IPAddress.Parse(rc_ip), 11000));
            return null;
        }

        public string generateGetPathFromRCMessage(SubNetworkPointPool Source, SubNetworkPointPool Destination, short widestBand, short frequency)
        {
            return "ROUTE_PATH\n" + Source.snps[0].ipaddress.ToString() + "\n" +
                   Destination.snps[0].ipaddress.ToString() + "\n" + widestBand + "\n" + frequency;
        }

        /// <summary>
        /// Zamienia sekwencję SNPPów na wiadomośc do wysłania 
        /// TODO: Zamien na same IP i nry lacz
        /// </summary>
        /// <param name="snpps"></param>
        /// <returns></returns>
        public string snppsToString(List<SubNetworkPointPool> snpps)
        {
            //Wyjsciowy string
            StringBuilder SB = new StringBuilder();

            SB.Append(snpps[0].snps[0].ipaddress.ToString());
            SB.Append("#");
            SB.Append(snpps[0].snps[0].portOUT.ToString());
            SB.Append("#");

            for (int i = 1; i < snpps.Count-1; i++)
            {
                SB.Append(snpps[i].snps[0].portIN.ToString());
                SB.Append("#");
                SB.Append(snpps[i].snps[0].ipaddress.ToString());
                SB.Append("#");
                SB.Append(snpps[i].snps[0].portOUT.ToString());
                SB.Append("#");
            }

            SB.Append(snpps[snpps.Count-1].snps[0].portIN.ToString());
            SB.Append("#");
            SB.Append(snpps[snpps.Count-1].snps[0].ipaddress.ToString());

            return SB.ToString();
        }
    }
}
