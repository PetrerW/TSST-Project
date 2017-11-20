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
    class CommutationTableRow
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
    }
}
