using System.IO;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene][SerializeField] private string menuScene = string.Empty;

    [Header("Maps")]
    [SerializeField] private int numberOfRounds = 1;
    //[SerializeField] private MapSet mapSet = null;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;

    [Header("TALIE ELEMENTÓW (KARTY / PIONKI)")]
    [SerializeField] private List<GameObject> listaKartySciezkiAkcjePrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> listaKartyKrasnoludyPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> listaPionkowPrefabs = new List<GameObject>();


    //[SerializeField] private GameObject playerSpawnSystem = null;
    //[SerializeField] private GameObject roundSystem = null;
    [Header("Ustawienia Poczatkowe")]
    [SerializeField] private GameObject ustawieniaPoczatkowePrefab = null;

    //private MapHandler mapHandler;

    [SerializeField] private CardManager cardManager = null;
    public CardManager CardManager
    {
        get { return cardManager; }
        set { cardManager = value; }
    }
    [SerializeField] private TasowanieKart tasowanieKart = null;

    public TasowanieKart TasowanieKart
    {
        get { return tasowanieKart; }
    }

    [SerializeField] private SystemTur systemTur = null;


    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {          
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        //Debug.Log($"NetworkManagerLobby: OnClientConnect()");
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        //Debug.Log($"NetworkManagerLobby: OnClientDisconnect()");
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        //Debug.Log($"NetworkManagerLobby: OnServerConnect()");
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //Debug.Log($"NetworkManagerLobby: OnServerAddPlayer()");
        menuScene = Path.GetFileNameWithoutExtension(menuScene);
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
        Debug.Log($"Graczy na serwerze: {numPlayers}");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log($"NetworkManagerLobby: OnServerDisconnect()");
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Debug.Log($"NetworkManagerLobby: OnStopServer()");
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        GamePlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }


    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            if (!IsReadyToStart()) { return; }

            ServerChangeScene("Scene_Map_01"); // to na chwile, można odkomentować poniższe chyba i to usunąć po lewej



            // te poniżej zakomentowałem na chwile
            //mapHandler = new MapHandler(mapSet, numberOfRounds);

            //ServerChangeScene(mapHandler.NextMap);
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Scene_Map"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                //NetworkServer.Destroy(conn.identity.gameObject); //niszczymy obiekt RoomPlayer

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }



    public override void OnServerSceneChanged(string sceneName)
    {
        //Debug.Log("OnServerSceneChanged() - w NetworkManagerLobby");
        Debug.Log("Scena została zmieniona na grę planszę");
        if (sceneName.StartsWith("Scene_Map"))
        {
            cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
            tasowanieKart = GameObject.Find("TasowanieKart").GetComponent<TasowanieKart>();

            foreach(NetworkGamePlayerLobby gracz in GamePlayers)
            {
                gracz.SetCzyMoznaWczytacCardManager("true");
            }

            //GameObject taliaKartSciezkiAkcje = Instantiate(this.taliaKartSciezkiAkcje);     to nie działa można usunąć
            //NetworkServer.Spawn(taliaKartSciezkiAkcje);                                     to nie działa można usunąć

            // spawn kart sciezki i akcje 
            foreach (GameObject gameObject in listaKartySciezkiAkcjePrefabs)
            {
                GameObject karta = Instantiate(gameObject);
                NetworkServer.Spawn(karta);
                cardManager.getListaKartDoGrania().Add(karta);
            }

            // spawn kart krasnoludów
            foreach (GameObject gameObject in listaKartyKrasnoludyPrefabs)
            {
                GameObject kartaKrasnoluda = Instantiate(gameObject);
                NetworkServer.Spawn(kartaKrasnoluda);
                cardManager.getListaKartKrasnoludow().Add(kartaKrasnoluda);
            }
            // 0-niebieski_lojalny
            // 1-niebieski_lojalny
            // 2-niebieski_lojalny
            // 3-niebieski_sabotażysta
            // 4-niebieski_samolub
            // 5-zolty_lojalny
            // 6-zolty_lojalny
            // 7-zolty_lojalny
            // 8-zolty_sabotazysta
            // 9-zolty_samolub

            foreach (GameObject karta in cardManager.getListaKartKrasnoludow())
            {
                cardManager.getStosKartKrasnoludowDoRozdania().Add(karta);
            }

            // spawn pionków
            foreach (GameObject gameObject in listaPionkowPrefabs)
            {
                GameObject pionek = Instantiate(gameObject);
                NetworkServer.Spawn(pionek);
                cardManager.getListaPionkow().Add(pionek);
            }

            // usuń lojalne krasnoludy z klanu w zależności od liczby graczy
            if (GamePlayers.Count < 5)
            {
                tasowanieKart.UsunKrasnoludyZTalii(2, cardManager.getStosKartKrasnoludowDoRozdania(), cardManager.getListaKartKrasnoludow());
            }
            if (GamePlayers.Count > 4 && GamePlayers.Count < 7)
            {
                tasowanieKart.UsunKrasnoludyZTalii(1, cardManager.getStosKartKrasnoludowDoRozdania(), cardManager.getListaKartKrasnoludow());
            }


            // przetasuj wszystkie talie
            tasowanieKart.Przetasuj<GameObject>(cardManager.getListaKartDoGrania());
            tasowanieKart.Przetasuj<GameObject>(cardManager.getListaKartStartu());
            tasowanieKart.Przetasuj<GameObject>(cardManager.getStosKartKrasnoludowDoRozdania());
            tasowanieKart.Przetasuj<GameObject>(cardManager.getListaPionkow());
            tasowanieKart.Przetasuj<GameObject>(cardManager.getListaKartSkarbu());
            tasowanieKart.Przetasuj<GameObject>(cardManager.getListaKartKopalni());
            //tasowanieKart.Przetasuj<GameObject>(cardManager.getListaKartKrasnoludow());
            Debug.Log("Karty zostały przetasowane");

            //Ułóż karty skarbu na spawnPointach
            for(int i=0; i<GamePlayers.Count; i++)
            {
                GameObject kartaSkarbu = cardManager.getListaKartSkarbu()[i];
                kartaSkarbu.transform.position = GameManager.Instance.SpawnPointsKartySkarbu[i].position;
                // obroc karty obrot y = 270
                kartaSkarbu.transform.rotation = Quaternion.Euler(0f, 270f, 0f);

                cardManager.getListaKartSkarbuDoBrania().Add(kartaSkarbu);
            }


            foreach (GameObject karta in cardManager.getListaKartDoGrania())
            {
                cardManager.getStosKartDoDobierania().Add(karta);
            }

            List<GameObject> pionkiNiebieskie = new List<GameObject>();
            List<GameObject> pionkiZolte = new List<GameObject>();

            foreach (GameObject gameObject in cardManager.getListaPionkow())
            {
                if (gameObject.GetComponent<PionekInfo>()._Klan == PionekInfo.Klan.Niebieski)
                {
                    pionkiNiebieskie.Add(gameObject);
                }

                if (gameObject.GetComponent<PionekInfo>()._Klan == PionekInfo.Klan.Zolty)
                {
                    pionkiZolte.Add(gameObject);
                }
            }


            // ROZDAJ GRACZOM KARTY
            // do grania
            tasowanieKart.RozdajKartyDoGraniaGraczom<GameObject>(cardManager.getStosKartDoDobierania(), GamePlayers);
            // karty krasnoludów
            tasowanieKart.RozdajKartyKrasnoludaGraczom(1, cardManager.getStosKartKrasnoludowDoRozdania(), GamePlayers);

            tasowanieKart.UsunPionkiKtoreSaNiepotrzebne(GamePlayers, pionkiZolte, pionkiNiebieskie);
            tasowanieKart.RozdajGraczomPionki(GamePlayers, pionkiZolte, pionkiNiebieskie);

            // TU SKOŃCZYŁEM, CO DALEJ?????

            //foreach(NetworkGamePlayerLobby gracz in GamePlayers)
            //{
            //    gracz.CzyMoznaWczytacKarteKrasnoluda = 1; // POPRAWIĆ!!!!!!!!!!!!!!!!!! tu być nie może bo na klienta nie przesyła
            //}
            // znaczniki krasnoludów
            // jeszcze jakieś może nie wiem


            //cardManager.getStosKartDoDobierania().AddRange(cardManager.getListaKartDoGrania());
            //tasowanieKart.RozdajKartyDoGraniaGraczom<GameObject>(cardManager.getStosKartDoDobierania(), GamePlayers);
            //Debug.Log("Teraz nastąpi przypisanie AssignClientAuthority do kart...");
            //(cardManager.getListaKartDoGrania()[0]).GetComponent<NetworkIdentity>().AssignClientAuthority(GamePlayers[0].GetComponent<NetworkIdentity>().connectionToClient);
            //(cardManager.getListaKartDoGrania()[1]).GetComponent<NetworkIdentity>().AssignClientAuthority(GamePlayers[1].GetComponent<NetworkIdentity>().connectionToClient);
            Debug.Log("karty zostały rozdane graczom");



            // ułóż karty obozów na polach startowych
            GameManager.Instance.UlozKartyStartuNaPolachStartowych(cardManager.getListaKartStartu());

            // PRACUJĘ TERAZ TUTAJ NAD TYM
            GameManager.Instance.UlozKartyKopalniNaPolachKopalni();










            //GameManager.Instance.UlozKartyNaRece(GamePlayers);

            //GameManager.Instance.OdwrocKartyNaReceAwersemDoGory(GamePlayers); // tu jeszcze poprawić, przyjrzeć się dlaczego niektóre karty się nie odwracają


            //GameManager.Instance.WylaczObiektyKartyDoGrania(cardManager.getListaKartDoGrania());     // na chwile to wyłączam teraz 28.04.2024 00:45



            // POZWÓL GRACZOM, ŻEBY WCZYTALI SWOJE KARTY


            foreach (NetworkGamePlayerLobby gracz in GamePlayers)
            {
                //gracz.SetDisplayName("Buba");
                gracz.SetTrueCzyMoznaWykonacFunkcje("true");
            }

            foreach (NetworkGamePlayerLobby gracz in GamePlayers)
            {
                gracz.SetCzyMoznaWczytacKarteKrasnoluda("true");
            }

            foreach(NetworkGamePlayerLobby gracz in GamePlayers)
            {
                gracz.SetCzyMoznaWczytacPionek("true");
            }

            SystemTur systemTur = Instantiate(this.systemTur);
            NetworkServer.Spawn(systemTur.gameObject);

            // wczytanie systemuTur graczom na serwerze
            foreach (NetworkGamePlayerLobby gracz in GamePlayers)
            {
                gracz.SystemTur = systemTur;
            }

            // wczytanie pól mapy na serwerze na klientach do listy w klasie ZasadyGry 
            foreach (NetworkGamePlayerLobby gracz in GamePlayers)
            {
                if (gracz.GetComponent<ZasadyGry>().PolaPlanszyList.Count < 1)
                {
                    GameObject mapa = GameObject.Find("Mapa");
                    for (int i = 0; i < mapa.transform.childCount; i++)
                    {
                        GameObject pole = mapa.transform.GetChild(i).gameObject;
                        gracz.GetComponent<ZasadyGry>().PolaPlanszyList.Add(pole);
                    }
                }
            }

            foreach (NetworkGamePlayerLobby gracz in GamePlayers)
            {
                gracz.GetComponent<ZasadyGry>().CzyObozyStartoweNaSerwerzeWczytane = 1;
                gracz.GetComponent<ZasadyGry>().CzyKopalnieNaSerwerzeWczytane = 1;
            }


            // zespawnuj karty krasnoluda graczom
            // zespawnuj pionki graczom
            // zespawnuj karty skarbu na stole

            GameManager.Instance.UlozPionkiGraczyNaKartachObozow();

            foreach(NetworkGamePlayerLobby gracz in GamePlayers)
            {
                gracz.SetCzyMoznaWczytacPionkiReszcieGraczy("true");
            }



            // zespawnuj karty graczowi
            //GameObject karta = Instantiate(cardManager.getListaKartDoGrania()[0]);
            //NetworkServer.Spawn(karta, GamePlayers[1].connectionToClient);
            //karta.transform.position = GameManager.Instance.SpawnPointsKartyNaRece[0].position;


            //GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            //NetworkServer.Spawn(playerSpawnSystemInstance);

            //GameObject roundSystemInstance = Instantiate(roundSystem);
            //NetworkServer.Spawn(roundSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        //Debug.Log($"NetworkManagerLobby: OnServerReady() wywolanie");
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}

