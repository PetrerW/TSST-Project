using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace NetworkingTools
{
    /// <summary>
    /// Klasa reprezentująca pakiet
    /// </summary>
    /// 
    /// <remarks>
    /// W sumie 64B
    /// 24B na naglowek
    /// 40B na informacje uzyteczna
    /// 
    /// Naglowek:
    /// port - 2B. Od 0 do 1 Jest zrodlowy, gdy slemy do chmury, a docelowy, gdy pakiet leci od chmury do routera
    /// IP celu - 4B. Od 2 do 5
    /// IP zrodla - 4B Od 6 do 9
    /// Nr pakietu  - 2B od 10 do 11
    /// Nr czestotliwosci - 2B od 12 do 13, short. nr = x => czestotliwosc = 193 000 GHz + x  * 12.5 GHz
    /// Pasmo - 2B od 14 do 15, short. Pasmo = 5 => pasmo = 12.5*5 [GHz]
    /// Dlugosc informacji uzytkowej - 2B od 16 do 17, short
    /// 6B od 18 do 23 - zarezerwowane na ewentualne przyszle rzeczy. Np. nr pakietu, gdy dzielimy pakiet 
    /// Informacja uzyteczna (domyslnie):
    /// Tekst + 
    /// timestamp. 
    /// 
    /// Wiadomość od i do NMSa
    /// 
    /// </remarks>

    public class Package
    {
        //Dlugosc pola informacji uzytkowej [B] 40
        public short usableInfoMaxLength { get; }

        //Dlugosc naglowka, [B]. 24
        public short headerMaxLength { get; }

        //numer portu, 2B
        public short portNumber;

        //adres IPv4 celu, 4B
        public IPAddress IP_Destination;

        //adres IPv4 źródła, 4B
        public IPAddress IP_Source;

        //nr pakietu. Zaczyna sie od 1
        public short packageNumber { get; set; }

        //czestotliwosciowy odpowiednik lambdy
        public short frequency { get; set; }

        //Pasmo zajmowane przez kanal do transmisji pakietu
        public short band { get; set; }

        //Wlasciwa dlugosc informacji uzytkowej, bez wypelnienia zerowymi bajtami
        public short usableInfoLength { get; set; }

        //6B zarezerwowane dodatkowo na przyszłe ficzery drugiego etapu

        //40-bajtowe pole z wiadomością użytkową
        public List<byte> usableInfoBytes { get; set; }

        //pole z wiadomością Ala ma kota
        public string usableMessage { get; set; }

        public List<byte> headerBytes { get; set; }


        /// <summary>
        /// Ustawia wielkosc pol pakietu, nr portu
        /// </summary>
        public Package()
        {
            usableInfoMaxLength = 40;
            headerMaxLength = 24;

            //domyslny numer portu
            portNumber = 1;

            //domyslny adres celu
            IP_Destination = IPAddress.Parse("127.0.0.1");

            //domyslny adres zrodla
            IP_Source = IPAddress.Parse("127.0.0.1");

            //pierwszy pakiet
            packageNumber = 1;

            //0 + 193 000 GHz...
            frequency = 0;

            //12.5GHz
            band = 1;

            //Ala ma kota  + czas danego dnia w milisekundach
            usableMessage =
                "Ala ma kora, a kor nie ma lai" + DateTime.Now.Date.TimeOfDay.TotalMilliseconds.ToString();

            //Dlugosc tego stringa
            usableInfoLength = (short)usableMessage.Length;

            //zapisz pola do list z bajtami
            this.toBytes();
        }

        /// <summary>
        /// Konstruktor z wiadomoscia uzytkowa.
        /// </summary>
        /// <param name="usableMessage"></param>
        public Package(string usableMessage) : this()
        {
            this.usableMessage = usableMessage;

            //Aktualizacja wartosci tablic z bajtami
            actualizeBytes();
        }

        /// <summary>
        /// Konstruktor z ustawianiem wszystkich naglowkow.
        /// </summary>
        /// <param name="usableMessage"></param>
        /// <param name="port"></param>
        /// <param name="IP_Source"></param>
        /// <param name="IP_Destination"></param>
        public Package(string usableMessage, short port, string IP_Destination, string IP_Source) : this()
        {
            this.usableMessage = usableMessage;
            this.portNumber = port;
            this.IP_Source = IPAddress.Parse(IP_Source);
            this.IP_Destination = IPAddress.Parse(IP_Destination);

            //Aktualizacja wartosci tablic z bajtami
            actualizeBytes();
        }

        /// <summary>
        /// Konstruktor ze wszystkimi parametrami z naglowka i wiadomoscia.
        /// </summary>
        /// <param name="usableMessage"></param>
        /// <param name="port"></param>
        /// <param name="IP_Source"></param>
        /// <param name="IP_Destination"></param>
        /// <param name="packageNumber"></param>
        /// <param name="frequency"></param>
        /// <param name="band"></param>
        /// <param name="usableInfoLength"></param>
        public Package(string usableMessage, short port, string IP_Destination, string IP_Source, short packageNumber,
            short frequency, short band, short usableInfoLength) : this()
        {
            this.usableMessage = usableMessage;
            this.portNumber = port;
            this.IP_Source = IPAddress.Parse(IP_Source);
            this.IP_Destination = IPAddress.Parse(IP_Destination);
            this.packageNumber = packageNumber;
            this.frequency = frequency;
            this.band = band;
            this.usableInfoLength = usableInfoLength;

            //Aktualizacja wartosci tablic z bajtami
            actualizeBytes();
        }

        /// <summary>
        /// Zapisuje pakiet w postaci tablicy bajtow.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] toBytes()
        {
            //zaktualizuj zawartosc list z bajtami
            actualizeBytes();

            var bytesList = new List<byte>(headerBytes);
            bytesList.AddRange(usableInfoBytes);

            return bytesList.ToArray();
        }

        /// <summary>
        /// Aktualizuje listy bajtow po zmianie jakiegos pola w obiekcie klasy Package.
        /// </summary>
        public void actualizeBytes()
        {
            //zapisanie nru portu w postaci tablicy bajtow
            byte[] portNumber_bytes = BitConverter.GetBytes(portNumber);

            //zmiana ip celu na bajty (4B)
            byte[] IP_Destination_bytes = IP_Destination.GetAddressBytes();

            //zmiana ip zrodla na bajty (4B)
            byte[] IP_Source_bytes = IP_Source.GetAddressBytes();

            //zmiana nru pakietu na bajty
            byte[] packageNumberBytes = BitConverter.GetBytes(packageNumber);

            //zmiana nru czestotliwosci na bajty
            byte[] frequencyBytes = BitConverter.GetBytes(frequency);

            //zmiana pasma na bajty
            byte[] bandBytes = BitConverter.GetBytes(band);

            //zamiana dlugosci informacji uzytkowej na bajty
            byte[] usableInfoLengthBytes = BitConverter.GetBytes(usableInfoLength);

            //jak headerBytes jest juz jakis niezerowy
            if (this.headerBytes != null)
                //to trzeba go wyzerowac
                this.headerBytes = null;

            this.headerBytes = new List<byte>();

            //Dodanie wszystkich pol w kolejnosci, w postaci bajtow, do listy bajtow
            headerBytes.AddRange(portNumber_bytes);
            headerBytes.AddRange(IP_Destination_bytes);
            headerBytes.AddRange(IP_Source_bytes);
            headerBytes.AddRange(packageNumberBytes);
            headerBytes.AddRange(frequencyBytes);
            headerBytes.AddRange(bandBytes);
            headerBytes.AddRange(usableInfoLengthBytes);

            //wypelnienie jej zerami
            headerBytes = fillBytesWIth0(headerMaxLength, headerBytes);

            //jak nie jest nullem, to trzeba to wyzerowac
            if (usableInfoBytes != null)
                usableInfoBytes = null;

            this.usableInfoBytes = new List<byte>();

            //dodanie wiadomosci do listy
            usableInfoBytes.AddRange(Encoding.ASCII.GetBytes(usableMessage));

            //uzupełnienie zerami
            usableInfoBytes = this.fillBytesWIth0(usableInfoMaxLength, usableInfoBytes);
        }

        /// <summary>
        /// Wypelnia liste bajtow zerami do okreslonego miejsca
        /// </summary>
        /// <param name="maxLength"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public List<byte> fillBytesWIth0(int maxLength, List<byte> bytes)
        {
            if (bytes.Count > maxLength)
            {
                throw new Exception("Bytes table is longer than destination size. Cannot shorten the list!");
                //zwroce bytes, bo jak przyrownamy do outputu funkcji to bytes sie nie zmieni
                return bytes;
            }
            for (int i = bytes.Count; i < maxLength; i++)
            {
                //dodaję 0000 0000, aż rozmiar listy byteów urośnie do 40
                bytes.Add(0x00);
            }

            return bytes;
        }

        /// <summary>
        /// Odnajduje i konwertuje bajty z pakietu na liczbe typu short.
        /// </summary>
        /// <param name="packageBytes"> Pakiet w postaci bajtów.</param>
        /// <returns>Nr portu z pakietu.</returns>
        public static short extractPortNumber(byte[] packageBytes)
        {
            //Nr portu sie zaczyna od indeksu 0 i ma dlugosc 2
            byte[] bytes = Package.extract(0, 2, packageBytes);

            //konwertuje bajty na shorta, 0 to indeks mowiacy, skad zaczac w tablicy
            return BitConverter.ToInt16(bytes, 0);
        }

        /// <summary>
        /// Wycina z tablicy bajtow te od docelowego IP i zamienia je na IPAddress.
        /// </summary>
        /// <param name="packageBytes"></param>
        /// <returns></returns>
        public static IPAddress exctractDestinationIP(byte[] packageBytes)
        {
            //Destination IP sie zaczyna na 2. pozycji i ma dlugosc 4B
            byte[] bytes = Package.extract(2, 4, packageBytes);

            //Konwersja na adres IP
            return new IPAddress(bytes);
        }

        /// <summary>
        /// Wycina kawałek z tablicy bajtow o okreslonej dlugosci w okreslonym miejscu.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="packageBytes"></param>
        /// <returns></returns>
        public static byte[] extract(short startIndex, short length, byte[] packageBytes)
        {

            byte[] bytes = new byte[length];

            int k = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                bytes[k] = packageBytes[i];
                k++;
            }

            return bytes;
        }

        /// <summary>
        /// Wycina z tablicy bajtow te od zrodlowego IP i zamienia je na IPAddress.
        /// </summary>
        /// <param name="packageBytes"></param>
        /// <returns></returns>
        public static IPAddress exctractSourceIP(byte[] packageBytes)
        {
            //Source IP sie zaczyna na 6. pozycji i ma dlugosc 4B
            byte[] bytes = Package.extract(6, 4, packageBytes);

            //Konwersja na adres IP
            return new IPAddress(bytes);
        }

        /// <summary>
        /// Wycina z tablicy bajtow i konwertuje na shorta nr pakietu.
        /// </summary>
        /// <param name="packageBytes"></param>
        /// <returns></returns>
        public static short extractPackageNumber(byte[] packageBytes)
        {
            //nr pakietu jest od 10 do 11 w tablicy
            byte[] bytes = Package.extract(10, 2, packageBytes);

            //konwertuje bajty na shorta, 0 to indeks mowiacy, skad zaczac w tablicy
            return BitConverter.ToInt16(bytes, 0);
        }

        /// <summary>
        /// Wycina z tablicy bajtow i konwertuje na shorta nr czestotliwosci.
        /// </summary>
        /// <param name="packageBytes"></param>
        /// <returns></returns>
        public static short extractFrequency(byte[] packageBytes)
        {
            //czestotliwosc jest od 12 do 13 w tablicy
            byte[] bytes = Package.extract(12, 2, packageBytes);

            //konwertuje bajty na shorta, 0 to indeks mowiacy, skad zaczac w tablicy
            return BitConverter.ToInt16(bytes, 0);
        }

        /// <summary>
        /// Wycina z tablicy bajtow i konwertuje na shorta pasmo.
        /// </summary>
        /// <param name="packageBytes"></param>
        /// <returns></returns>
        public static short extractBand(byte[] packageBytes)
        {
            //czestotliwosc jest od 12 do 13 w tablicy
            byte[] bytes = Package.extract(14, 2, packageBytes);

            //konwertuje bajty na shorta, 0 to indeks mowiacy, skad zaczac w tablicy
            return BitConverter.ToInt16(bytes, 0);
        }


        /// <summary>
        /// Wycina z tablicy bajtow i konwertuje na shorta dlugosc pola z informacja uzytkowa.
        /// </summary>
        /// <param name="packageBytes"></param>
        /// <returns></returns>
        public static short extractUsableInfoLength(byte[] packageBytes)
        {
            //czestotliwosc jest od 12 do 13 w tablicy
            byte[] bytes = Package.extract(16, 2, packageBytes);

            //konwertuje bajty na shorta, 0 to indeks mowiacy, skad zaczac w tablicy
            return BitConverter.ToInt16(bytes, 0);
        }

        /// <summary>
        /// Wycina z tablicy bajtow fragment zawierajacy wiadomosc uzytkowa i zamienia ja na stringa
        /// </summary>
        /// <param name="packageBytes"></param>
        /// <param name="usableInfoLength></param>
        /// <returns></returns>
        public static string extractUsableMessage(byte[] packageBytes, short usableInfoLength)
        {
            //Wiadomosc uzytkowa jest od 24. indeksu i ma dlugosc 40
            byte[] bytes = Package.extract(24, usableInfoLength, packageBytes);

            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Zmienia wartosc wiadomosci pakietu i podmienia ja w tablicy bajtow. UWAGA: Uzupelnia ja zerami!
        /// Nalezy podac tablice bajtow BEZ zer na koncu!
        /// </summary>
        /// <param name="usableMessageBytes"></param>
        public void changeMessage(byte[] usableMessageBytes)
        {

            //Jak tablica bajtow jest za duza, to rzuc wyjatek
            if (usableMessageBytes.Length > usableInfoMaxLength)
                throw new Exception("Package.changeMessage(byte[]): Dlugosc podanej tablicy bajtow jest wieksza, niz " + usableInfoMaxLength + " bajtow!");

            //wpisanie pola z wiadomoscia
            this.usableMessage = Encoding.ASCII.GetString(usableMessageBytes);

            //przypisanie dlugosci wiadomosci
            this.usableInfoLength = (short)this.usableMessage.Length;

            //aktualizacja z wpisanych pol
            this.actualizeBytes();
        }

        /// <summary>
        /// Zmienia wartosc wiadomosci uzytkowej i odpowiednie bajty
        /// </summary>
        /// <param name="message"></param>
        public void changeMessage(string message)
        {
            //Jak tablica bajtow jest za duza, to rzuc wyjatek
            if (message.Length > usableInfoMaxLength)
                throw new Exception("Package.changeMessage(string): Dlugosc podanego stringa bajtow jest wieksza, niz " + usableInfoMaxLength + " bajtow!");

            //przypisanie wiadomosci
            this.usableMessage = message;

            //przypisanie dlugosci wiadomosci
            this.usableInfoLength = (short)message.Length;

            //aktualizacja z gotowych wpisanych pol
            this.actualizeBytes();
        }

        public static byte[] PacketToByte(Package packet)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, packet);
            return ms.ToArray();
        }

        public static Package ByteToPacket(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            object o = (object)bf.Deserialize(ms);
            return (Package)o;
        }

    }
}
