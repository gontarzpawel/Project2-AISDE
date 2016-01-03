﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmyGrafowe
{
    class PoprawianieEtykiet:IWyznaczanieSciezek
    {
        private Queue<int> fifo;
        private List<string> sciezki;
        public Siec siec;
        public double[] kosztSciezki;
        public Wezel[] wezly;
        public Lacze[] lacza;
        private double nieskonczonosc = Double.PositiveInfinity;
        private int wskaznik_ilosci_dostepnych_drog;


        public PoprawianieEtykiet()
        {
            fifo = new Queue<int>();
            wskaznik_ilosci_dostepnych_drog = 0;
            sciezki = new List<string>();
        }
        public string sciezkaMiedzyWezlami(int wezelPocz, int wezelKoncowy) 
        {
            Lacze[] lacza_current;
            int wezelKonca,wezelPoczatku;
            int tmp;
            string sciezka = "Najgrubsza sciezka z wierzcholka " + wezelPocz + " do wierzcholka " + wezelKoncowy + " : ";

            #region Reset danych
            for (int i = 1; i <= siec.liczbaWezlow; i++) //Reset wezlow
            {
                wezly[i].etykieta = 0;
            }
            siec.tablica_wierzcholkow = new int[siec.liczbaWezlow + 1]; //Reset tablicy wierzchołków
            for (int i = 0; i <= siec.liczbaWezlow; i++)
            {
                siec.tablica_wierzcholkow[i] = 0;
            }
            siec.liczba_odwiedzonych_wezlow = 0;
            #endregion

            wezly[wezelPocz].etykieta = nieskonczonosc;
            fifo.Enqueue(wezelPocz);
            while(fifo.Count != 0 && wezly[wezelKoncowy].odwiedzone == false) // warunek konca poszukiwania najgrubszej sciezki 
            //?? czy przypadkiem nie trzeba sprawdzac do oproznienia FIFO BARTEK?
            {
                tmp = fifo.Dequeue();
                wezly[tmp].odwiedzone = true;
                lacza_current = zwracamPolaczenia(tmp); //zwracam możliwe drogi z danego wezla do innych
                siec.tablica_wierzcholkow[siec.liczba_odwiedzonych_wezlow] = tmp;   //odnotowuje drogę

                lacza_current = sortowanie(lacza_current); //sortowanie w celu sprawdzania najpierw najgrubszych polaczen
                while(wskaznik_ilosci_dostepnych_drog != 1) //dopóki istnieja zwrócone wcześniej drogi, zmieniam etykiety
                {
                    wezelKonca = lacza_current[wskaznik_ilosci_dostepnych_drog - 1].wezelKoncowy;
                    wezelPoczatku = lacza_current[wskaznik_ilosci_dostepnych_drog - 1].wezelPoczatkowy;
                    if(wezly[wezelKonca].etykieta <= Math.Min(lacza_current[wskaznik_ilosci_dostepnych_drog - 1].koszt, wezly[wezelPoczatku].etykieta))
                    {
                        wezly[wezelKonca].etykieta = Math.Min(lacza_current[wskaznik_ilosci_dostepnych_drog - 1].koszt, wezly[wezelPoczatku].etykieta);
                        fifo.Enqueue(wezelKonca);
                    }
                    wskaznik_ilosci_dostepnych_drog--;
                }
                siec.liczba_odwiedzonych_wezlow++;
                sciezka += tmp + " ";
            }
            //foreach (int element in siec.tablica_wierzcholkow)
            //Console.WriteLine(element);

            sciezki.Add(sciezka);
            Console.ReadLine();
            return sciezka;
        }

        private Lacze[] sortowanie(Lacze[] lacze)
        {
            int j = 1;
            while (j != wskaznik_ilosci_dostepnych_drog - 1)
            {
                j++;
                for (int i = 1; i < wskaznik_ilosci_dostepnych_drog - 1; i++)
                {
                    if (lacze[i].koszt.CompareTo(lacze[i + 1].koszt) > 0)
                    {
                        Lacze temp;
                        temp = lacze[i + 1];
                        lacze[i + 1] = lacze[i];
                        lacze[i] = temp;
                    }
                }
            }

            return lacze;
        } //sortuje wagi laczy, zeby zaczynac zawsze od najgrubszej (mysle, ze to ma znaczenie w FIFO)
        private Lacze[] zwracamPolaczenia(int indeksPoczatku)
        {
            int j = 1;
            Lacze[] polaczenia_z_poczatkowym = new Lacze[siec.liczbaKrawedzi + 1];
            for(int i = 0; i < siec.liczbaKrawedzi; i++)
            {
                if(lacza[i + 1].wezelPoczatkowy == indeksPoczatku)
                {
                    polaczenia_z_poczatkowym[j] = lacza[i + 1];
                    j++;
                }
            }
            wskaznik_ilosci_dostepnych_drog = j;
            return polaczenia_z_poczatkowym;
        } //funkcja odpowiadajaca za zwracanie polaczen danego wezla z pozostalymi
        
        public string sciezkaMiedzyWszystkimiWezlami(int wezelPocz) //wszystkie sciezki od danego wezla poczatkowego do kazdego wezla
        {
            string sciezka = "";

            for (int i = 1; i < siec.liczbaWezlow; i++)
            {
                sciezka += sciezkaMiedzyWezlami(wezelPocz, i);
            }
            return sciezka;
        }
    }
}