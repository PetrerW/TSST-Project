using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using NetworkingTools;
using System.Configuration;

namespace SubNetwork
{

    class MessageCC
    {
        public string addressCC { get; set; }
        public string addressStart { get; set; }
        public string startPort { get; set; }
        public string addressEnd { get; set; }
        public string endPort { get; set; }

        public MessageCC(string addressCC,string addressS,string portA,string addressE,string portB)
        {
            this.addressCC = addressCC;
            this.addressStart = addressS;
            this.startPort = portA;
            this.addressCC = addressE;
            this.endPort = portB;

        }

    }

  public class ControlConnection
    {
        string numberCC;
        List<MessageStruct> listOfMessages;
        List<MessageCC> messagesCC;
        MessageStruct response;
        MessageCC transponderStart, transponderEnd;
        string numberOfHops;
        string frequency;
        string connectionBitrate;
        //określa, czy ten CC jest centralnym CC dla tej sieci operatorskiej, będący najwyżej w hierarchii
        bool mainCC = false;

        public ControlConnection(string numberCC)
        {
            this.numberCC = numberCC;
            listOfMessages = new List<MessageStruct>();
            messagesCC = new List<MessageCC>();


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
            catch (Exception)
            {

            }

        }


        public void ReceivedMessage()
        {

            byte[] data = new byte[64];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);



            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["CC" + numberCC]), 11000);
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

                    Task.Run(()=>chooiceAction(words));
                }


            }
            catch (Exception)
            {

            }


        }


        private void ConnectionRequest(SubNetworkPoint startPoint, SubNetworkPoint endPoint)
        {

        }

        private void chooiceAction(string[] message)
        {
            string action = message[1];
            string address = message[0];
            //string token = message[2];


            if (action == MessageNames.CONNECTION_REQUEST)
            {
                string code = message[2];
                string typeObject = message[3];

                if (typeObject.StartsWith("NCC"))
                {
                    //Jest to CC najwyzsze w hierarchii bo kontaktuje się z nim NCC
                    mainCC = true;

                    string startAddress = message[4];
                    string endAddress = message[5];
                    string bitrate = message[6];
                    string hops = message[7];
                    numberOfHops = hops;
                    connectionBitrate = bitrate;

                    if (code == "PUT")
                    {
                        generateLogReceived(typeObject, MessageNames.CONNECTION_REQUEST);
                        response = new MessageStruct();
                        response.ip = message[0];
                        response.function = message[1];
                        

                        //Według przyjętej konwencji jeżeli hops=1 to połącznie odbywa się w ramach jednej sieci operatorskiej 
                        if(Int32.Parse(hops)==1)
                        {
                            //Oznacza to że wezeł poczatkowy i końcowy na tym poziomie są węzłami dostępowymi z transponderami
                            //Zamieniamy z szarej częstotliwości na inną(wejście), lub z innej na szarą(wyjście)
                            transponderStart = new MessageCC("", startAddress, "", endAddress, "");
                            transponderEnd = new MessageCC("", startAddress, "", endAddress, "");

                        }else
                        {
                            MessageStruct msg = new MessageStruct();
                            msg.ip = ConfigurationManager.AppSettings["CCDomain" + numberCC];
                            msg.function = MessageNames.PEER_COORDINATION;
                            listOfMessages.Add(msg);
                            listOfMessages.Add(new MessageStruct() { function=MessageNames.CONNECTION_REQUEST, ip=ConfigurationManager.AppSettings["CC"+numberCC]});

                            transponderStart = new MessageCC("", startAddress, "", endAddress, "");
                            string messagePEERCOORDINATION = ConfigurationManager.AppSettings["CC" + numberCC] + "#"
                           + MessageNames.PEER_COORDINATION + "#" + "PUT" + "#" + "CC" + "#" + startAddress + "#" + endAddress +
                           "#" + bitrate + "#" + hops + "#";
                            SendingMessage(ConfigurationManager.AppSettings["CCDomain" + numberCC], messagePEERCOORDINATION);
                        }

                       
                      string  messageToSend = ConfigurationManager.AppSettings["CC" + numberCC] + "#"
                            + MessageNames.ROUTE_TABLE_QUERY + "#" + startAddress + "#" + endAddress + "#" + bitrate + "#" + hops + "#";
                        SendingMessage(ConfigurationManager.AppSettings["RC" + numberCC], messageToSend);
                    }
                }
                else
                {
                    if (code == "PUT")
                    {
                        generateLogReceived(typeObject, MessageNames.CONNECTION_REQUEST);
                        response = new MessageStruct();
                        response.ip = message[0];
                        response.function = message[1];

                        if (ConfigurationManager.AppSettings["RC" + numberCC] != "")
                        {
                            string startAddress = message[4];
                            string startPort = message[5];
                            string endAddress = message[6];
                            string endPort = message[7];
                            string bitrate = message[8];
                            string hops = message[9];
                            string connectionFrequency = message[10];
                            numberOfHops = hops;
                            connectionBitrate = bitrate;
                            frequency = connectionFrequency;
                            

                            string messageToSend = ConfigurationManager.AppSettings["CC" + numberCC] + "#"
                              + MessageNames.ROUTE_TABLE_QUERY + "#" + startAddress + "#" + endAddress + "#" + bitrate + "#" + hops + "#";

                            SendingMessage(ConfigurationManager.AppSettings["RC" + numberCC], messageToSend);
                        }
                        else
                        {
                            //message[4]-adress CC siedzacego na routerze
                            //message[6]-port na wejsciu
                            //message[8]-port na wysciu
                            //message[9]-czestotliwosc na wejsciu
                            //message[10]-czestotliwosc na wyjsciu


                            /*Connection Control Interface CCI*/
                            //Freq_IN#Port_IN#freq_OUT#Port_OUT#

                            string messageUpdateRouter = message[9] + "#" + message[6] + "#" + message[10] + "#" + message[8] + "#";
                            SendingMessage(message[4], messageUpdateRouter);

                            string responseMessageToCC = ConfigurationManager.AppSettings["CC" + numberCC] + "#" + MessageNames.CONNECTION_REQUEST + "#" + "OK#";
                            SendingMessage(message[0], responseMessageToCC);

                        }
                    }
                    else if (code == "OK")
                    {
                        generateLogReceived(typeObject, MessageNames.CONNECTION_REQUEST + "RESPONSE");
                        

                        int index = listOfMessages.FindIndex(x => x.function == MessageNames.CONNECTION_REQUEST);
                        MessageStruct msg = listOfMessages.ElementAt(index);
                        msg.count = msg.count - 1;
                        listOfMessages[index] = msg;
                        if (msg.count == 0)
                        {
                            listOfMessages.RemoveAt(index);

                            if(listOfMessages.Count==0)
                            {
                                string responseMessageToUp = ConfigurationManager.AppSettings["CC" + numberCC] + "#" + 
                                    MessageNames.CONNECTION_REQUEST + "#" + "OK" + "#";
                                SendingMessage(response.ip, responseMessageToUp);
                            }
                        }                            
                    }
                }
            }
            else if (action == MessageNames.LINK_CONNECTION_DEALLOCATION)
            {

            }
            else if (action == MessageNames.LINK_CONNECTION_REQUEST)
            {
                generateLogReceived("LRM", MessageNames.LINK_CONNECTION_REQUEST+"RESPONSE");
                string token = message[2];
                string code = message[3];
                if (code == "LAMBDA")
                {
                     frequency = message[4];
                    int index = listOfMessages.FindIndex(x => x.tocken == token);
                    MessageStruct msg = listOfMessages.ElementAt(index);
                    msg.count = msg.count - 1;
                    
                    listOfMessages[index] = msg;

                }
                else if (code == "OK")
                {
                    int index = listOfMessages.FindIndex(x => x.tocken == token);
                    MessageStruct msg = listOfMessages.ElementAt(index);
                    msg.count = msg.count - 1;
                    listOfMessages[index] = msg;
                    if (msg.count == 0)
                    {
                        if(mainCC==true)
                        {
                            //Według przyjętej konwencji jeżeli hops=1 to połącznie odbywa się w ramach jednej sieci operatorskiej 
                            if (Int32.Parse(numberOfHops) == 1)
                            {
                                transponderStart.addressCC = messagesCC[0].addressCC;
                                transponderStart.startPort = messagesCC[0].startPort;
                                transponderEnd.addressCC = messagesCC[messagesCC.Count-1].addressCC;
                                transponderStart.endPort = messagesCC[messagesCC.Count - 1].endPort;

                            }
                            else
                            {
                               
                            }
                        }


                        foreach (var item in messagesCC)
                        {

                            string MessageToCC = ConfigurationManager.AppSettings["CC" + numberCC] + "#" + MessageNames.LINK_CONNECTION_REQUEST+"#" +
                               "PUT"+ "#" + "CC" + "#" +item.addressStart+"#"+item.startPort+"#"+item.addressEnd+"#"+item.endPort+"#"+
                               connectionBitrate+"#"+numberOfHops+"#"+ frequency + "#";
                            SendingMessage(item.addressCC, MessageToCC);
                        }
                        listOfMessages.RemoveAt(index);
                        messagesCC.Clear();
                        frequency = string.Empty;
                        numberOfHops = string.Empty;
                        connectionBitrate = string.Empty;
                    }
                }
                

            }
            else if (action == MessageNames.ROUTE_TABLE_QUERY)
            {
               
                List<SubNetworkPoint> subNetworkPoints = new List<SubNetworkPoint>();
                generateLogReceived("RC", MessageNames.ROUTE_TABLE_QUERY + "RESPONSE");
                string bitrate = message[2];
                string token = message[3];
                MessageStruct lrmMessage = new MessageStruct();
                lrmMessage.function = MessageNames.LINK_CONNECTION_REQUEST;
                lrmMessage.ip = ConfigurationManager.AppSettings["LRM" + numberCC];
                lrmMessage.tocken = token;
                lrmMessage.count = (message.Length - 4) / 5 * 2;
                listOfMessages.Add(lrmMessage);
                int index =listOfMessages.FindIndex(x => x.function == MessageNames.CONNECTION_REQUEST);
                MessageStruct CCMessage = listOfMessages.ElementAt(index);
                CCMessage.count = (message.Length - 4) / 5;
                listOfMessages[index] = CCMessage;



                for (int i = 4; i < message.Length; i += 5)
                {
                    
                    messagesCC.Add(new MessageCC(message[i], message[i + 1], message[i + 2], message[i + 3], message[i + 4]));
                    if (frequency == null)
                    {
                        string messageToLRMIN = ConfigurationManager.AppSettings["CC" + numberCC] + MessageNames.LINK_CONNECTION_REQUEST + "#" + bitrate +
                          "#" + token + "#" + numberOfHops +
                            "#" + "IN" + "#" + message[i + 1] + "#" + message[i + 2] + "#";
                        SendingMessage(ConfigurationManager.AppSettings["LRM" + numberCC], messageToLRMIN);
                        string messageToLRMOUT = ConfigurationManager.AppSettings["CC" + numberCC] + MessageNames.LINK_CONNECTION_REQUEST + "#" + bitrate +
                            "#" + token + "#" + numberOfHops + "#" + "OUT" + "#" + message[i + 3] + "#" + message[i + 4] + "#";

                        SendingMessage(ConfigurationManager.AppSettings["LRM" + numberCC], messageToLRMOUT);
                    }
                    else
                    {
                        string messageToLRMIN = ConfigurationManager.AppSettings["CC" + numberCC] + MessageNames.LINK_CONNECTION_REQUEST + "#" + bitrate +
                         "#" + token + "#" + numberOfHops +
                           "#" + "IN" + "#" + message[i + 1] + "#" + message[i + 2] + "#"+frequency+"#";
                        SendingMessage(ConfigurationManager.AppSettings["LRM" + numberCC], messageToLRMIN);
                        string messageToLRMOUT = ConfigurationManager.AppSettings["CC" + numberCC] + MessageNames.LINK_CONNECTION_REQUEST + "#" + bitrate +
                            "#" + token + "#" + numberOfHops + "#" + "OUT" + "#" + message[i + 3] + "#" + message[i + 4] + "#" + frequency + "#";

                        SendingMessage(ConfigurationManager.AppSettings["LRM" + numberCC], messageToLRMOUT);
                    }                  

                   /* subNetworkPoints.Add(new SubNetworkPoint(IPAddress.Parse(message[i + 1]), Int32.Parse(message[i + 2])));
                    subNetworkPoints.Add(new SubNetworkPoint(IPAddress.Parse(message[i + 3]), Int32.Parse(message[i + 4])));*/

                }

            }
            else if (action == MessageNames.PEER_COORDINATION)
            {
                string code = message[2];
                string typeObject = message[3];
                string startAddress = message[4];
                string endAddress = message[5];
                string bitrate = message[6];
                string hops = message[7];

                if (code == "PUT")
                {
                    response = new MessageStruct();
                    response.ip = message[0];
                    response.function = message[1];

                    string messageToSend = ConfigurationManager.AppSettings["CC" + numberCC] + "#"
                              + MessageNames.ROUTE_TABLE_QUERY + "#" + startAddress + "#" + endAddress + "#" + bitrate + "#";
                    SendingMessage(ConfigurationManager.AppSettings["RC" + numberCC], messageToSend);
                }
                else if (code == "OK")
                {
                    generateLogReceived("CC", MessageNames.PEER_COORDINATION + "RESPONSE");
                    int index = listOfMessages.FindIndex(x => x.function == MessageNames.PEER_COORDINATION);
                    MessageStruct msg = listOfMessages.ElementAt(index);
                    
                    
                        listOfMessages.RemoveAt(index);

                        if (listOfMessages.Count == 0)
                        {
                            string responseMessageToUp = ConfigurationManager.AppSettings["CC" + numberCC] + "#" +
                                MessageNames.PEER_COORDINATION + "#" + "OK" + "#";
                            SendingMessage(response.ip, responseMessageToUp);
                        }
                    
                }
            }

        }


        public static void generateLogReceived(string nameReomteObject, string function)
        {
            Console.WriteLine("[" + Timestamp.generateTimestamp() + "]" + "CC received message " + function + " from " + nameReomteObject);
        }

    }
}
