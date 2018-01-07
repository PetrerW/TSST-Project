using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCC
{
    public class Configuration
    {
        public void ReadingFromConfigFile(string path)
        {
            string line;
            char[] delimiterChars = { '#' };
            string[] words;
            try
            {
                using (StreamReader file = new StreamReader(path))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        words = line.Split(delimiterChars);
                        if (words[0].StartsWith("interface_listen"))
                        {
                            NCC.InterfaceToListen.Add(words[0],words[1]);
                        }
                        else
                        {
                            NCC.InterfaceToSend.Add(words[0],words[1]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot read from configuration file");
            }
        }
    }
}
