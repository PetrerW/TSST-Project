using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Klasa zawierająca tablicę komutacji pakietów w węzłach na brzegu sieci.
    /// </summary>
    class BorderNodeCommutationTableRow
    {
        /// <summary>
        /// Jakim adresem IP przyszedł pakiet?
        /// </summary>
        public IPAddress IP_IN { get; set; }

        /// <summary>
        /// Jakim portem przyszedł pakiet?
        /// </summary>
        public short port_in { get; set; }

        /// <summary>
        /// Jakie pasmo przypisać temu łączu?
        /// </summary>
        public short band { get; set; }

        /// <summary>
        /// Jaką częstotliwość przypisać temu łączu? 
        /// </summary>
        public short frequency { get; set; }

        /// <summary>
        /// Jaką wydajnośc modulacji nadać łączu?
        /// </summary>
        public short modulationPerformance { get; set; }

        /// <summary>
        /// Jaką prędkość bitową nadać temu łączu? 
        /// </summary>
        public short bitRate { get; set; }

        /// <summary>
        /// Na jaki adres chmury przesłać pakiet?
        /// </summary>
        public IPAddress IPSocketOUT { get; set; }

        /// <summary>
        /// Na jaki port przeslać pakiet?
        /// </summary>
        public short socketPort { get; set; }

        public BorderNodeCommutationTableRow()
        {
            //empty
        }




    }
}
