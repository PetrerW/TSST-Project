using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            ControlConnection cc = new ControlConnection("1");
            cc.ReceivedMessage();
        }
    }
}
