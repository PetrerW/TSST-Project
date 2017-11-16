using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingTools.cs
{
    /// <summary>
    /// Zestaw klas z publicznie dostępnymi funkcjami
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            Program P = new Program();
            P.runUnitTests();
        }

        public void runUnitTests()
        {
            NetworkingToolsTester NTT = new NetworkingToolsTester();
            NTT.packageConstructorTest1();
            Console.ReadKey();
        }
    }
}
