using System;
using System.Globalization;
using System.Linq;

namespace NetworkingTools
{
    /// <summary>
    /// Klasa odpowiedzialna za stworzenie znacznika czasowego, używanego przy wyświetlaniu logów
    /// </summary>
    public class Timestamp
    {
        /// <summary>
        /// Funkcja służąca do stworzenia znacznika czasowego w momencie wywołania
        /// </summary>
        /// <returns>Zwraca wartość znacznika czasowego</returns>
        public static string generateTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

    }
}
