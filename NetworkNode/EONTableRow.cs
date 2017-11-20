using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Rząd tabeli z systemu EON.
    /// </summary>
    class EONTableRow
    {
        /// <summary>
        /// Na jakiej częstotliwości zaczyna się pasmo?
        /// </summary>
        public short busyFrequency { get; set; }

        /// <summary>
        /// Zajęte pasmo na wejściu węzła.
        /// </summary>
        public short busyBandIN { get; set; }
        
        /// <summary>
        /// Zajęte pasmo na wyjściu węzła.
        /// </summary>
        public short busyBandOUT { get; set; }
    }
}
