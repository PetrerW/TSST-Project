﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace NetworkingTools.cs
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
    /// EON - 6B od 10 do 15
    /// 8B od 16 do 23 - zarezerwowane na ewentualne przyszle rzeczy. Np. nr pakietu, gdy dzielimy pakiet 
    /// 
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
        public short usableInfoLength { get; }

        //Dlugosc naglowka, [B]. 24
        public short headerLength { get; }

        //numer portu, 2B
        public short portNumber;

        //adres IPv4 celu, 4B
        public IPAddress IP_Destination;

        //adres IPv4 źródła, 4B
        public IPAddress IP_Source;

        //EON, 8B - zarezerwowane.
        
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
            usableInfoLength = 40;
            headerLength = 24;

            //domyslny numer portu
            portNumber = 1;
            
            //domyslny adres celu
            IP_Destination = IPAddress.Parse("127.0.0.1");

            //domyslny adres zrodla
            IP_Source = IPAddress.Parse("127.0.0.1");

            //Ala ma kota  + czas danego dnia w milisekundach
            usableMessage =
                "Ala ma kora, a kor nie ma lai" + DateTime.Now.Date.TimeOfDay.TotalMilliseconds.ToString();

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
            this.toBytes();
        }

        /// <summary>
        /// Konstruktor z ustawianiem wszystkich naglowkow.
        /// </summary>
        /// <param name="usableMessage"></param>
        /// <param name="port"></param>
        /// <param name="IP_Source"></param>
        /// <param name="IP_Destination"></param>
        public Package(string usableMessage, short port, string IP_Source, string IP_Destination) : this()
        {
            this.usableMessage = usableMessage;
            this.portNumber = port;
            this.IP_Source = IPAddress.Parse(IP_Source);
            this.IP_Destination = IPAddress.Parse(IP_Destination);

            //Aktualizacja wartosci tablic z bajtami
            this.toBytes();
        }

        /// <summary>
        /// Zapisuje pakiet w postaci tablicy bajtow.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] toBytes()
        {
            //zapisanie nru portu w postaci tablicy bajtow
            byte[] portNumber_bytes = BitConverter.GetBytes(portNumber);

            //zmiana ip celu na bajty (4B)
            byte[] IP_Destination_bytes = IP_Destination.GetAddressBytes();

            //zmiana ip zrodla na bajty (4B)
            byte[] IP_Source_bytes = IP_Source.GetAddressBytes();

            //jak headerBytes jest juz jakis niezerowy
            if (this.headerBytes != null)
                //to trzeba go wyzerowac 
                this.headerBytes = null;

            this.headerBytes = new List<byte>();


            //Dodanie portu i adresow w postaci bajtow do listy bajtow
            headerBytes.AddRange(portNumber_bytes);
            headerBytes.AddRange(IP_Destination_bytes);
            headerBytes.AddRange(IP_Source_bytes);

            //wypelnienie jej zerami
            headerBytes = fillBytesWIth0(headerLength, headerBytes);

            //jak nie jest nullem, to trzeba to wyzerowac
            if (usableInfoBytes != null)
                usableInfoBytes = null;

            this.usableInfoBytes = new List<byte>();
            //dodanie wiadomosci do listy
            usableInfoBytes.AddRange(Encoding.ASCII.GetBytes(usableMessage));

            //uzupełnienie zerami
            usableInfoBytes = this.fillBytesWIth0(usableInfoLength, usableInfoBytes);

            var bytesList = new List<byte>(headerBytes);
            bytesList.AddRange(usableInfoBytes);

            return bytesList.ToArray();
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
        public static short exctractPort(byte[] packageBytes)
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
            for (int i = startIndex; i < startIndex+length; i++)
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


    }
}
