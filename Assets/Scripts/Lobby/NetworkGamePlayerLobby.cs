using JetBrains.Annotations;
using Mirror;
using Mirror.RemoteCalls;
using System;
using System.Collections.Generic;
using UnityEngine;


public class NetworkGamePlayerLobby : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    private string displayName = "Loading...";

    [SyncVar]
    [SerializeField]
    private string czyMoznaWykonacFunkcje = "false";

    [SyncVar]
    [SerializeField]
    private string czyMoznaWczytacKarteKrasnoluda = "false";

    [SyncVar]
    [SerializeField]
    private string czyMoznaWczytacPionek = "false";

    [SyncVar]
    [SerializeField]
    private string czyMoznaWczytacPionkiReszcieGraczy = "false";

    [SyncVar]
    [SerializeField]
    private string czyMoznaWczytacCardManager = "false";

    [SyncVar]
    [SerializeField]
    private int kartyWczytaneDoListy = 0;

    [Header("nrKolejności")]
    [SyncVar]
    [SerializeField]
    private int nrKolejnosci = 0;
    public int NrKolejnosci
    {
        get { return nrKolejnosci; }
        set { nrKolejnosci = value; }
    }

    // UI Informacje o graczu w prawym dolnym rogu ekranu
    UI_DisplayPlayerInfo uI_DisplayPlayerInfo = null;

    [SerializeField] private ZasadyGry zasadyGry = null;
    [SerializeField] private SystemTur systemTur = null;
    public SystemTur SystemTur
    {
        get { return systemTur; }
        set { systemTur = value; }
    }


    // karty, pionek
    [SerializeField] private List<GameObject> kartyNaRece = new List<GameObject>();
    [SerializeField] private GameObject kartaNiesieciowaPrefab = null;
    [SerializeField] private GameObject pionekKrasnolud = null;
    [SerializeField] private GameObject kartaKrasnolud = null;
    [SerializeField] private GameObject znacznikKrasnolud = null;
    [SerializeField] private List<GameObject> kartySkarbu = new List<GameObject>();

    [SyncVar]
    [SerializeField]
    private float punkty = 0;

    // bool stany pomocnicze
    bool czyCardManagerWczytany = false;
    bool czyKartyOdwroconeAwersemDoGory = false;
    bool czyNazwaGraczaUI_ustawiona = false;
    bool czyKartaKrasnoludaOdwroconaAwersemDoGory = false;
    bool czyDaneZKartyKrasnoludaWczytaneDoUI = false;
    bool czyNazwyGraczyWczytaneDoPionkow = false;
    bool czyTabelaGraczyOdswiezona = false;

    [SerializeField] private bool czyKartyWlaczone = false;

    private NetworkManagerLobby room;
    public NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public List<GameObject> KartyNaRece
    {
        get { return kartyNaRece; }
    }

    public List<GameObject> KartySkarbu
    {
        get { return kartySkarbu; }
    }

    public GameObject KartaKrasnolud
    {
        get { return kartaKrasnolud; }
        set { kartaKrasnolud = value; }
    }

    public GameObject PionekKrasnolud
    {
        get { return pionekKrasnolud; }
        set { pionekKrasnolud = value; }
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public int GetKartyWczytaneDoListy()
    {
        return kartyWczytaneDoListy;
    }

    public float GetIloscPunktow()
    {
        return punkty;
    }

    public void SetIloscPunktow(float value)
    {
        punkty = value;
    }

    public void SetKartyWczytaneDoListy()
    {
        this.kartyWczytaneDoListy = 1;
    }

    public void SetCzyMoznaWczytacCardManager(string value)
    {
        czyMoznaWczytacCardManager = value;
    }

    public void SetTrueCzyMoznaWykonacFunkcje(string value)
    {
        this.czyMoznaWykonacFunkcje = value;
    }

    public void SetCzyMoznaWczytacKarteKrasnoluda(string value)
    {
        this.czyMoznaWczytacKarteKrasnoluda = value;
    }

    public void SetCzyMoznaWczytacPionek(string value)
    {
        this.czyMoznaWczytacPionek = value;
    }

    public void SetCzyMoznaWczytacPionkiReszcieGraczy(string value)
    {
        this.czyMoznaWczytacPionkiReszcieGraczy = value;
    }




    [ClientCallback]
    private void Update()
    {
        if (isLocalPlayer && uI_DisplayPlayerInfo == null)
        {
            GameObject UI_Display = GameObject.Find("UI_Display");

            if (UI_Display != null)
            {
                uI_DisplayPlayerInfo = UI_Display.GetComponent<UI_DisplayPlayerInfo>();
            }
        }

        if (isLocalPlayer && kartaKrasnolud != null && uI_DisplayPlayerInfo != null && !czyDaneZKartyKrasnoludaWczytaneDoUI)
        {
            Debug.Log("---------------USTAWIENIA INTERFEJSU UI -----------------------");
            uI_DisplayPlayerInfo.SetUI_NazwaGracza(this.displayName);
            Debug.Log($"Nazwa gracza: {this.displayName}");
            uI_DisplayPlayerInfo.SetUI_KlanGracza((kartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu.ToString());
            Debug.Log($"Klan Gracza: {(kartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKlanu.ToString()}");
            uI_DisplayPlayerInfo.SetUI_KlasaGracza((kartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKrasnoluda.ToString());
            Debug.Log($"Klasa Gracza: {(kartaKrasnolud.GetComponent<CardData>().ScriptableKarta as KartaKrasnoluda)._TypKrasnoluda.ToString()}");

            uI_DisplayPlayerInfo.GetButtonPominRuchPionkiem().onClick.AddListener(() => zasadyGry.CmdPrzyciskPominRuchPionka());
            uI_DisplayPlayerInfo.GetButtonOdrzucKarte().onClick.AddListener(() => zasadyGry.OdrzucKarte());
            uI_DisplayPlayerInfo.GetButtonDalej().onClick.AddListener(() => zasadyGry.PrzejdzDalej());
            uI_DisplayPlayerInfo.GetButtonPowrotDoMenu().onClick.AddListener(() => zasadyGry.PowrotDoMenu());



            czyDaneZKartyKrasnoludaWczytaneDoUI = true;
        }

        if (isLocalPlayer && uI_DisplayPlayerInfo != null && zasadyGry != null)
        {
            uI_DisplayPlayerInfo.SetUI_WybranaKarta(ZwrocOpisKarty(zasadyGry.KartaKliknieta));


            if (systemTur != null && !czyTabelaGraczyOdswiezona)
            {
                CmdOdswiezTabeleGraczyUI();    // tu nie bede wykonywal tej funkcji, wykonam ją w UstawKrokTuryNaWykonany
                czyTabelaGraczyOdswiezona = true;
            }
        }

        //if (isLocalPlayer && uI_DisplayPlayerInfo != null && zasadyGry.KartaKliknieta != null)
        //{
        //    uI_DisplayPlayerInfo.SetUI_WybranaKarta(ZwrocOpisKarty(zasadyGry.KartaKliknieta));
        //}

        //// do usunieca ten if
        //if (isLocalPlayer && !czyNazwaGraczaUI_ustawiona)
        //{
        //    GameObject UI_Display = GameObject.Find("UI_Display");
        //    if (UI_Display != null)
        //    {
        //        UI_Display.GetComponent<UI_DisplayPlayerInfo>().SetUI_NazwaGracza(this.displayName);
        //        czyNazwaGraczaUI_ustawiona = true;
        //    }
        //}



        if (isLocalPlayer && czyMoznaWczytacKarteKrasnoluda == "true")
        {
            //Debug.Log("Powinna się wykonać funkcja ZnadzMojaKarteKrasnoluda()");
            ZnajdzMojaKarteKrasnoluda();
        }

        if (isLocalPlayer && kartaKrasnolud != null && !czyKartaKrasnoludaOdwroconaAwersemDoGory)
        {
            kartaKrasnolud.GetComponent<CardData>().OdkryjKarte();
            czyKartaKrasnoludaOdwroconaAwersemDoGory = true;
        }

        if (isLocalPlayer && kartaKrasnolud != null && czyMoznaWczytacPionek == "true")
        {
            ZnajdzMojPionek();
        }

        if (isLocalPlayer && czyMoznaWczytacPionkiReszcieGraczy == "true" && !czyNazwyGraczyWczytaneDoPionkow)
        {
            CmdWczytajNazwyGraczyDoPionkow();
            CmdUstawPionkomKartyNaKtorychStojaIKartomPrzypiszPionki();
        }

        //if (isLocalPlayer && Room.CardManager == null)
        //{
        //    WczytajCardManager();
        //}

        if (isLocalPlayer && systemTur == null)
        {
            //Debug.Log("Powinna się wykonać funkcja ZnajdzSystemTur()");
            ZnajdzSystemTur();
        }

        if (isLocalPlayer && czyMoznaWykonacFunkcje == "true")
        {
            //Debug.Log($"Karty na rece = {kartyNaRece.Count} poczatek update()");

            //Debug.Log($"Tu LocalPlayer - {this.displayName}");

            if (kartyNaRece.Count > 4 && !czyKartyOdwroconeAwersemDoGory)
            {
                SetKartyWczytaneDoListy();
                GameManager.Instance.OdwrocKartyNaReceAwersemDoGory(kartyNaRece);
                Debug.Log("Karty na rece zostaly odkryte teraz je widac ich awers - NetworkGamePlayerLobby.cs");
                czyKartyOdwroconeAwersemDoGory = true;
            }

            //Debug.Log($"czyMoznaWykonacFunkcje wartosc {czyMoznaWykonacFunkcje}");
            //Debug.Log($"displayName: {displayName}");
            if (czyMoznaWykonacFunkcje == "true")
            {
                //Debug.Log("CzyMoznaWykonacFunkcje = true  weszlismy do ifa");
                for (int i = 0; i < 63; i++)
                {
                    GameObject gameObject = GameObject.Find($"Karta ({i})(Clone)");
                    if (gameObject != null)
                    {
                        //Debug.Log($"znaleziono karte: {gameObject.name} - {this.displayName}");
                        if (gameObject.GetComponent<NetworkIdentity>().hasAuthority)
                        {
                            //Debug.Log($"mam authority nad kartą {gameObject.name} - NetworkGamePlayerLobby.cs - {this.displayName} - conectionId:");
                            if (kartyNaRece.Count < 1)
                            {
                                kartyNaRece.Add(gameObject);
                            }
                            else
                            {
                                if (!kartyNaRece.Contains(gameObject))
                                {
                                    kartyNaRece.Add(gameObject);
                                }
                            }

                        }
                        else
                        {
                            //Debug.Log($"BRAK authority nad kartą {gameObject.name} - NetworkGamePlayerLobby.cs");
                            if (isClientOnly)
                            {
                                gameObject.SetActive(false);
                            }
                            //Debug.Log($"Karta zostala wylaczona {gameObject.name}");
                            if (isServer)
                            {
                                if (!kartyNaRece.Contains(gameObject))
                                {
                                    gameObject.GetComponent<BoxCollider>().enabled = false;
                                    gameObject.GetComponent<MeshRenderer>().enabled = false;
                                    // znajdz awers i rewers i wylacz
                                    gameObject.transform.GetChild(0).gameObject.SetActive(false);
                                    gameObject.transform.GetChild(1).gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Debug.Log("Nie znaleziono karty, gameobject = null");
                    }

                }

                foreach (GameObject karta in kartyNaRece)
                {
                    karta.SetActive(true);
                    //Debug.Log("Karta zostala wlaczona w foreach");
                }

                for (int i = 0; i < 5; i++)
                {
                    //Debug.Log($"Teraz nastapi ulozenie kart na rece liczba kart do rozlozenia {kartyNaRece.Count}");
                    kartyNaRece[i].transform.position = GameManager.Instance.SpawnPointsKartyNaRece[i].transform.position;
                }

            }

            //Debug.Log($"Karty na rece {kartyNaRece.Count}");

            CmdSetCzyMoznaWykonacFunkcje("false");
        }

        if (isLocalPlayer && systemTur != null)
        {
            //Debug.Log("SystemTur został wczytany");
        }

        // petla gry
        if (isLocalPlayer && systemTur != null)
        {
            if (nrKolejnosci == systemTur.NrKolejnoscGracza)
            {
                Debug.Log("Petla Gryy");
                switch (systemTur.Stan)
                {
                    case 1:
                        czyTabelaGraczyOdswiezona = false;
                        zasadyGry.RaycastKartaIPolePlanszy();
                        if (systemTur.IleKartZostaloJuzOdrzuconych > 0) { break; }
                        Debug.Log("zasadyGry.PolozKarteNaPoluPlanszy()");
                        zasadyGry.PolozKarteNaPoluPlanszy();
                        zasadyGry.ZagrajKarteNarzedzia();
                        break;

                    case 2:
                        Debug.Log($"systemTur.Stan = {systemTur.Stan} , Krok 2 W switchu Ruch Pionkiem");
                        if (systemTur.CzyWczytanoIloscRuchowPionkiemDoWykorzystania == "false")
                        {
                            if ((pionekKrasnolud.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje.GetComponent<CardData>().ScriptableKarta as KartaSciezki)._TypSciezki == KartaSciezki.TypSciezki.Ognisko)
                            {
                                CmdSetIloscRuchowPionkiemDoWykorzystania(5);
                                Debug.Log($"Ilosc Ruchow Pionkiem zostala ustawiona na = 5");
                            }
                            else
                            {
                                CmdSetIloscRuchowPionkiemDoWykorzystania(3);
                                Debug.Log($"Ilosc Ruchow Pionkiem zostala ustawiona na = 3");
                            }

                            CmdSetCzyWczytanoIloscRuchowPionkiemDoWykorzystania("true");
                        }
                        else
                        {
                            Debug.Log("zasadyGry.PrzesunPionekKrasnoluda()");
                            Debug.Log($"Ile ruchow pozostało : {systemTur.IleRuchowPionkiemJeszczeZostalo}");
                            zasadyGry.ZbierzKarteSkarbu();
                            zasadyGry.PrzesunPionekKrasnoluda();
                        }

                        break;

                    case 3:
                        // dobranie kart
                        zasadyGry.DobierzKarty();
                        CmdOdswiezTabeleGraczyUI();
                        break;
                }
            }
        }
    }



    [Command]
    private void CmdSetIloscRuchowPionkiemDoWykorzystania(int value)
    {
        systemTur.IleRuchowPionkiemJeszczeZostalo = value;
    }

    [Command]
    private void CmdSetCzyWczytanoIloscRuchowPionkiemDoWykorzystania(string value)
    {
        systemTur.CzyWczytanoIloscRuchowPionkiemDoWykorzystania = value;
    }

    [Command]
    public void CmdSetCzyMoznaWykonacFunkcje(string value)
    {
        SetTrueCzyMoznaWykonacFunkcje(value);
    }

    [Command]
    public void CmdSetCzyMoznaWczytacKarteKrasnoluda(string value)
    {
        SetCzyMoznaWczytacKarteKrasnoluda(value);
    }

    [Command]
    public void CmdSetCzyMoznaWczytacPionek(string value)
    {
        SetCzyMoznaWczytacPionek(value);
    }


    [Command]
    public void CmdWczytajNazwyGraczyDoPionkow()
    {
        foreach (NetworkGamePlayerLobby gracz in Room.GamePlayers)
        {
            RpcWczytajNazwyGraczyDoPionkow(gracz.GetDisplayName(), gracz.pionekKrasnolud.name);
        }
    }

    [ClientRpc]
    public void RpcWczytajNazwyGraczyDoPionkow(string nazwaGracza, string pionekGameObjectName)
    {
        GameObject pionek = GameObject.Find($"{pionekGameObjectName}");
        foreach (NetworkGamePlayerLobby gracz in Room.GamePlayers)
        {
            if (gracz.GetDisplayName() == nazwaGracza)
            {
                gracz.pionekKrasnolud = pionek;
                gracz.pionekKrasnolud.GetComponent<PionekInfo>().NazwaGraczaTextMeshPro_text = nazwaGracza;
                //lub
                // pionek.getComponent<PionekInfo>().NazwaGraczaTextMeshPro_text = nazwaGracza;
            }
        }
        czyNazwyGraczyWczytaneDoPionkow = true;
    }

    [Command]
    public void CmdUstawPionkomKartyNaKtorychStojaIKartomPrzypiszPionki()
    {
        foreach (NetworkGamePlayerLobby gracz in Room.GamePlayers)
        {
            RpcUstawPionkomKartyNaKtorychStojaIKartomPrzypiszPionki(gracz.pionekKrasnolud.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje.name, gracz.pionekKrasnolud.name);
        }
    }

    [ClientRpc]
    public void RpcUstawPionkomKartyNaKtorychStojaIKartomPrzypiszPionki(string nazwaKartyObozuStartu, string nazwaPionka)
    {
        GameObject kartaObozuStartu = GameObject.Find($"Talia_KartyStartu/{nazwaKartyObozuStartu}");
        GameObject pionek = GameObject.Find($"{nazwaPionka}");

        (kartaObozuStartu.GetComponent<CardData>().ScriptableKarta as KartaSciezki).ListaPionkowStojacychNaTejKarcie.Add(pionek);
        pionek.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje = kartaObozuStartu;
    }

    //[Command]
    //public void UtworzKartyMonoBehaviourNaRece(List<GameObject> listaKartNaRece)
    //{
    //    Debug.Log("WlaczKartyNaRece - GameManager.cs");
    //    TargetRpcWlaczKartyKlientomNaRece(listaKartNaRece);
    //}
    //[TargetRpc]
    //public void TargetRpcWlaczKartyKlientomNaRece(List<GameObject> listaKartNaRece)
    //{
    //    Debug.Log("RpcWlaczKartyKlientomNaRece() - GameManager.cs");
    //    foreach (GameObject karta in listaKartNaRece)
    //    {
    //        karta.SetActive(true);
    //        //GameObject kartaNiesieciowa = Instantiate(kartaNiesieciowaPrefab);
    //        //CardData cardData = karta.GetComponent<CardData>();
    //        //kartaNiesieciowa.GetComponent<CardData>().ScriptableKarta = cardData.ScriptableKarta;
    //        //kartaNiesieciowa.transform.position = karta.transform.position;           
    //    }
    //}

    public override void OnStartClient()
    {
        //Debug.Log("OnStartClient() - w klasie NetworkGamePlayerLobby");
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);
    }

    public override void OnStopClient() //OnNetworkDestroy()
    {
        Room.GamePlayers.Remove(this);
    }

    //zakomentowanie rozwiązało problem (jakiś kod błędu 0x000021)
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    public override void OnStartAuthority()
    {
        //Debug.Log("OnStartAuthority - w NetworkGamePlayerLobby");
    }

    public void ZnajdzMojaKarteKrasnoluda()
    {
        //Debug.Log("Szukam mojej karty Krasnoluda...");
        List<GameObject> kartyKrasnoludow = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            //Debug.Log($"Petla for i={i}");
            GameObject gameObject = GameObject.Find($"KartaKrasnoluda({i})(Clone)");
            if (gameObject != null)
            {
                kartyKrasnoludow.Add(gameObject);
                //Debug.Log($"{gameObject.name} zostala dodana do listy krasnoludow");
            }
        }

        Debug.Log($"Liczba kart krasnoludow na liscie wynosi : {kartyKrasnoludow.Count}");

        foreach (GameObject gameObject in kartyKrasnoludow)
        {
            if (gameObject.GetComponent<NetworkIdentity>().hasAuthority)
            {
                //Debug.Log($"Mam authority nad: {gameObject.name}");
                // przypisz do zmiennej
                kartaKrasnolud = gameObject;
                //Debug.Log($"kartaKrasnolud = {gameObject.name} aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            }
            else // wyłącz widoczność innych kart
            {
                //Debug.Log($"Nie mam authority {gameObject.name}");
                if (isClient)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        if (kartaKrasnolud != null)
        {
            // włącz widoczność
            kartaKrasnolud.SetActive(true);
            // połóż tam gdzie powinna leżeć
            kartaKrasnolud.transform.position = GameManager.Instance.SpawnPointKartaKrasnoluda.transform.position;
        }

        CmdSetCzyMoznaWczytacKarteKrasnoluda("false");
    }

    private void WczytajCardManager()
    {
        GameObject cardManagerGameObject = GameObject.Find("CardManager");
        CardManager cardManagerSkrypt = cardManagerGameObject.GetComponent<CardManager>();

        if (cardManagerSkrypt != null)
        {
            Room.CardManager = cardManagerSkrypt;
        }
    }

    public void ZnajdzSystemTur()
    {
        //Debug.Log("Szukam SystemTur...");
        GameObject gameObject = GameObject.Find("SystemTur(Clone)");
        if (gameObject != null)
        {
            this.systemTur = gameObject.GetComponent<SystemTur>();
        }
    }

    public string ZwrocOpisKarty(GameObject kartaGameObject)
    {
        if (kartaGameObject == null)
        {
            return "Brak wybranej karty";
        }
        else
        {
            Karta karta = kartaGameObject.GetComponent<CardData>().ScriptableKarta;

            if (karta is KartaSciezki)
            {
                if (karta is KartaPrzeszkody)
                {
                    return (karta as KartaPrzeszkody).ToString();
                }
                else
                {
                    return (karta as KartaSciezki).ToString();
                }
            }
            else if (karta is KartaAkcji)
            {
                if (karta is KartaWydarzenia)
                {
                    return (karta as KartaWydarzenia).ToString();
                }
                else if (karta is KartaUjawnienia)
                {
                    return (karta as KartaUjawnienia).ToString();
                }
                else if (karta is KartaNarzedzia)
                {
                    return (karta as KartaNarzedzia).ToString();
                }
                else
                {
                    return "Brak wybranej";
                }
            }
            else
            {
                return "Brak wybranej";
            }

        }
    }

    private void ZnajdzMojPionek()
    {
        //Debug.Log("Szukam pionków...");
        List<GameObject> listaPionkow = new List<GameObject>();

        for (int i = 0; i < 10; i++)
        {
            //Debug.Log($"Petla for i={i}");
            GameObject gameObject = GameObject.Find($"Pionek_{i}(Clone)");
            if (gameObject != null)
            {
                listaPionkow.Add(gameObject);
                //Debug.Log($"{gameObject.name} zostal dodany do listy Pionkow");
            }
        }

        //Debug.Log($"Liczba pionkow na liscie wynosi : {listaPionkow.Count}");

        foreach (GameObject gameObject in listaPionkow)
        {
            if (gameObject.GetComponent<NetworkIdentity>().hasAuthority)
            {
                //Debug.Log($"Mam authority nad: {gameObject.name}");
                // przypisz do zmiennej
                pionekKrasnolud = gameObject;
            }
            else // wyłącz widoczność innych kart
            {
                //Debug.Log($"Nie mam authority {gameObject.name}");
                //if (isClient)
                //{
                //    gameObject.SetActive(false);
                //}
            }
        }

        //if (kartaKrasnolud != null)
        //{
        //    // włącz widoczność
        //    kartaKrasnolud.SetActive(true);
        //    // połóż tam gdzie powinna leżeć
        //    kartaKrasnolud.transform.position = GameManager.Instance.SpawnPointKartaKrasnoluda.transform.position;
        //}

        CmdSetCzyMoznaWczytacPionek("false");
    }

    public void OdswiezTabeleGraczyUi()
    {
        GameObject cardManagerGameObject = GameObject.Find("CardManager");
        CardManager cardManagerSkrypt = cardManagerGameObject.GetComponent<CardManager>();
        GameObject systemTurGameObject = GameObject.Find("SystemTur(Clone)");
        SystemTur systemTurSkrypt = systemTurGameObject.GetComponent<SystemTur>();

        int liczbaKartStosDoDobierania = cardManagerSkrypt.getStosKartDoDobierania().Count;
        int liczbaKartStosOdrzuconych = cardManagerSkrypt.getStosKartOdrzuconych().Count;
        int liczbaGraczy = systemTurSkrypt.ListaKolejnoscGraczy.Count;

        string[] tablicaImionGraczy = new string[liczbaGraczy];

        for (int i = 0; i < liczbaGraczy; i++)
        {
            tablicaImionGraczy[i] = systemTurSkrypt.ListaKolejnoscGraczy[i].GetDisplayName();
        }

        RpcOdswiezTabeleGraczyUI(liczbaKartStosDoDobierania, liczbaKartStosOdrzuconych, tablicaImionGraczy);
    }

    [Command]
    public void CmdOdswiezTabeleGraczyUI()
    {
        GameObject cardManagerGameObject = GameObject.Find("CardManager");
        CardManager cardManagerSkrypt = cardManagerGameObject.GetComponent<CardManager>();
        GameObject systemTurGameObject = GameObject.Find("SystemTur(Clone)");
        SystemTur systemTurSkrypt = systemTurGameObject.GetComponent<SystemTur>();

        int liczbaKartStosDoDobierania = cardManagerSkrypt.getStosKartDoDobierania().Count;
        int liczbaKartStosOdrzuconych = cardManagerSkrypt.getStosKartOdrzuconych().Count;
        int liczbaGraczy = systemTurSkrypt.ListaKolejnoscGraczy.Count;

        string[] tablicaImionGraczy = new string[liczbaGraczy];

        for (int i = 0; i < liczbaGraczy; i++)
        {
            tablicaImionGraczy[i] = systemTurSkrypt.ListaKolejnoscGraczy[i].GetDisplayName();
        }

        RpcOdswiezTabeleGraczyUI(liczbaKartStosDoDobierania, liczbaKartStosOdrzuconych, tablicaImionGraczy);
    }

    [ClientRpc]
    public void RpcOdswiezTabeleGraczyUI(int liczbaKartStosDobierania, int liczbaKartStosOdrzuconych, string[] tablicaImionGraczy)
    {      
        uI_DisplayPlayerInfo.SetUI_LiczbaKartNaStosieDoDobierania(liczbaKartStosDobierania);
        uI_DisplayPlayerInfo.SetUI_LiczbaKartNaStosieKartOdrzuconych(liczbaKartStosOdrzuconych);       
        uI_DisplayPlayerInfo.SetUI_LiczbaGraczy(tablicaImionGraczy.Length /*Room.GamePlayers.Count*/);
        uI_DisplayPlayerInfo.SetUI_TabelaGraczy(tablicaImionGraczy);
    }
}

