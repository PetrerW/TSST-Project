using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkNode;

namespace AISDE
{
    public class Lacze
    {
        protected int identyfikatorKrawedzi;
        protected int wezelpierwszy;
        protected int wezeldrugi;

        //~Piotrek 
        protected Wezel WezelPierwszy;
        protected Wezel WezelDrugi;
        //

        protected float waga;

        public Lacze(int _identyfikatorKrawedzi, int _wezelpierwszy, int _wezeldrugi)
        {
            this.identyfikatorKrawedzi = _identyfikatorKrawedzi;
            this.wezelpierwszy = _wezelpierwszy;
            this.wezeldrugi = _wezeldrugi;
            this.waga = 0;
        }

        public Lacze(int _identyfikatorKrawedzi, Wezel _WezelPierwszy, Wezel _WezelDrugi, int waga)
        {
            this.identyfikatorKrawedzi = _identyfikatorKrawedzi;
            this.WezelPierwszy = _WezelPierwszy;
            this.WezelDrugi = _WezelDrugi;
            this.wezelpierwszy = _WezelPierwszy.idWezla;
            this.wezeldrugi = _WezelDrugi.idWezla;

            short band = (short) (EONTable.capacity - Math.Sqrt(waga));

            //Jezeli jest jeszcze miejsce
            if (EONTable.capacity >= Math.Sqrt(this.Waga))
            {
                //Przypisanie nowego kosztu
                this.waga = waga;

                //Na wyjsciu wezla 1
                this.Wezel1.eonTable.addRow(new EONTableRowOut(0, (short)(EONTable.capacity - band)));
                //I na wejsciu wezla 2
                this.Wezel2.eonTable.addRow(new EONTableRowIN(0, (short)(EONTable.capacity - band)));
            }
        }

        public Lacze(int _identyfikatorKrawedzi, Wezel _WezelPierwszy, Wezel _WezelDrugi, short band, short frequency)
        {
            this.identyfikatorKrawedzi = _identyfikatorKrawedzi;
            this.WezelPierwszy = _WezelPierwszy;
            this.WezelDrugi = _WezelDrugi;
            this.wezelpierwszy = _WezelPierwszy.idWezla;
            this.wezeldrugi = _WezelDrugi.idWezla;

            if (band <= EONTable.capacity)
            {
                //Na wyjsciu wezla 1
                this.Wezel1.eonTable.addRow(new EONTableRowOut(frequency, band));
                //I na wejsciu wezla 2
                this.Wezel2.eonTable.addRow(new EONTableRowIN(frequency, band));
                this.waga = (float)Math.Pow(band, 2);
            }
        }

        public int idKrawedzi
        {
            get { return identyfikatorKrawedzi; }
            set { identyfikatorKrawedzi = value; }
        }

        //~Piotrek
        public Wezel Wezel1
        {
            get { return WezelPierwszy; }
            set { WezelPierwszy = value; }
        }

        public Wezel Wezel2
        {
            get { return WezelDrugi; }
            set { WezelDrugi = value; }
        }
        //

        public int wezel1
        {
            get { return wezelpierwszy; }
            set { wezelpierwszy = value; }
        }

        public int wezel2
        {
            get { return wezeldrugi; }
            set { wezeldrugi = value; }
        }
        public float Waga
        {
            get { return waga; }
            set { waga = value; }
        }
    }
}