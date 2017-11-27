using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Klasa z tabelą komutacji węzłów brzegowych.
    /// </summary>
    public class BorderNodeCommutationTable
    {
        /// <summary>
        /// Tabela komutacji węzłów brzegowych
        /// </summary>
        public List<BorderNodeCommutationTableRow> Table { get; set; }
    }
}
