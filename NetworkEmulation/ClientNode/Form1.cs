using ClientNode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientNode
{
    public partial class Form1 : Form
    {
        //Niezbędne zmienne

        //Obiekt klienta
        private ClientApp client;

        //Obiekt związany z pobraniem aktualnego czasu
        private DateTime localtime;

        //Obiekt klasy Form1 - niezbędny w celu odwoływania się do pracy naszego ubiektu
        public Form1 _Form1;

       public Form1()
        {
            InitializeComponent();
            _Form1 = this;
            client = new ClientApp();
            comboBoxClients.Items.Add("Klient1");
            comboBoxClients.Items.Add("Klient2");
            comboBoxClients.Items.Add("Klient3");
            comboBoxClients.Items.Add("Klient4");

        }

        //Akcja po naciśnięciu przycisku wyślij
        private void buttonSend_Click(object sender, EventArgs e)
        {
            localtime = new DateTime();
            localtime = DateTime.Now;
            localtime.ToShortDateString();

            //Wiadomość, która będzie wysyłana do innego klienta
            string message = textBoxMessage.Text;
            //Cel wysyłania naszej wiadomości, pobrany z comboBoxa
            string destination = comboBoxClients.SelectedItem.ToString();

            //wywołanie czegoś na kształt: WyślijWiadomość(wiadomość, cel); na rzecz naszego klienta
            //Funkcja będzie posiadała funkcje przeszukująca tablicę, którą otrzyma od CHMURY
            //w celu znalezienia danych do nadania odpowiednich parametrów wiadomości

            textBoxLog.AppendText("[" + localtime + "] [TRYING TO SEND A MESSAGE] [ClientName]: " + destination + "\r\n");
        }

        
    }
}
