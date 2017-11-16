using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTools.cs
{
    /// <summary>
    /// Klasa reprezentująca pakiet
    /// </summary>
    public class Package
    {
        //48-bajtowe pole z wiadomością użytkową
        public byte[] bytes { get; set; }

        //Pozostałe 16 bajtów
        public byte[] headerBytes { get; set; }

        public string usableMessage { get; set; }

        public Package()
        {
            //Ala ma kota  + czas danego dnia w milisekundach
            usableMessage =
                "Ala ma kora, a kor nie ma lai" + DateTime.Now.Date.TimeOfDay.TotalMilliseconds.ToString();

            bytes = Encoding.ASCII.GetBytes(usableMessage);
            
            headerBytes = new byte[16];
        }
    }
}
