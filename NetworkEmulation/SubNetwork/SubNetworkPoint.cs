using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using NetworkNode;

namespace SubNetwork
{
    /// <summary>
    /// Status SNPa. ACTIVE - istniejący w rzeczywistości
    /// POTENTIAL - nieistniejący, ale może istnieć i wymaga stworzenia przez LRM
    /// </summary>
    public enum STATE { AVAILABLE = 1, POTENTIAL = 2 }

    public class SubNetworkPoint
    {
        public EONTable eonTable = new EONTable();
        public STATE state;
        public IPAddress ipaddress;
        public int portIN;
        public int portOUT;

        public SubNetworkPoint()
        {
            this.ipaddress = IPAddress.Parse("127.0.0.1");
            this.state = STATE.AVAILABLE;
            this.portIN = 11000;
            this.portOUT = 11000;
        }

        public SubNetworkPoint(IPAddress ipaddress): this()
        {
            this.ipaddress = ipaddress;
        }

        public SubNetworkPoint(IPAddress ipaddress, STATE state)
        {
            this.ipaddress = ipaddress;
            this.state = state;
        }

        public SubNetworkPoint(IPAddress ipaddress,STATE state,int port )
        {
            this.ipaddress = ipaddress;
            this.portIN = port;
            this.state = state;
        }
        public SubNetworkPoint(IPAddress ipaddress, int port):this()
        {
            this.ipaddress = ipaddress;
            this.portIN = port;
        }

        public SubNetworkPoint(IPAddress ipaddress, int portIN, int portOUT, STATE state)
        {
            this.ipaddress = ipaddress;
            this.portIN = portIN;
            this.portOUT = portOUT;
            this.state = state;
        }

        public SubNetworkPoint(IPAddress ipaddress, int portIN, int portOUT)
        {
            this.ipaddress = ipaddress;
            this.portIN = portIN;
            this.portOUT = portOUT;
            
        }
    }
}

