using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using NetworkingTools;

namespace NetworkingTools
{

 public class SocketListener
    {
        Socket listener;
        private static bool listening = true;
        private readonly object _syncRoot = new object();
        private static IPEndPoint remoteEP;

        public SocketListener()
        {


        }


        public static bool Listening
        {
            get { return listening; }
            set { listening = value; }
        }


        public Socket ListenAsync(string adresIPListener, CancellationToken cancellationToken = default(CancellationToken))
        {
            Socket socketClient = null;
            Socket listener = null;
            IPAddress ipAddress =
                         ipAddress = IPAddress.Parse(adresIPListener);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
           

            cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    byte[] bytes = new Byte[1024];

                        // Create a TCP/IP socket.  
                        listener = new Socket(ipAddress.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);
                    if (!listener.IsBound)
                    {
                        Listening = true;
                        listener.Bind(localEndPoint);
                        listener.Listen(100);
                    }
    
                        cancellationToken.ThrowIfCancellationRequested();
                        //zadanie polegajace na rozpoznaniu kolejnego chcacego sie poloczyc klienta

                        socketClient = listener.Accept();
                        Console.WriteLine("Connected to new client");

                }
                catch (SocketException se)
                {
                    Console.WriteLine($"Socket Exception: {se}");
                }
                finally
                {
                    // StopListening();
                }
            if (socketClient==null)
            {
                return new Socket(ipAddress.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);
            }
            
            return socketClient;
        }

        public void StopListening()
        {
            if (Listening)
            {
                lock (_syncRoot)
                {
                    Listening = false;

                    try
                    {
                        if (listener.IsBound)
                            listener.Close();
                    }
                    catch (ObjectDisposedException)
                    {
                        Console.WriteLine("Cancelled the listener");
                    }
                }
            }
        }


        //oczekiwanie na wiadomosci w oddzielnym watku dla kazdego klienta, to sie da zrobic tez lepiej
        public string ProcessRecived(Socket client, CancellationToken cancellationToken = default(CancellationToken))
        {

            string tmp = "";

            cancellationToken.ThrowIfCancellationRequested();
           


                //to do zmiany oczywiscie, to bylo tylko przejsciowo testowane, bedziemy nasluchiwac do okreslonego warunkiem momentu

                //jezeli dane w strumieniu to wykonuj
                byte[] bytes = new byte[1024];
                int bytesRead = 0;
                try
                {
                    do
                {
                   
                        //Odczytywanie danych ze strumienia
                        bytesRead = client.Receive(bytes, 0, client.Available,

                           SocketFlags.None);
                        if (bytesRead > 0)
                        {
                            tmp = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                            bytesRead = 0;


                        }

                    

                } while (bytesRead > 0);
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine("Polaczenie zostalo zerwane");
                }
                catch (SocketException)
                {
                IPEndPoint ippoint = client.LocalEndPoint as IPEndPoint;
                Console.WriteLine("Brak dostepu do polaczenia na sockecie "+ippoint.ToString());
                client.Close();
                    
                }
                catch (Exception ioe)
                {
                    Console.WriteLine($"Read timed out: {ioe}");
                    bytesRead = 0;
                    
                }
                return tmp;

           
        }

        public byte[] ProcessRecivedBytes(Socket client, CancellationToken cancellationToken = default(CancellationToken))
        {
            IPEndPoint ippoint = client.LocalEndPoint as IPEndPoint;
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                byte[] buffer = new byte[64];
                byte[] package;
                int bytesRead = client.Receive(buffer);

                //   try
                //  {
                do
                {
                    package = new byte[64];
                    Array.Copy(buffer, package, bytesRead);
                    Console.WriteLine("We got: " + Package.exctractDestinationIP(package).ToString() + " " + Package.extractBand(package).ToString()
                         + " " + Package.extractFrequency(package).ToString() + " " + Package.extractPackageNumber(package).ToString()
                         + " " + Package.extractPortNumber(package).ToString() + " " + Package.extractUsableInfoLength(package).ToString()
                        + " " + Package.exctractSourceIP(package).ToString() + " " + Package.extractUsableMessage(package, Package.extractUsableInfoLength(package)));
                    bytesRead = 0;
                } while (bytesRead > 0);

                
                return package.ToArray();

            }
            catch (ObjectDisposedException)
                {
                    Console.WriteLine("Polaczenie zostalo zerwane");
                return null;
                }
                catch (SocketException)
                {
                
                Console.WriteLine("Brak dostepu do polaczenia na sockecie "+ippoint.ToString());
                client.Close();
                return null;
                    
                }
                catch (Exception ioe)
                {
                    Console.WriteLine($"Read timed out: {ioe}");
                return null;
                    
                }
            finally
            {

            }
        }
    }
}
