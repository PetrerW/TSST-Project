using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /// <summary>
    /// Tablica komutacji routera, także brzegowego.
    /// </summary>
    public class CommutationTable
    {
        public List<CommutationTableRow> Table { get; set; }
    }
}
