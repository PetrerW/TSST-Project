using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Rząd tabeli kierowania pakietów dla węzłów NIEbrzegowych. 
    /// </summary>
    public class CommutationTableRow
    {
        /// <summary>
        /// Tunelem o jakiej częstotliwości przyszedł pakiet?
        /// Liczby naturalne od 0 do 63.
        /// </summary>
        public short frequency_in { get; set; }

        /// <summary>
        /// Na jaki port skierować pakiet?
        /// </summary>
        public short port_out { get; set; }

        /// <summary>
        /// Na jaki adres IP skierować pakiet?
        /// </summary>
        public IPAddress IP_OUT { get; set; }

        public CommutationTableRow()
        {
            //empty
        }

        /// <summary>
        /// Konstruktor ze wszystkimi parametrami.
        /// </summary>
        /// <param name="frequency_in"></param>
        /// <param name="port_out"></param>
        /// <param name="IP_OUT"></param>
        public CommutationTableRow(short frequency_in, short port_out, string IP_OUT)
        {
            this.frequency_in = frequency_in;
            this.port_out = port_out;
            this.IP_OUT = IPAddress.Parse(IP_OUT);
        }
    }
}
