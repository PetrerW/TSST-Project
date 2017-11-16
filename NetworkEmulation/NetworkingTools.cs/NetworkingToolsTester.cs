using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Project->add reference->assemblies->extensions->Mircosoft.VisualStudio.QualityTools.UnitTestFramework, version 10.1.0.0
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkingTools.cs
{
    /// <summary>
    /// Klasa do testowania NetworkingTools
    /// </summary>
    [TestClass]
    public class NetworkingToolsTester
    {

        //test konstruktora klasy Package.cs
        [TestMethod]
        public void packageConstructorTest1()
        {
            Package P = new Package();
            Assert.AreEqual(48,P.bytes.Length);
        }
    }

}
