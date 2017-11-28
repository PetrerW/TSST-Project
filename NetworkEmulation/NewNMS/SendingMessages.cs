using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NewNMS
{
    class SendingMessages
    {
        
            Application _Application;

            public static void SendingPackageBytes(Socket socket, byte[] msg)
            {
                try
                {
                    // byte[] byteData = msg;

                    socket.Send(msg);

                }
                catch (SocketException)
                {

                }
                catch (Exception)
                {

                }
            }
        
    }
}