using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkNode;

namespace AISDE
{
    public class Wezel
    {
        protected int identyfikatorWezla;
        protected int wspolrzednaX;
        protected int wspolrzednaY;
        protected int liczbaKlientow;
        protected Wezel doMniePrzez;
        protected float etykieta;
        protected bool odwiedzony;
        protected List<Lacze> doprowadzoneKrawedzie = new List<Lacze>();
        public string ip = String.Empty;
        public EONTable eonTable;
       
        public Wezel()
        {
            identyfikatorWezla = 0;
            wspolrzednaX = 0;
            wspolrzednaY = 0;
            doMniePrzez = null;
            etykieta = 0;
            odwiedzony = false;
            ip = "";
            doprowadzoneKrawedzie = new List<Lacze>();
            eonTable = new EONTable();
        }

        public Wezel(int identyfikatorWezla, int wspolrzednaX, int wspolrzednaY,int liczbaKlientow):this()
        {
            this.identyfikatorWezla = identyfikatorWezla;
            this.wspolrzednaX = wspolrzednaX;
            this.wspolrzednaY = wspolrzednaY;
            this.liczbaKlientow = liczbaKlientow;
        }

        public Wezel(int identyfikatorWezla, int wspolrzednaX, int wspolrzednaY):this()
        {
            this.identyfikatorWezla = identyfikatorWezla;
            this.wspolrzednaX = wspolrzednaX;
            this.wspolrzednaY = wspolrzednaY;
        }

        public Wezel(int identyfikatorWezla, int wspolrzednaX, int wspolrzednaY, string ip):this()
        {
            this.identyfikatorWezla = identyfikatorWezla;
            this.wspolrzednaX = wspolrzednaX;
            this.wspolrzednaY = wspolrzednaY;
            this.ip = ip;
        }

        public Wezel(int id, string ip):this()
        {
            this.identyfikatorWezla = id;
            this.ip = ip;
        }

        public Wezel(int id, string ip, EONTable table):this()
        {
            this.identyfikatorWezla = id;
            this.ip = ip;
            this.eonTable = table;
        }

        public int LKlientow
        {
            get {return liczbaKlientow; }
            set {this.liczbaKlientow=value; }

        }

        public int idWezla
        {
            get { return identyfikatorWezla; }
            set { identyfikatorWezla = value; }
        }

        public int wspX
        {
            get { return wspolrzednaX; }
            set { wspolrzednaX = value; }
        }

        public int wspY
        {
            get { return wspolrzednaY; }
            set { wspolrzednaY = value; }
        }

        public bool Odwiedzony
        {
            get { return odwiedzony; }
            set { odwiedzony = value; }
        }
    

    
        public Wezel NajlepiejPrzez
        {
            get { return doMniePrzez; }
            set { doMniePrzez = value; }
        }

        public float Etykieta
        {
            get { return etykieta; }
            set { etykieta = value; }
        }

        public void wprowadzenieIndeksowKrawedzi(Lacze ktore)
        {
            doprowadzoneKrawedzie.Add(ktore);
        }
        public List<Lacze> listaKrawedzi
        {
            get { return doprowadzoneKrawedzie; }
        }
       
    }
}
