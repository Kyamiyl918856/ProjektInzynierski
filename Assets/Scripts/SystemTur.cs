using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static KartaKrasnoluda;

public class SystemTur : NetworkBehaviour
{
    private NetworkGamePlayerLobby gracz = null;
    private CardManager cardManager = null;

    bool czyZakonczonoGre = false;

    // Stany:
    // Zagranie lub odrzucenie karty - 1
    // Ruch krasnoludem - 2
    // Dobranie Kart - 3
    [Header("Stan")]
    [SyncVar]
    [SerializeField]
    private int stan = 1;
    public int Stan
    {
        get { return stan; }
        set { stan = value; }
    }

    [Header("ZaganieLubOdrzucenie")]
    [SyncVar]
    [SerializeField]
    private int zagranieLubOdrzucenie = 0;
    public int ZagranieLubOdczucenie
    {
        get { return zagranieLubOdrzucenie; }
        set { zagranieLubOdrzucenie = value; }
    }

    [Header("RuchKrasnoludem")]
    [SyncVar]
    [SerializeField]
    private int ruchKrasnoludem = 0;
    public int RuchKrasnoludem
    {
        get { return ruchKrasnoludem; }
        set { ruchKrasnoludem = value; }
    }

    [Header("WzieteKartySkarbu")]
    [SyncVar]
    [SerializeField]
    private int wzieteKartySkarbu = 0;
    public int WzieteKartySkarbu
    {
        get { return wzieteKartySkarbu; }
        set { wzieteKartySkarbu = value; }
    }

    [Header("DobranieKart")]
    [SyncVar]
    [SerializeField]
    private int dobranieKart = 0;
    public int DobranieKart
    {
        get { return dobranieKart; }
        set { dobranieKart = value; }
    }

    [Header("IleKartZostaloJuzOdrzuconych")]
    [SyncVar]
    [SerializeField]
    private int ileKartZostaloJuzOdrzuconych = 0;
    public int IleKartZostaloJuzOdrzuconych
    {
        get { return ileKartZostaloJuzOdrzuconych; }
        set { ileKartZostaloJuzOdrzuconych = value; }
    }



    [SyncVar]
    [SerializeField]
    private int ileRuchowPionkiemJeszczeZostalo = 3; // lub 0 , zobaczê jeszcze
    public int IleRuchowPionkiemJeszczeZostalo
    {
        get { return ileRuchowPionkiemJeszczeZostalo; }
        set { ileRuchowPionkiemJeszczeZostalo = value; }
    }

    [SyncVar]
    [SerializeField]
    private string czyWczytanoIloscRuchowPionkiemDoWykorzystania = "false";
    public string CzyWczytanoIloscRuchowPionkiemDoWykorzystania
    {
        get { return czyWczytanoIloscRuchowPionkiemDoWykorzystania; }
        set { czyWczytanoIloscRuchowPionkiemDoWykorzystania = value; }
    }



    [Header("nrKolejnoscGracza")]
    [SyncVar]
    [SerializeField]
    private int nrKolejnoscGracza = 0;
    public int NrKolejnoscGracza
    {
        get { return nrKolejnoscGracza; }
    }

    [SerializeField] private List<NetworkGamePlayerLobby> kolejnoscGraczy = new List<NetworkGamePlayerLobby>();
    public List<NetworkGamePlayerLobby> ListaKolejnoscGraczy
    {
        get { return kolejnoscGraczy; }
    }

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public NetworkGamePlayerLobby Gracz
    {
        get { return gracz; }
    }

    [ServerCallback]
    private void Update()
    {
        SprawdzCzyZakonczycGre();
        RpcWypiszKomunikatNaKonsoli($"Tura gracza: {kolejnoscGraczy[nrKolejnoscGracza].GetDisplayName()}");

        if (zagranieLubOdrzucenie == 0) { return; }
        if (ruchKrasnoludem == 0) { return; }
        if (dobranieKart == 0) { return; }

        gracz = kolejnoscGraczy[nrKolejnoscGracza];

        zagranieLubOdrzucenie = 0;
        ruchKrasnoludem = 0;
        czyWczytanoIloscRuchowPionkiemDoWykorzystania = "false";
        dobranieKart = 0;
        stan = 1;
        ileKartZostaloJuzOdrzuconych = 0;
        wzieteKartySkarbu = 0;

        if (nrKolejnoscGracza < kolejnoscGraczy.Count - 1)
        {
            nrKolejnoscGracza++;
        }
        else
        {
            nrKolejnoscGracza = 0;
        }

        gracz.OdswiezTabeleGraczyUi();

    }

    private void SprawdzCzyZakonczycGre()
    {
        if (czyZakonczonoGre == true) { return; }

        int liczbaKartNaRekachGraczy = 0;
        foreach (NetworkGamePlayerLobby gracz in kolejnoscGraczy)
        {
            liczbaKartNaRekachGraczy+= gracz.KartyNaRece.Count;
        }


        if (!(cardManager.getListaKartSkarbuDoBrania().Count < 1 || (liczbaKartNaRekachGraczy < 1 && cardManager.getStosKartDoDobierania().Count < 1)))
        {
            return;
        }



        // logika przeliczania punktów graczy i przekazania ich do funkcji RpcWyswietlWynikiGraczom
        int liczbaGraczyZoltych = 0;
        int liczbaGraczyNiebieskich = 0;

        float liczbaPunktowZoltych = 0;
        float liczbaPunktowNiebieskich = 0;

        float liczbaPunktowNaJednegoZoltego = 0;
        float liczbaPunktowNaJednegoNiebieskiego = 0;

        foreach (NetworkGamePlayerLobby gracz in ListaKolejnoscGraczy)
        {
            KartaKrasnoluda.TypKrasnoluda typKrasnoludaGracza = (gracz.KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKrasnoluda;
            KartaKrasnoluda.TypKlanu typKlanuGracza = (gracz.KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu;

            if ((typKrasnoludaGracza == KartaKrasnoluda.TypKrasnoluda.Lojalny && typKlanuGracza == KartaKrasnoluda.TypKlanu.¯ó³ty) || (typKrasnoludaGracza == KartaKrasnoluda.TypKrasnoluda.Sabota¿ysta && typKlanuGracza == KartaKrasnoluda.TypKlanu.Niebieski))
            {
                liczbaGraczyZoltych++;

                foreach (GameObject kartaSkarbuGameObject in gracz.KartySkarbu)
                {
                    KartaSkarbu kartaSkarbu = (kartaSkarbuGameObject.GetComponent<CardData>().ScriptableKarta as KartaSkarbu);

                    liczbaPunktowZoltych += (float)kartaSkarbu.Wartosc;
                }
            }
            else if ((typKrasnoludaGracza == KartaKrasnoluda.TypKrasnoluda.Lojalny && typKlanuGracza == KartaKrasnoluda.TypKlanu.Niebieski) || (typKrasnoludaGracza == KartaKrasnoluda.TypKrasnoluda.Sabota¿ysta && typKlanuGracza == KartaKrasnoluda.TypKlanu.¯ó³ty))
            {
                liczbaGraczyNiebieskich++;

                foreach (GameObject kartaSkarbuGameObject in gracz.KartySkarbu)
                {
                    KartaSkarbu kartaSkarbu = (kartaSkarbuGameObject.GetComponent<CardData>().ScriptableKarta as KartaSkarbu);

                    liczbaPunktowNiebieskich += (float)kartaSkarbu.Wartosc;
                }
            }
        }

        liczbaPunktowNaJednegoZoltego = liczbaPunktowZoltych / liczbaGraczyZoltych;
        liczbaPunktowNaJednegoNiebieskiego = liczbaPunktowNiebieskich / liczbaGraczyNiebieskich;

        foreach (NetworkGamePlayerLobby gracz in ListaKolejnoscGraczy)
        {
            KartaKrasnoluda.TypKrasnoluda typKrasnoludaGracza = (gracz.KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKrasnoluda;
            KartaKrasnoluda.TypKlanu typKlanuGracza = (gracz.KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu;

            if (typKlanuGracza == KartaKrasnoluda.TypKlanu.¯ó³ty && typKrasnoludaGracza != KartaKrasnoluda.TypKrasnoluda.Samolub)
            {
                gracz.SetIloscPunktow(liczbaPunktowNaJednegoZoltego);
            }
            else if (typKlanuGracza == KartaKrasnoluda.TypKlanu.Niebieski && typKrasnoludaGracza != KartaKrasnoluda.TypKrasnoluda.Samolub)
            {
                gracz.SetIloscPunktow(liczbaPunktowNaJednegoNiebieskiego);
            }
            else
            {
                float iloscPunktow = 0;
                foreach (GameObject kartaSkarbuGameObject in gracz.KartySkarbu)
                {
                    KartaSkarbu kartaSkarbu = (kartaSkarbuGameObject.GetComponent<CardData>().ScriptableKarta as KartaSkarbu);

                    iloscPunktow += (float)kartaSkarbu.Wartosc;
                }

                if(gracz.KartySkarbu.Count < 1)
                {
                    gracz.SetIloscPunktow(gracz.GetIloscPunktow() + 0);
                }
                else
                {
                    gracz.SetIloscPunktow(gracz.GetIloscPunktow() + iloscPunktow);
                }

                
            }
        }

        GameObject[] gracze = new GameObject[kolejnoscGraczy.Count];
        string[] nazwyGraczy = new string[kolejnoscGraczy.Count];
        string[] klanyGraczy = new string[kolejnoscGraczy.Count];
        string[] klasyGraczy = new string[kolejnoscGraczy.Count];
        float[] punktyGraczy = new float[kolejnoscGraczy.Count];

        for (int i = 0; i < kolejnoscGraczy.Count; i++)
        {
            //gracze[i] = kolejnoscGraczy[i].gameObject;
            
            nazwyGraczy[i] = kolejnoscGraczy[i].GetDisplayName();
            klanyGraczy[i] = (kolejnoscGraczy[i].KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu.ToString();
            klasyGraczy[i] = (kolejnoscGraczy[i].KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKrasnoluda.ToString();
            punktyGraczy[i] = kolejnoscGraczy[i].GetIloscPunktow();

            Debug.Log($"{nazwyGraczy[i]}");
            Debug.Log($"{klanyGraczy[i]}");
            Debug.Log($"{klasyGraczy[i]}");
            Debug.Log($"{punktyGraczy[i]}");

        }

        RpcWyswietlWynikiGraczom(nazwyGraczy, klanyGraczy, klasyGraczy, punktyGraczy);

        czyZakonczonoGre = true;
    }

    [ClientRpc]
    public void RpcWyswietlWynikiGraczom(/*GameObject[] gracze*/ string[] nazwyGraczy, string[] klanyGraczy, string[] klasyGraczy, float[] punktyGraczy)
    {
        UI_DisplayPlayerInfo uI_DisplayPlayerInfo = GameObject.Find("UI_Display").GetComponent<UI_DisplayPlayerInfo>();

        uI_DisplayPlayerInfo.GetPanelWyniki().SetActive(true);


        int i = 0;
        foreach (List<TMP_Text> gracz_TMP_Text in uI_DisplayPlayerInfo.GetListaGraczyPodsumowanie())
        {
            if (i >= nazwyGraczy.Length) { break; }
            //gracz_TMP_Text[0].text = gracze[i].GetComponent<NetworkGamePlayerLobby>().GetDisplayName();
            //gracz_TMP_Text[1].text = (gracze[i].GetComponent<NetworkGamePlayerLobby>().KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu.ToString();
            //gracz_TMP_Text[2].text = (gracze[i].GetComponent<NetworkGamePlayerLobby>().KartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKrasnoluda.ToString();
            //gracz_TMP_Text[3].text = gracze[i].GetComponent<NetworkGamePlayerLobby>().GetIloscPunktow().ToString();

            Debug.Log($"{nazwyGraczy[i]}");
            Debug.Log($"{klanyGraczy[i]}");
            Debug.Log($"{klasyGraczy[i]}");
            Debug.Log($"{punktyGraczy[i]}");

            gracz_TMP_Text[0].text = nazwyGraczy[i];
            gracz_TMP_Text[1].text = klanyGraczy[i];
            gracz_TMP_Text[2].text = klasyGraczy[i];
            gracz_TMP_Text[3].text = punktyGraczy[i].ToString();
            i++;
        }

    }

    public override void OnStartServer()
    {
        kolejnoscGraczy.AddRange(Room.GamePlayers);
        Room.TasowanieKart.Przetasuj<NetworkGamePlayerLobby>(kolejnoscGraczy);
        Debug.Log("Kolejnoœæ graczy zosta³a ustalona");
        int i = 0;
        foreach (NetworkGamePlayerLobby gracz in kolejnoscGraczy)
        {
            Debug.Log($"Gracz {gracz.GetDisplayName()}");
            gracz.NrKolejnosci = i;
            i++;
        }
        gracz = kolejnoscGraczy[0];

        stan = 1;

        cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();

        //foreach (NetworkGamePlayerLobby gracz in Room.GamePlayers)
        //{
        //    ZnajdzSystemTur();
        //}
    }

    //[ClientRpc]
    //public void ZnajdzSystemTur()
    //{
    //    gracz.ZnajdzSystemTur();
    //}



    [ClientRpc]
    private void RpcWypiszKomunikatNaKonsoli(string komunikat)
    {
        Debug.Log(komunikat);
    }


    public void SetIleRuchowPionkiemJeszczeZostalo(int value)
    {
        ileRuchowPionkiemJeszczeZostalo = value;
    }

    public string ZwrocKomunikatJakiKrokWTurze()
    {
        if (stan == 1)
        {
            return "Zagraj kartê lub odrzuæ 1-2 karty";
        }
        else if (stan == 2)
        {
            return $"Przesuñ swój pionek. Dostêpne ruchy: {ileRuchowPionkiemJeszczeZostalo}";
        }
        else if (stan == 3)
        {
            return "Dobieram karty...";
        }
        else
        {
            return "ERROR";
        }
    }
}
