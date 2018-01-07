using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Configuration;
using NetworkingTools;


namespace SubNetwork
{
    public struct StoreLambda
    {
        public short frequency
        { get; set; }
        public string token { get; set; }
    }

    class LinkResourceManager
    {
        public NetworkNode.EONTable eonTable;

        public string numberLRM;

        List<SubNetworkPoint> snp;
        List<StoreLambda> listOfFrequency;

        public LinkResourceManager(string numberLRM)
        {
            this.numberLRM = numberLRM;
            eonTable = new NetworkNode.EONTable();
            snp = new List<SubNetworkPoint>();
            listOfFrequency = new List<StoreLambda>();
            readingSNP();
        }

        private void SendingMessage(string ipaddress, string message)
        {
            byte[] data = new byte[64];

            UdpClient newsock = new UdpClient();

            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(ipaddress), 11000);

            try
            {
                data = Encoding.ASCII.GetBytes(message);
                newsock.Send(data, data.Length, sender);

            }
            catch (Exception ex)
            {


            }

        }


        private void ReceivedMessage()
        {

            byte[] data = new byte[64];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            string ipaddress;

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["LRM" + numberLRM]), 11000);
            UdpClient newsock = new UdpClient(ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            try
            {

                data = newsock.Receive(ref sender);

                string receivedMesage = Encoding.ASCII.GetString(data);

                char separator = '#';
                string[] words = receivedMesage.Split(separator);


            }
            catch (Exception)
            {

            }


        }

        private void chooiceAction(string[] message)
        {
            string action = message[1];
            string address = message[0];
            //string token = message[2];

            if (action == MessageNames.LINK_CONNECTION_REQUEST)
            {
                string bitrate = message[2];
                string token = message[3];
                string hops = message[4];
                string in_Or_Out = message[5];
                string routerAddress = message[6];
                string routerPort = message[7];

                if (message.Length == 8)
                {
                    //Z racji tego że do CC przychodzi z RC pula punktów, a do LRM przekazujemy tylko pojedyńcze snp to wprowadziłem token
                    //który pozwoli jednoznacznie identyfikować nadawanie tej samej lambdy w ramach jednej operacji
                    //Wyszukujemy czy lambda z danym identyfikatorem jest już w liście
                    // Jezeli nie to nadajemy lambde, ktora nie jest jeszcze używana

                    int objectTmp = listOfFrequency.FindIndex(x => x.token == token);

                    short modulation = NetworkNode.BorderNodeCommutationTable.determineModulationPerformance(Int16.Parse(hops));
                    short band = NetworkNode.BorderNodeCommutationTable.determineBand(Int16.Parse(bitrate), modulation);

                    if (objectTmp == (-1))
                    {
                        short frequency = checkAvaliability(band);
                        StoreLambda lambda = new StoreLambda();
                        lambda.frequency = frequency;
                        lambda.token = token;
                        listOfFrequency.Add(lambda);
                        //Jeżeli wszystkie lambdy są zajęte to checkAvaliability zwrca -1
                        if (frequency != (-1))
                        {
                            eonTable.addRow(new NetworkNode.EONTableRowIN(frequency, band));
                            string responseMessage = ConfigurationManager.AppSettings["LRM" + numberLRM] + "#" +
                                  MessageNames.LINK_CONNECTION_REQUEST + "#" + token + "#" + "LAMBDA" + frequency + "#";
                            SendingMessage(address, responseMessage);

                            string localTopologyMessage = ConfigurationManager.AppSettings["LRM" + numberLRM] + "#" +
                                  MessageNames.LOCAL_TOPOLOGY + "#" + routerAddress + "#" + routerPort + "#" + in_Or_Out + "#" + band / 64 * 100 + "#";
                            SendingMessage(ConfigurationManager.AppSettings["RC" + numberLRM], localTopologyMessage);
                        }
                        else { Console.Write("All frequency are busy"); }
                    }
                    else
                    {

                        string responseMessage = ConfigurationManager.AppSettings["LRM" + numberLRM] + "#" +
                                               MessageNames.LINK_CONNECTION_REQUEST + "#" + token + "#" + "OK" + "#";
                        SendingMessage(address, responseMessage);

                        string localTopologyMessage = ConfigurationManager.AppSettings["LRM" + numberLRM] + "#" +
                                  MessageNames.LOCAL_TOPOLOGY + "#" + routerAddress + "#" + routerPort + "#" + in_Or_Out + "#" + band / 64 * 100 + "#";
                        SendingMessage(ConfigurationManager.AppSettings["RC" + numberLRM], localTopologyMessage);
                    }
                }
                else
                {
                    string frequencyTmp = message[8];
                    int objectTmp = listOfFrequency.FindIndex(x => x.token == token);

                    if (objectTmp == (-1))
                    {

                        short modulation = NetworkNode.BorderNodeCommutationTable.determineModulationPerformance(Int16.Parse(hops));
                        short band = NetworkNode.BorderNodeCommutationTable.determineBand(Int16.Parse(bitrate), modulation);
                        bool true_or_false = eonTable.CheckAvailability(Int16.Parse(frequencyTmp), band, "in");
                        //Jeżeli wszystkie lambdy są zajęte to checkAvaliability zwrca false
                        if (true_or_false)
                        {
                            //jeżeli nie ma częstotliwości dla tego tokena to tworzymy taką parę i dodajemy ją do listy, pozwalającej kontrolować zajęte częstotliwości,
                            // i odnajdywać kolejne lambdę dla tego samego zestawienia ścieżki                         
                            StoreLambda lambda = new StoreLambda();
                            lambda.frequency = Int16.Parse(frequencyTmp);
                            lambda.token = token;
                            listOfFrequency.Add(lambda);
                            //WIadomość zwrotna do CC potwierdzająca przydzielenie zasobów
                            eonTable.addRow(new NetworkNode.EONTableRowIN(Int16.Parse(frequencyTmp), band));
                            string responseMessage = ConfigurationManager.AppSettings["LRM" + numberLRM] + "#" +
                                  MessageNames.LINK_CONNECTION_REQUEST + "#" + token + "#" + "LAMBDA" + frequencyTmp + "#";
                            SendingMessage(address, responseMessage);

                            //Widomość aktualizująca informacje o sieci w RC czyli LOCAL_TOPOLOGY
                            string localTopologyMessage = ConfigurationManager.AppSettings["LRM" + numberLRM] + "#" +
                                  MessageNames.LOCAL_TOPOLOGY + "#" + routerAddress + "#" + routerPort + "#" + in_Or_Out + "#" + band / 64 * 100 + "#";
                            SendingMessage(ConfigurationManager.AppSettings["RC" + numberLRM], localTopologyMessage);
                        }
                        else { Console.Write("All frequency are busy"); }
                    }
                    else
                    {

                        string responseMessage = ConfigurationManager.AppSettings["LRM" + numberLRM] + "#" +
                                               MessageNames.LINK_CONNECTION_REQUEST + "#" + token + "#" + "OK" + "#";
                        SendingMessage(address, responseMessage);
                    }
                }
            }
            else if (action == MessageNames.LINK_CONNECTION_DEALLOCATION)
            {

            }
        }

        public void readingSNP()
        {
            int numberOfSNP = Int32.Parse(ConfigurationManager.AppSettings[numberLRM + "SNPCount"]);
            for (int i = 0; i < numberOfSNP; i++)
            {
                string[] words = ConfigurationManager.AppSettings[numberLRM + "SNP" + i].Split('#');
                snp.Add(new SubNetworkPoint(IPAddress.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2])));
            }
        }

        public short checkAvaliability(short band)
        {
            bool flag = true;
            for (short i = 0; i < eonTable.InFrequencies.Count; i++)
            {
                for (int j = i; j < i + band; j++)
                {
                    if (eonTable.InFrequencies[j] != (-1))
                    {
                        flag = false;
                    }
                }
                if (flag == true)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
