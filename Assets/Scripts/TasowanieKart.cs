using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TasowanieKart : NetworkBehaviour
{
    void Start()
    {
        //Debug.Log("Wywołanie Start() TasowanieKart.cs");
        // List<Karta> Karty_Uporzadkowane = new List<Karta>();
        // Karty_Uporzadkowane.AddRange(Singleton.Instance.UtworzoneKarty.KartyStartu);
        // Pokaż_Karty(Karty_Uporzadkowane);

        // List<Karta> Karty_Przetasowane = new List<Karta>();
        // Karty_Przetasowane = Przetasuj(Karty_Uporzadkowane);
        // Pokaż_Karty(Karty_Przetasowane);
    }

    public List<T> Przetasuj<T>(List<T> karty)
    {
        System.Random random = new System.Random();

        //List<T> przetasowaneKarty = new List<T>();
        List<T> bufor = new List<T>();

        int licznik = 0;
        foreach (T t in karty)
        {
            bufor.Add(t);
            // Debug.Log("karta dodana do bufora: " + licznik);
            licznik++;
        }

        karty.Clear();

        for (int i = bufor.Count - 1; i >= 0; i--)
        {
            int randomNumber = random.Next(0, i + 1);
            //Debug.Log("wylosowany numer: " + randomNumber);
            karty.Add(bufor[randomNumber]);
            //Debug.Log("karta dodana do listy o numerze: " + randomNumber);
            bufor.Remove(bufor[randomNumber]);
            //Debug.Log("karta usunieta z listy o numerze: " + randomNumber);
        }

        return karty;
    }

    public void Pokaż_Karty<T>(List<T> karty)
    {
        for (int i = 0; i < karty.Count; i++)
        {
            Debug.Log("Iteracja: " + i + "| Karta: " + karty[i]);
        }
    }

    public List<T> OdrzucKarty<T>(List<T> karty, int liczbaKartDoOdrzucenia)
    {
        int liczbaKart = karty.Count;
        for (int i = karty.Count; i > (liczbaKart - liczbaKartDoOdrzucenia); i--)
        {
            karty.Remove(karty[i - 1]);
            //Debug.Log("Karta nr "+ (i-1) + " odrzucona");
        }
        Debug.Log("karty.Count = " + karty.Count);
        return karty;
    }

    public void RozdajKartyDoGraniaGraczom<T>(List<T> karty, List<NetworkGamePlayerLobby> gracze)
    {
        int liczbaKartDoRozdaniaJednemuGraczowi = 0;
        if (gracze.Count < 7)
        {
            liczbaKartDoRozdaniaJednemuGraczowi = 5;
        }
        else if (gracze.Count >= 7)
        {
            liczbaKartDoRozdaniaJednemuGraczowi = 4;
        }

        foreach (NetworkGamePlayerLobby gracz in gracze)
        {

            int liczbaKartZeStosuDoGrania = karty.Count;
            for (int i = karty.Count; i > (liczbaKartZeStosuDoGrania - liczbaKartDoRozdaniaJednemuGraczowi); i--)
            {
                gracz.KartyNaRece.Add(karty[i - 1] as GameObject);
                Debug.Log("Próba przypisania authority graczowi nad kartą do grania (Ręka 4-5 kart)");
                (karty[i - 1] as GameObject).GetComponent<NetworkIdentity>().AssignClientAuthority(gracz.GetComponent<NetworkIdentity>().connectionToClient);
                karty.Remove(karty[i - 1]);
            }
        }
    }

    public GameObject DajGraczowiKarteZeStosuDoDobierania(NetworkGamePlayerLobby gracz, List<GameObject> listaKartStosDoDobierania)
    {
        int liczbaKartNaStosieDoDobierania = listaKartStosDoDobierania.Count;

        GameObject karta = listaKartStosDoDobierania[liczbaKartNaStosieDoDobierania - 1];

        gracz.KartyNaRece.Add(karta);
        karta.GetComponent<NetworkIdentity>().AssignClientAuthority(gracz.GetComponent<NetworkIdentity>().connectionToClient);
        listaKartStosDoDobierania.Remove(karta);
        Debug.Log($"Karta {karta.name} zostala przekazana graczowi do ręki");


        return karta;
    }

    public void RozdajKartyKrasnoludaGraczom(int liczbaKartDoRozdaniaJednemuGraczowi, List<GameObject> listaZKtorejBedaRozdawaneKarty, List<NetworkGamePlayerLobby> listaGraczy)
    {
        foreach (NetworkGamePlayerLobby gracz in listaGraczy)
        {
            int liczbaKartNaLisciePoczatkowej = listaZKtorejBedaRozdawaneKarty.Count;
            for (int i = listaZKtorejBedaRozdawaneKarty.Count; i > (liczbaKartNaLisciePoczatkowej - liczbaKartDoRozdaniaJednemuGraczowi); i--)
            {
                //Debug.Log($"Liczba kart na liscie Krasnoludow: = {listaZKtorejBedaRozdawaneKarty.Count} - w RozdajKartyGraczom()");
                gracz.KartaKrasnolud = listaZKtorejBedaRozdawaneKarty[i - 1];
                listaZKtorejBedaRozdawaneKarty[i - 1].GetComponent<NetworkIdentity>().AssignClientAuthority(gracz.GetComponent<NetworkIdentity>().connectionToClient);
                //Debug.Log($"Authority Karty Krasnoluda {listaZKtorejBedaRozdawaneKarty[i - 1].name} zostalo przypisane graczowi {gracz.GetDisplayName()}");
                listaZKtorejBedaRozdawaneKarty.Remove(listaZKtorejBedaRozdawaneKarty[i - 1]);
            }
        }
    }

    public void UsunKrasnoludyZTalii(int iloscKrasnoludowZKazdegoKlanuDoUsuniecia, List<GameObject> listaKartKrasnoludowZKtorejKrasnaleZostanaUsuniete, List<GameObject> listaKartKrasnoludowPoKtorejBedzieForeach)
    {
        //Debug.Log("UsunKrasnoludyZTalii()");
        int usunieteLojalneZolteKrasnale = 0;
        int usunieteLojalneNiebieskieKrasnale = 0;
        foreach (GameObject kartaKrasnoluda in listaKartKrasnoludowPoKtorejBedzieForeach)
        {
            KartaKrasnoluda.TypKrasnoluda typKrasnoluda = (kartaKrasnoluda.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKrasnoluda;
            KartaKrasnoluda.TypKlanu typKlanu = (kartaKrasnoluda.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu;

            if (typKrasnoluda == KartaKrasnoluda.TypKrasnoluda.Lojalny && typKlanu == KartaKrasnoluda.TypKlanu.Niebieski && usunieteLojalneNiebieskieKrasnale < iloscKrasnoludowZKazdegoKlanuDoUsuniecia)
            {
                listaKartKrasnoludowZKtorejKrasnaleZostanaUsuniete.Remove(kartaKrasnoluda);
                NetworkServer.Destroy(kartaKrasnoluda);
                usunieteLojalneNiebieskieKrasnale++;
            }

            if (typKrasnoluda == KartaKrasnoluda.TypKrasnoluda.Lojalny && typKlanu == KartaKrasnoluda.TypKlanu.Żółty && usunieteLojalneZolteKrasnale < iloscKrasnoludowZKazdegoKlanuDoUsuniecia)
            {
                listaKartKrasnoludowZKtorejKrasnaleZostanaUsuniete.Remove(kartaKrasnoluda);
                NetworkServer.Destroy(kartaKrasnoluda);
                usunieteLojalneZolteKrasnale++;
            }
        }
    }

    public void UsunPionkiKtoreSaNiepotrzebne(List<NetworkGamePlayerLobby> listaGraczy, List<GameObject> listaPionkowZoltych, List<GameObject> listaPionkowNiebieskich)
    {
        int liczbaPionkowZoltych = 5;
        int liczbaPionkowNiebieskich = 5;
        int liczbaGraczyZoltych = 0;
        int liczbaGraczyNiebieskich = 0;
        foreach (NetworkGamePlayerLobby gracz in listaGraczy)
        {
            if ((gracz.KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu == KartaKrasnoluda.TypKlanu.Żółty)
            {
                liczbaGraczyZoltych++;
            }

            if ((gracz.KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu == KartaKrasnoluda.TypKlanu.Niebieski)
            {
                liczbaGraczyNiebieskich++;
            }
        }
        //int liczbaPionkowZoltychDoUsuniecia = liczbaPionkowZoltych - liczbaGraczyZoltych;
        //int liczbaPionkowNiebieskichDoUsuniecia = liczbaPionkowNiebieskich - liczbaGraczyNiebieskich;

        for (int i = liczbaPionkowZoltych - 1; i >= liczbaGraczyZoltych; i--)
        {
            GameObject pionek = listaPionkowZoltych[i];
            listaPionkowZoltych.Remove(listaPionkowZoltych[i]);
            NetworkServer.Destroy(pionek);
        }

        for (int i = liczbaPionkowNiebieskich - 1; i >= liczbaGraczyNiebieskich; i--)
        {
            GameObject pionek = listaPionkowNiebieskich[i];
            listaPionkowNiebieskich.Remove(listaPionkowNiebieskich[i]);
            NetworkServer.Destroy(pionek);
        }
    }

    public void RozdajGraczomPionki(List<NetworkGamePlayerLobby> listaGraczy, List<GameObject> listaPionkowZoltych, List<GameObject> listaPionkowNiebieskich)
    {
        int rozdanePionkiZolte = 0;
        int rozdanePionkiNiebieskie = 0;

        foreach (NetworkGamePlayerLobby gracz in listaGraczy)
        {
            if ((gracz.KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu == KartaKrasnoluda.TypKlanu.Żółty)
            {
                gracz.PionekKrasnolud = listaPionkowZoltych[rozdanePionkiZolte];
                gracz.PionekKrasnolud.GetComponent<PionekInfo>().NazwaGraczaTextMeshPro_text = gracz.GetDisplayName();
                listaPionkowZoltych[rozdanePionkiZolte].GetComponent<NetworkIdentity>().AssignClientAuthority(gracz.GetComponent<NetworkIdentity>().connectionToClient);
                rozdanePionkiZolte++;
            }

            if ((gracz.KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu == KartaKrasnoluda.TypKlanu.Niebieski)
            {
                gracz.PionekKrasnolud = listaPionkowNiebieskich[rozdanePionkiNiebieskie];
                gracz.PionekKrasnolud.GetComponent<PionekInfo>().NazwaGraczaTextMeshPro_text = gracz.GetDisplayName();
                listaPionkowNiebieskich[rozdanePionkiNiebieskie].GetComponent<NetworkIdentity>().AssignClientAuthority(gracz.GetComponent<NetworkIdentity>().connectionToClient);
                rozdanePionkiNiebieskie++;
            }
        }
    }

}
