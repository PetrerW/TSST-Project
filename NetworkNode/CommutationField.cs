using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkingTools;

namespace NetworkNode
{
    /// <summary>
    /// Klasa reprezentujaca pole komutacyjne.
    /// </summary>
    public class CommutationField
    {
        //Maksymalny rozmiar bufora wejsciowego
        public short maxBuffInSize;

        //Maksymalny rozmiar bufora wyjsciowego
        public short maxBuffOutSize;

        //Bufor wejsciowy
        public volatile Buffer bufferIn;

        //Lista wyjsciowych buforow
        public volatile List<Buffer> BuffersOut;

        //Referencja na tablice komutacji brzegowego wezla sieciowego
        public volatile BorderNodeCommutationTable borderNodeCommutationTable;

        //Referencja na tablice komutacji wezla sieciowego
        public volatile CommutationTable commutationTable;

        //Referencja na tablice eonowa wezla sieciowego
        public volatile EONTable EonTable;


        public NetworkNode _networkNode;

        /// <summary>
        /// Konstruktor 
        /// TODO: Kazdy router ma na razie 3 bufory wyjsciowe
        /// </summary>
        public CommutationField()
        {
            bufferIn = new Buffer();
            BuffersOut = new List<Buffer>(3);
        }

        /// <summary>
        /// Konstruktor okreslajacy ilosc buforow wyjsciowych
        /// </summary>
        /// <param name="buffersOutCount"></param>
        public CommutationField(int buffersOutCount)
        {
            bufferIn = new Buffer();
            BuffersOut = new List<Buffer>(buffersOutCount);
        }

        /// <summary>
        /// Konstruktor z referencjami na tablice i liczba wyjsciowych buforow
        /// </summary>
        /// <param name="borderNodeCommutationTable"></param>
        /// <param name="commutationTable"></param>
        /// <param name="EonTable"></param>
        public CommutationField(ref BorderNodeCommutationTable borderNodeCommutationTable,
            ref CommutationTable commutationTable, ref EONTable EonTable, int buffersOutCount)
        {
            bufferIn = new Buffer();
            BuffersOut = new List<Buffer>(buffersOutCount);
            this.borderNodeCommutationTable = borderNodeCommutationTable;
            this.EonTable = EonTable;
            this.commutationTable = commutationTable;
        }

        /// <summary>
        /// Funkcja przetwarzajaca pakiety
        /// </summary>
        /// <param name="packageBytes"></param>
        public async Task<List<Queue<byte[]>>> processPackage(byte[] packageBytes)
        {
            //jezeli bufor jest pelny...
            if (bufferIn.queue.Count == maxBuffInSize)
            {
                //Wyprozniamy bufor wejsciowy i zapisujemy jego stan do tego listOfQUeues
                var listOfQueues = bufferIn.emptyBuffer();

                //do zwrotu wartosci
                List<Queue<byte[]>> returnListOfQueues;

                //ale listOfQueues powinno miec zawsze 3 kolejki na I etapie...
                for (int i = 0; i < listOfQueues.Count; i++)
                {
                    //Jak jeden pakiet ma ustawiona czestotliwosc na -1, to jest szansa, ze wszystkie maja i 
                    //Jezeli wszystkie pakiety posiadaja -1 w czestotliwosci
                    if (Package.extractFrequency(listOfQueues[i].Peek()) == -1 &&
                        Buffer.checkIfAllPackagesFrequency(listOfQueues[i], -1))
                    {
                        listOfQueues[i] = new Queue<byte[]>(_networkNode.borderNodeCommutationTable.
                            changeHeaderForMessagesFromClient(listOfQueues[i]));

                        //Nowa, zmieniona czestotliwosc. 
                        //TODO: Na razie jest jedna, a na drugi etap trzeba bedzie przygotowac sortowanie, dzielenie i 
                        //TODO: dodawanie do konkretnych kolejek

                        //Podpatrujemy na jakas czestotliwosc z pakietu
                        short freq = Package.extractFrequency(listOfQueues[i].Peek());

                        //sprawdzamy, czy wszystkie czestotliwosci sa takie same
                        if (Package.extractFrequency(listOfQueues[i].Peek()) == freq &&
                            Buffer.checkIfAllPackagesFrequency(listOfQueues[i], freq))
                        {
                            //szukanie takiej kolejki, ze wierzchni wpis ma interesujaca nas czestotliwosc
                            var queue = listOfQueues.Find(q => Package.extractFrequency(q.Peek()) == freq);

                            //Gdy nie udalo sie znalezc - kolejka z pakietami o czestotliwosci freq zostaje na swoim miejscu na liscie. Nalezy dodac bufor wyjsciowy do pola
                            //BuffersOut.Add(new Buffer(listOfQueues[i].Count));

                            //A gdy nie udalo sie znalezc, to dodajemy elementy kolejki z wczesniejszymi "-1" do kolejki z freq
                            if (queue != null)
                            {
                                while (listOfQueues[i].Count > 0)
                                {
                                    //Zdejmowanie z jednej kolejki i dopisywanie do drugiej. Nie trzeba znowu sortowac - pakiety z 
                                    //"-1" maja prawdopodobnie inne pochodzenie
                                    queue.Enqueue(listOfQueues[i].Dequeue());
                                }
                            }
                        }
                        else
                        {
                            //Rzucamy wyjatek gdy czestotliwosci nie sa takie same
                            throw new Exception("CommutationField.processPackage(): Zamienione czestotliwosci z -1 na inna nie sa takie same!");
                        }
                    }

                    //Kopiowanie z podkolejek do kolejnych buforow wyjsciowych
                    BuffersOut[i].queue = new Queue<byte[]>(listOfQueues[i]);

                    //sortowanie kolejek wedlug ID
                    BuffersOut[i].queue = Buffer.sortQueueByID(ref BuffersOut[i].queue);

                    //Lista podkolejek o pakietach o tym samym ID
                    var listOfSameIDQueues = Buffer.divideSortedQueueByID(ref BuffersOut[i].queue);

                    //Kazda z tych podkolejek sortuje wedlug numeru pakietu
                    for (int j = 0; j < listOfSameIDQueues.Count; j++)
                    {
                        var temp = listOfSameIDQueues[i];
                        //Sortowanie po numerze pakietu
                        listOfSameIDQueues[i] = new Queue<byte[]>(Buffer.sortQueueByPackageNumber(ref temp));
                    }

                    //Sklejanie kolejek z powrotem
                    var joinedQueue = new Queue<byte[]>(Buffer.joinSortedByPackageNumberQueues(listOfSameIDQueues));

                    //TODO: Zmien to. Trzeba zajrzec w tablice komutacji i skierowac na odpowiedni bufor.
                    //BuffersOut[i].queue = joinedQueue;

                    //Wpisanie naglowkow do kolejki z pakietami
                    joinedQueue = commutationTable.changeHeaderForMessagesFromClient(joinedQueue);

                    //Zagladamy na jeden port (wszystkie powinny byc takie same)
                    short port = Package.extractPortNumber(joinedQueue.Peek());

                    //
                    switch (port)
                    {
                        case 1:
                            BuffersOut[0].queue = joinedQueue;
                            break;
                        case 2:
                            BuffersOut[1].queue = joinedQueue;
                            break;
                        case 3:
                            BuffersOut[2].queue = joinedQueue;
                            break;
                        default:
                            BuffersOut[0].queue = joinedQueue;
                            break;
                    }

                }

                //Tworzenie z buforow listy kolejek i wysterowanie ich na wyjscie
                returnListOfQueues = new List<Queue<byte[]>>() { BuffersOut[0].queue, BuffersOut[1].queue, BuffersOut[2].queue};

                //wyprozniamy bufor
                return returnListOfQueues;
            }
            else
            {
                //dodanie do bufora pakietu
                bufferIn.queue.Enqueue(packageBytes);
                return null;
            }
        }
    }
}
