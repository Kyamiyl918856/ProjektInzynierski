using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    //public NetworkManager networkManager;
    public TasowanieKart tasowanieKart;
    public CardManager cardManager;
    public int nrGraczaAktualnaTura;
    public ZasadyGry zasadyGry;
    [SerializeField] private List<Transform> spawnPointsKartyNaRece;
    [SerializeField] private Transform spawnPointKartaKrasnoluda;

    [SerializeField] private GameObject spawnPointDlaKart = null;
    [SerializeField] private Transform spawnPointDlaKartOdrzuconych;
    [SerializeField] private List<Transform> spawnPointsKartySkarbu;

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public enum KrokWTurze
    {
        ZagranieLubOdrzucenieKart,
        RuchPionkiemNaPlanszy,
        DobranieKart
    }

    public List<Transform> SpawnPointsKartyNaRece
    {
        get { return spawnPointsKartyNaRece; }
    }

    public Transform SpawnPointKartaKrasnoluda
    {
        get { return spawnPointKartaKrasnoluda; }
    }

    public Transform SpawnPointDlaKartOdrzuconych
    {
        get { return spawnPointDlaKartOdrzuconych; }
    }

    public List<Transform> SpawnPointsKartySkarbu
    {
        get { return spawnPointsKartySkarbu; } 
    }

    private void Awake()
    {
        Instance = this;

        //networkManager = GetComponent<NetworkManager>();
        //tasowanieKart = GetComponent<TasowanieKart>();
    }


    void Start()
    {
        // start gry
        // ropoczęcie pętli gry
        // plansza pojawia się na stole
        // przetasowanie kart kopalni i umieszcza się zakryte na odpowiednich polach na planszy


        // przetasowanie kart skarbów i wyłożenie tyle zakrytych kart, ilu jest graczy. Resztę się odrzuca
        //List<GameObject> kartySkarbu = new List<GameObject>();
        //kartySkarbu.AddRange(cardManager.getListaKartSkarbu());
        //kartySkarbu.AddRange(tasowanieKart.Przetasuj(kartySkarbu));

        // przetasowanie kart krasnoludów
        // jeśli jest minimum 7 graczy to zostają wszystkie karty krasnoludów
        // poniżej 7 i powyżej 4 graczy usunąć po 1 lojalnym krasnoludzie z każdego klanu z decku
        // poniżej 5 graczy usunąć po 2 lojalne krasnoludy z każdego klanu z decku 
        // i rozdanie po jednej karcie krasnoluda każdemu z graczy

        // przetasowanie kart startu i kopalni i polozenie na planszy
        /*
        if (isServer)
        {
            Debug.Log("Jestem serwerem, tasuję karty startu...");
            tasowanieKart.Przetasuj<GameObject>(cardManager.getListaKartStartu());
            Debug.Log("Jestem serwerem, tasuję karty kopalni...");
            tasowanieKart.Przetasuj<GameObject>(cardManager.getListaKartKopalni());
        }
        UlozKartyStartuNaPolachStartowych();
        UlozKartyKopalniNaPolachKopalni();

        // przetasowanie kart do grania (ścieżki + akcje)
        if (isServer)
        {
            Debug.Log("Jestem serwerem, tasuję karty do grania...");
            tasowanieKart.Przetasuj<GameObject>(cardManager.getListaKartDoGrania());
        }

        // odrzucenie 10 kart ze stosu do dobierania
        Debug.Log("Odrzucenie 10 kart ze stosu do dobierania");
        tasowanieKart.OdrzucKarty(cardManager.getListaKartDoGrania(), 10);
        //tasowanieKart.Pokaż_Karty(cardManager.getListaKartDoGrania());

        // rozlozenie wszystkich kart na stole - kod testowy - do skasowania później
        //UlozKartyDoDobieraniaWStos();
        UlozKartyDoDobieraniaKazdaPojedynczoNaStole();
        ObrocKartyAwersemDoGory();

        // rozdanie graczom kart do grania
        //Debug.Log("Rozdanie Graczom kart do grania");
        //tasowanieKart.RozdajKartyDoGraniaGraczom(cardManager.getListaKartDoGrania(), playerManager.GetListaGraczy());
        // rozpoczęcie drugiej pętli rund
        // ropoczęcie rundy
        // ropoczęcie tury gracza (każdy po kolei)
        */

    }

    

    public void OdwrocKartyNaReceAwersemDoGory(List<NetworkGamePlayerLobby> listaGraczy)
    {
        foreach(NetworkGamePlayerLobby gracz in listaGraczy)
        {
            foreach(GameObject karta in gracz.KartyNaRece)
            {
                karta.GetComponent<CardData>().OdkryjKarte();
            }
        }
    }
    public void OdwrocKartyNaReceAwersemDoGory(List<GameObject> listaKart)
    {
        foreach(GameObject karta in listaKart)
        {
            karta.GetComponent<CardData>().OdkryjKarte();
            Debug.Log("Karta powinna zostać odkryta odwrócona");
        }
    }
    
    public void UlozKartyNaRece(List<NetworkGamePlayerLobby> listaGraczy)
    {
        foreach(NetworkGamePlayerLobby gracz in listaGraczy)
        {
            int i = 0;
            foreach(GameObject karta in gracz.KartyNaRece)
            {
                karta.transform.position = spawnPointsKartyNaRece[i].transform.position;
                i++;
            }
        }
    }

    public void WylaczObiektyKartyDoGrania(List<GameObject> listaKart)
    {
        foreach (GameObject karta in listaKart)
        {
            karta.SetActive(false);
        }
    }

    public void WylaczMeshRendererBoxColliderKartNaReceKazdegoGracza(List <GameObject> listaKart)
    {
        foreach(GameObject karta in listaKart)
        {
            MeshRenderer meshRenderer = karta.GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
            BoxCollider boxCollider = karta.GetComponent<BoxCollider>();
            boxCollider.enabled = false;
        }
    }

    public void UlozKartyStartuNaPolachStartowych(List<GameObject> listaKartStartowych)
    {
        Debug.Log("UlozKartyStartuNaPolachStartowych");
        int i = 0;
        foreach(GameObject gameObject in zasadyGry.PolaPlanszyList)
        {
            if(gameObject.GetComponent<PoleMapy>()._TypPola == PoleMapy.TypPola.Startowe)
            {
                Debug.Log("Pola Startowe wczytuję karty startowe obozów..." + i);
                gameObject.GetComponent<PoleMapy>().KartaPolozonaNaPolu = listaKartStartowych[i];
                gameObject.GetComponent<PoleMapy>().KartaPolozonaNaPolu.transform.position = gameObject.transform.position + new Vector3(0,0.1f,0);
                i++;
                if (i == 2) { return; }
            }                     
        }
    }

    public void UlozKartyKopalniNaPolachKopalni()
    {
        Debug.Log("UlozKartyKopalniNaPolachKopalni");
        int i = 0;
        foreach(GameObject gameObject in zasadyGry.PolaPlanszyList)
        {
            if(gameObject.GetComponent<PoleMapy>()._TypPola == PoleMapy.TypPola.Kopalnia)
            {
                gameObject.GetComponent<PoleMapy>().KartaPolozonaNaPolu = cardManager.getListaKartKopalni()[i];
                gameObject.GetComponent<PoleMapy>().KartaPolozonaNaPolu.transform.position = gameObject.transform.position + new Vector3(0, 0.1f, 0);
                i++;
                if (i == 4) { return; }
            }
        }
    }

    public void UlozPionkiGraczyNaKartachObozow()
    {
        foreach(GameObject kartaStartu in cardManager.getListaKartStartu())
        {
            if((kartaStartu.GetComponent<CardData>().ScriptableKarta as KartaStartu)._TypKlanu == KartaStartu.TypKlanu.Żółty)
            {
                foreach(NetworkGamePlayerLobby gracz in Room.GamePlayers)
                {
                    if(gracz.PionekKrasnolud.GetComponent<PionekInfo>()._Klan == PionekInfo.Klan.Zolty)
                    {
                        gracz.PionekKrasnolud.transform.position = kartaStartu.transform.position;
                        gracz.PionekKrasnolud.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje = kartaStartu;
                        (kartaStartu.GetComponent<CardData>().ScriptableKarta as KartaSciezki).ListaPionkowStojacychNaTejKarcie.Add(gracz.PionekKrasnolud); 
                    }
                }
            }
            else if((kartaStartu.GetComponent<CardData>().ScriptableKarta as KartaStartu)._TypKlanu == KartaStartu.TypKlanu.Niebieski)
            {
                foreach (NetworkGamePlayerLobby gracz in Room.GamePlayers)
                {
                    if (gracz.PionekKrasnolud.GetComponent<PionekInfo>()._Klan == PionekInfo.Klan.Niebieski)
                    {
                        gracz.PionekKrasnolud.transform.position = kartaStartu.transform.position;
                        gracz.PionekKrasnolud.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje = kartaStartu;
                        (kartaStartu.GetComponent<CardData>().ScriptableKarta as KartaSciezki).ListaPionkowStojacychNaTejKarcie.Add(gracz.PionekKrasnolud);
                    }
                }

            }
        }
    }

    private void UlozKartyDoDobieraniaWStos()
    {
        float y = 0.1f;
        foreach (GameObject gameObject in cardManager.getListaKartDoGrania())
        {
            gameObject.transform.position = spawnPointDlaKart.transform.position + new Vector3(0, y, 0);
            y += 0.1f;
        }
    }
    private void UlozKartyDoDobieraniaKazdaPojedynczoNaStole()
    {
        float x = 0f;
        float z = 0f;
        int i = 1;
        foreach (GameObject gameObject in cardManager.getListaKartDoGrania())
        {
            gameObject.transform.position = spawnPointDlaKart.transform.position + new Vector3(x, 0, z);
            x -= 4.5f;
            if (i == 8)
            {
                x = 0f;
                i = 0;
                z += 7f;
            }
            i++;
        }
    }

    private void ObrocKartyAwersemDoGory()
    {
        foreach (GameObject gameObject in cardManager.getListaKartDoGrania())
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

   


}
