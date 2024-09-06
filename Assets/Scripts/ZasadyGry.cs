using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ZasadyGry : NetworkBehaviour
{
    [SerializeField] private List<GameObject> polaPlanszyList = new List<GameObject>();
    [SerializeField] private NetworkGamePlayerLobby gracz;
    [SerializeField] private GameObject niezydentifikowanyObiekt;
    [SerializeField] private GameObject kartaKliknieta;
    [SerializeField] private GameObject polePlanszyKlikniete;
    [SerializeField] private GameObject kartaSkarbuKliknieta;
    [SerializeField] private GameObject kartaNarzedziaKliknieta;
    [SerializeField] private GameObject kartaPrzeszkodyNaKtoraChcemyUzycKartyNarzedzia;

    public GameObject KartaKliknieta
    {
        get { return kartaKliknieta; }
    }

    public List<GameObject> PolaPlanszyList
    {
        get { return polaPlanszyList; }
    }

    [Header("czyObozyStartoweNaSerwerzeWczytane")]
    [SyncVar]
    [SerializeField]
    private int czyObozyStartoweNaSerwerzeWczytane = 0;

    [Header("czyKopalnieNaSerwerzeWczytane")]
    [SyncVar]
    [SerializeField]
    private int czyKopalnieNaSerwerzeWczytane = 0;

    public int CzyObozyStartoweNaSerwerzeWczytane
    {
        get { return czyObozyStartoweNaSerwerzeWczytane; }
        set { czyObozyStartoweNaSerwerzeWczytane = value; }
    }

    public int CzyKopalnieNaSerwerzeWczytane
    {
        get { return czyKopalnieNaSerwerzeWczytane; }
        set { czyKopalnieNaSerwerzeWczytane = value; }
    }

    [SerializeField] bool czyPolaWczytane = false;
    [SerializeField] bool czyPolaStartoweWczytane = false;
    [SerializeField] bool czyKopalnieWczytane = false;
    [SerializeField] bool czyKartaNarzedziZagrana = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer && czyPolaWczytane == false)
        {
            ZnajdzPolaMapy();
        }

        if (isLocalPlayer && isClientOnly && czyObozyStartoweNaSerwerzeWczytane == 1 && czyPolaWczytane && !czyPolaStartoweWczytane)
        {
            // przypisz karty startu do pól
            // wywolaj command, który sprawdzi, wszystkie pola
            CmdSprawdzPolaStartowe();

        }

        if (isLocalPlayer && isClientOnly && czyKopalnieNaSerwerzeWczytane == 1 && czyPolaWczytane && !czyKopalnieWczytane)
        {
            CmdSprawdzKopalnie();
        }

        //if(krok_w_turze == zagranieKarty_lub_odrzucenieKarty)
        //{
        //if(krok_w_turze == zagranieKarty)
        //{
        //1 PolozKarteNaPoluPlanszy();
        //2 OdkryjKarteKopalniJesliKliknieta();
        //3 ObrocSciezkeJesliKartaWybranaIPrzyciskRWcisniety();
        //}
        //else if(krok_w_turze == odrzucenieKarty)
        //{
        //OdrzucKarteLubDwieKarty();
        //}

        //}
        //if(krok_w_turze == ruszenieKrasnoludem)
        //{
        //RuszKrasnoludem();
        //}
        //if(krok_w_turze == dobranieKart)
        //{
        // DobierzKartyIlePotrzeba();
        //}

    }

    [Command]
    private void CmdSprawdzKopalnie()
    {
        List<GameObject> kopalnie = new List<GameObject>();
        foreach (GameObject pole in polaPlanszyList)
        {
            GameObject kopalnia = pole.GetComponent<PoleMapy>().GetKartaPolozonaNaPolu();

            if (kopalnia != null && kopalnia.GetComponent<CardData>().ScriptableKarta is KartaKopalni)
            {
                kopalnie.Add(kopalnia);
            }
            else
            {

            }
        }

        RpcWczytajKopalnie(kopalnie[0], kopalnie[1], kopalnie[2], kopalnie[3]);
    }

    [Command]
    private void CmdSprawdzPolaStartowe()
    {

        //Debug.Log($"CmdSprawdzPolaStartowe() - polaPlanszyList = {polaPlanszyList.Count}"); // polaPlanszyList jest = 0    Dlaczego???
        List<string> kartyStartowe = new List<string>();
        foreach (GameObject pole in polaPlanszyList)
        {
            GameObject kartaStartowa = pole.GetComponent<PoleMapy>().GetKartaPolozonaNaPolu();

            if (kartaStartowa != null && kartaStartowa.GetComponent<CardData>().ScriptableKarta is KartaStartu)
            {
                //tu sprawdzic indeksy albo nazwy tych gameobjectow kart i przekazac w parametrze string albo int
                kartyStartowe.Add(kartaStartowa.gameObject.name);
                //Debug.Log($"Karta startowa zostala znaleziona i zostala dodana do listy {kartaStartowa.gameObject.name}");
            }
            else
            {
                //Debug.Log("karta startowa na serwerze nie zostala znaleziona na polu");
            }
        }

        RpcWczytajPolaStartowe(kartyStartowe[0], kartyStartowe[1]);
    }

    [ClientRpc]
    private void RpcWczytajKopalnie(GameObject kopalnia0, GameObject kopalnia1, GameObject kopalnia2, GameObject kopalnia3)
    {
        List<GameObject> kopalnie = new List<GameObject>();
        kopalnie.Add(kopalnia0);
        kopalnie.Add(kopalnia1);
        kopalnie.Add(kopalnia2);
        kopalnie.Add(kopalnia3);

        int j = 0;
        foreach (GameObject pole in polaPlanszyList)
        {
            if (pole.GetComponent<PoleMapy>()._TypPola == PoleMapy.TypPola.Kopalnia)
            {
                pole.GetComponent<PoleMapy>().SetKartaPo³ozonaNaPolu(kopalnie[j]);
                j++;
            }
        }
        czyKopalnieWczytane = true;
    }

    [ClientRpc]
    private void RpcWczytajPolaStartowe(string nazwa1, string nazwa2) // Rozwi¹zany problem, nie da sie przekazywaæ GameObjectów i list bo s¹ zbyt z³o¿one do serializowania i deserializowania, tylko typy proste mozna przekazywaæ
    {
        //Debug.Log($"RpcWczytajPolaStartowe()");
        //Debug.Log($"Przekazana nazwa1 = {nazwa1}, przekazana nazwa2 = {nazwa2} - gracz: {this.gameObject.GetComponent<NetworkGamePlayerLobby>().GetDisplayName()}");

        List<GameObject> listaKartStartu = new List<GameObject>();
        GameObject taliaKartyStartu = GameObject.Find("Talia_KartyStartu");
        for (int i = 0; i < taliaKartyStartu.transform.childCount; i++)
        {
            listaKartStartu.Add(taliaKartyStartu.transform.GetChild(i).gameObject);
        }

        int j = 0;
        foreach (GameObject pole in polaPlanszyList)
        {
            if (pole.GetComponent<PoleMapy>()._TypPola == PoleMapy.TypPola.Startowe)
            {
                //Debug.Log("Znaleziono pole startowe");
                if (j == 0)
                {
                    if (listaKartStartu[0].name == nazwa1)
                    {
                        pole.GetComponent<PoleMapy>().SetKartaPo³ozonaNaPolu(listaKartStartu[0]);
                        //Debug.Log($"Do pola {pole.name} zosta³a przypisana karta {listaKartStartu[0].name}");
                    }
                    if (listaKartStartu[1].name == nazwa1)
                    {
                        pole.GetComponent<PoleMapy>().SetKartaPo³ozonaNaPolu(listaKartStartu[1]);
                        //Debug.Log($"Do pola {pole.name} zosta³a przypisana karta {listaKartStartu[1].name}");
                    }
                }
                else if (j == 1)
                {
                    if (listaKartStartu[0].name == nazwa2)
                    {
                        pole.GetComponent<PoleMapy>().SetKartaPo³ozonaNaPolu(listaKartStartu[0]);
                        //Debug.Log($"Do pola {pole.name} zosta³a przypisana karta {listaKartStartu[0].name}");
                    }
                    if (listaKartStartu[1].name == nazwa2)
                    {
                        pole.GetComponent<PoleMapy>().SetKartaPo³ozonaNaPolu(listaKartStartu[1]);
                        //Debug.Log($"Do pola {pole.name} zosta³a przypisana karta {listaKartStartu[1].name}");
                    }
                }
                // lista jest pusta? indeks wykroczy³ poza zakres
                j++;
            }
        }
        czyPolaStartoweWczytane = true;
    }

    private void ZnajdzPolaMapy()
    {
        GameObject mapa = GameObject.Find("Mapa");

        if (mapa != null && polaPlanszyList.Count < 1)
        {
            for (int i = 0; i < mapa.transform.childCount; i++)
            {
                GameObject pole = mapa.transform.GetChild(i).gameObject;
                polaPlanszyList.Add(pole);
                //Debug.Log($"Pole zosta³o dodane do listy {pole.gameObject.name}");
            }
            czyPolaWczytane = true;
        }
        else
        {
            //Debug.Log("Nie uda³o siê znaleŸæ MAPY !!!");
        }
    }

    public void OdkryjKarteKopalniJesliKliknieta()
    {
        if (kartaKliknieta == null) { return; }
        if (kartaKliknieta.GetComponent<CardData>().ScriptableKarta._TypKarty == Karta.TypKarty.Kopalnia)
        {
            (kartaKliknieta.GetComponent<CardData>().ScriptableKarta as KartaKopalni).CzyKopalniaOdkryta = true; //!(kartaKliknieta.GetComponent<CardData>().ScriptableKarta as KartaKopalni).CzyKopalniaOdkryta;
            kartaKliknieta.GetComponent<CardData>().OdkryjKarte();
            ResetKliknietejKartyIKliknietegoPolaPlanszy();
        }
    }

    public void ObrocSciezkeJesliKartaWybranaIPrzyciskRWcisniety()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (kartaKliknieta != null)
            {
                CardData karta = kartaKliknieta.GetComponent<CardData>();
                if (karta.ScriptableKarta is KartaSciezki)
                {
                    karta.ObrocKarte();
                    Debug.Log(karta + "Karta zostala obrócona");
                }
            }
        }
    }

    public void PrzejdzDalej()
    {
        GameObject systemTurGameObject = GameObject.Find("SystemTur(Clone)");
        SystemTur systemTurSkrypt = systemTurGameObject.GetComponent<SystemTur>();
        if (systemTurSkrypt.NrKolejnoscGracza != gracz.NrKolejnosci) { return; }
        if (systemTurSkrypt.IleKartZostaloJuzOdrzuconych < 1) { return; }
        if (systemTurSkrypt.IleKartZostaloJuzOdrzuconych >= 2) { return; }
        CmdUstawKrokZagraniaKartyNaWykonany();
    }

    public void PowrotDoMenu()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
        //SceneManager.LoadScene("Scene_Lobby");
    }

    public void OdrzucKarte()
    {
        if (kartaKliknieta == null) { return; }

        if (!gracz.KartyNaRece.Contains(kartaKliknieta)) { return; }

        GameObject systemTurGameObject = GameObject.Find("SystemTur(Clone)");
        SystemTur systemTurSkrypt = systemTurGameObject.GetComponent<SystemTur>();

        if (systemTurSkrypt.NrKolejnoscGracza != gracz.NrKolejnosci) { return; }

        if (systemTurSkrypt.ZagranieLubOdczucenie == 1) { return; }

        if (systemTurSkrypt.IleKartZostaloJuzOdrzuconych >= 2) { return; }

        gracz.KartyNaRece.Remove(kartaKliknieta);

        CmdOdrzucKarte(kartaKliknieta, GameManager.Instance.SpawnPointDlaKartOdrzuconych.position);

        // zmiana pozycji i odwrocenie karty (zakrycie gór¹ do do³u)
        //kartaKliknieta.transform.position = GameManager.Instance.SpawnPointDlaKartOdrzuconych.position;
        //kartaKliknieta.GetComponent<CardData>().ZakryjKarte();
    }

    [Command]
    public void CmdOdrzucKarte(GameObject kartaGameObject, Vector3 position)
    {
        Debug.Log($"Przekazana karta do odrzucenia to: {kartaGameObject.name}");
        kartaGameObject.GetComponent<BoxCollider>().enabled = true;
        kartaGameObject.GetComponent<MeshRenderer>().enabled = true;

        // zmieñ pozycjê karty 
        //kartaGameObject.transform.position = position;  komentuje 09.05.2024 godz 04:52, przenoszê wykonanie zmiany pozycji na klienta

        // Odwroc kartê awersem do do³u
        //kartaGameObject.GetComponent<CardData>().ZakryjKarte(); komentuje 09.05.2024 godz 04:52, przenoszê wykonanie zmiany pozycji na klienta

        // znajdz CardManager
        GameObject cardManagerGameObject = GameObject.Find("CardManager");
        CardManager cardManagerSkrypt = cardManagerGameObject.GetComponent<CardManager>();
        // dodaj karte do stosu kart odrzuconych
        cardManagerSkrypt.getStosKartOdrzuconych().Add(kartaGameObject);

        // u³ó¿ karty jedna na drugiej o +0.1 w osi Y
        Vector3 pozycjaOstaniejKarty = cardManagerSkrypt.getStosKartOdrzuconych()[0].transform.position;
        foreach (GameObject gameObject in cardManagerSkrypt.getStosKartOdrzuconych())
        {
            gameObject.transform.position = pozycjaOstaniejKarty + new Vector3(0f, 0.1f, 0f);
            pozycjaOstaniejKarty = gameObject.transform.position;
        }

        GameObject systemTurGameObject = GameObject.Find("SystemTur(Clone)");
        SystemTur systemTurSkrypt = systemTurGameObject.GetComponent<SystemTur>();

        systemTurSkrypt.IleKartZostaloJuzOdrzuconych = systemTurSkrypt.IleKartZostaloJuzOdrzuconych + 1;

        if (systemTurSkrypt.IleKartZostaloJuzOdrzuconych > 1)
        {
            systemTurSkrypt.ZagranieLubOdczucenie = 1;
            systemTurSkrypt.Stan = systemTurSkrypt.Stan + 1;
        }

        // usuñ karty graczowi z listy ale na serwerze
        foreach (NetworkGamePlayerLobby graczSkrypt in gracz.Room.GamePlayers)
        {
            foreach (GameObject gameObject in graczSkrypt.KartyNaRece)
            {
                if (kartaGameObject == gameObject)
                {
                    graczSkrypt.KartyNaRece.Remove(gameObject);
                    break;
                }
            }
        }

        kartaGameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();
        kartaGameObject.transform.position = GameManager.Instance.SpawnPointDlaKartOdrzuconych.position;
        kartaGameObject.GetComponent<CardData>().ZakryjKarte();

        RpcResetKlieknietejKarty();
    }

    [ClientRpc]
    public void RpcResetKlieknietejKarty()
    {
        ResetKliknietejKartyIKliknietegoPolaPlanszy();
    }



    [Command]
    public void CmdWykonajPrzesunieciePionka(string nazwaPionka, string nazwaKartySciezki)
    {
        Debug.Log("CmdWykonajPrzesunieciePionka()");
        GameObject pionek = GameObject.Find($"{nazwaPionka}");
        GameObject kartaNaKtoraPrzesunacPionek = GameObject.Find($"{nazwaKartySciezki}");

        pionek.transform.position = kartaNaKtoraPrzesunacPionek.transform.position;

        RpcWykonajPrzesunieciePionka(nazwaPionka, nazwaKartySciezki);

        GameObject systemTur = GameObject.Find($"SystemTur(Clone)");

        //CmdZmniejszIloscPozostalychRuchowOjeden();
        ZmniejszIloscPozostalychRuchowOjeden();

        if (systemTur.GetComponent<SystemTur>().IleRuchowPionkiemJeszczeZostalo < 1)
        {
            UstawKrokPrzesunieciaKrasnoludaNaWykonany();
        }
    }

    [ClientRpc]
    public void RpcWykonajPrzesunieciePionka(string nazwaPionka, string nazwaKartySciezki)
    {
        GameObject kartaNaKtoraChcePrzesunacPionek = GameObject.Find($"{nazwaKartySciezki}");
        if (kartaNaKtoraChcePrzesunacPionek == null)
        {
            kartaNaKtoraChcePrzesunacPionek = GameObject.Find($"Talia_KartyStartu/{nazwaKartySciezki}");
        }
        GameObject pionek = GameObject.Find($"{nazwaPionka}");

        (kartaNaKtoraChcePrzesunacPionek.GetComponent<CardData>().ScriptableKarta as KartaSciezki).ListaPionkowStojacychNaTejKarcie.Add(pionek);
        (pionek.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje.GetComponent<CardData>().ScriptableKarta as KartaSciezki).ListaPionkowStojacychNaTejKarcie.Remove(pionek);
        pionek.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje = kartaNaKtoraChcePrzesunacPionek;

    }

    public void PolozKarteNaPoluPlanszy()
    {
        RaycastKartaIPolePlanszy();
        if (kartaKliknieta == null || polePlanszyKlikniete == null) { return; }

        CardData cardData = kartaKliknieta.GetComponent<CardData>();
        PoleMapy poleMapy = polePlanszyKlikniete.GetComponent<PoleMapy>();

        // zapobiegniêcie ruszania karty œcie¿ki, która ju¿ le¿y na planszy
        foreach (GameObject gameObject in polaPlanszyList)
        {
            PoleMapy skryptPoleMapy = gameObject.GetComponent<PoleMapy>();
            if (kartaKliknieta == skryptPoleMapy.KartaPolozonaNaPolu)
            {
                ResetKliknietejKartyIKliknietegoPolaPlanszy();
                Debug.Log("Ta karta ju¿ le¿y na planszy, nie mo¿na jej przesun¹æ!");
                return;
            }
        }

        // sprawdzenie czy mo¿na kartê do³o¿yæ do œcie¿ki, czy œcie¿ki pasuj¹
        PoleMapy poleLeweSkrypt = null;
        PoleMapy polePraweSkrypt = null;
        PoleMapy poleGorneSkrypt = null;
        PoleMapy poleDolneSkrypt = null;

        int liczbaKartDoOkola = 0;

        GameObject karta_lewa_GameObject = null;
        GameObject karta_prawa_GameObject = null;
        GameObject karta_gorna_GameObject = null;
        GameObject karta_dolna_GameObject = null;
        GameObject karta_sciezka_kladziona_GameObject = kartaKliknieta;

        KartaSciezki karta_lewa = null;
        KartaSciezki karta_prawa = null;
        KartaSciezki karta_gorna = null;
        KartaSciezki karta_dolna = null;
        KartaSciezki karta_sciezka_kladziona = kartaKliknieta.GetComponent<CardData>().ScriptableKarta as KartaSciezki;

        foreach (GameObject gameObject in polaPlanszyList)
        {
            PoleMapy skryptPoleMapy = gameObject.GetComponent<PoleMapy>();
            if (skryptPoleMapy.X == poleMapy.X - 1 && skryptPoleMapy.Y == poleMapy.Y)
            {
                poleLeweSkrypt = skryptPoleMapy;
                if (poleLeweSkrypt != null && poleLeweSkrypt.KartaPolozonaNaPolu != null) { karta_lewa_GameObject = poleLeweSkrypt.KartaPolozonaNaPolu; karta_lewa = poleLeweSkrypt.KartaPolozonaNaPolu.GetComponent<CardData>().ScriptableKarta as KartaSciezki; liczbaKartDoOkola++; }
            }
            if (skryptPoleMapy.X == poleMapy.X + 1 && skryptPoleMapy.Y == poleMapy.Y)
            {
                polePraweSkrypt = skryptPoleMapy;
                if (polePraweSkrypt != null && polePraweSkrypt.KartaPolozonaNaPolu != null) { karta_prawa_GameObject = polePraweSkrypt.KartaPolozonaNaPolu; karta_prawa = polePraweSkrypt.KartaPolozonaNaPolu.GetComponent<CardData>().ScriptableKarta as KartaSciezki; liczbaKartDoOkola++; }
            }
            if (skryptPoleMapy.X == poleMapy.X && skryptPoleMapy.Y == poleMapy.Y + 1)
            {
                poleGorneSkrypt = skryptPoleMapy;
                if (poleGorneSkrypt != null && poleGorneSkrypt.KartaPolozonaNaPolu != null) { karta_gorna_GameObject = poleGorneSkrypt.KartaPolozonaNaPolu; karta_gorna = poleGorneSkrypt.KartaPolozonaNaPolu.GetComponent<CardData>().ScriptableKarta as KartaSciezki; liczbaKartDoOkola++; }
            }
            if (skryptPoleMapy.X == poleMapy.X && skryptPoleMapy.Y == poleMapy.Y - 1)
            {
                poleDolneSkrypt = skryptPoleMapy;
                if (poleDolneSkrypt != null && poleDolneSkrypt.KartaPolozonaNaPolu != null) { karta_dolna_GameObject = poleDolneSkrypt.KartaPolozonaNaPolu; karta_dolna = poleDolneSkrypt.KartaPolozonaNaPolu.GetComponent<CardData>().ScriptableKarta as KartaSciezki; liczbaKartDoOkola++; }
            }
        }

        if (liczbaKartDoOkola == 0) { Debug.Log("Brak kart wokó³"); ResetKliknietejKartyIKliknietegoPolaPlanszy(); return; }
        Debug.Log("liczba kart dooko³a = " + liczbaKartDoOkola);

        // sprawdzenie lewego pola
        if (poleLeweSkrypt != null)
        {
            if (poleLeweSkrypt.KartaPolozonaNaPolu != null)
            {
                if (

                    (karta_lewa.Wschód == true
                    && karta_sciezka_kladziona.Zachód == false
                    && (!(karta_lewa is KartaKopalni) || (karta_lewa is KartaKopalni && (karta_lewa as KartaKopalni).CzyKopalniaOdkryta))
                    )

                    ||

                    (karta_lewa.Wschód == false
                    && karta_sciezka_kladziona.Zachód == true
                    && (!(karta_lewa is KartaKopalni) || (karta_lewa is KartaKopalni && (karta_lewa as KartaKopalni).CzyKopalniaOdkryta))
                    )

                    ||

                    (karta_lewa.Wschód == false
                    && karta_sciezka_kladziona.Zachód == false
                    && liczbaKartDoOkola == 1
                    )

                    ||

                    (karta_lewa is KartaKopalni
                    && (karta_lewa as KartaKopalni).CzyKopalniaOdkryta == false
                    && liczbaKartDoOkola == 1)

                    )
                {
                    Debug.Log("Warunek pole Lewe Skrypt");
                    ResetKliknietejKartyIKliknietegoPolaPlanszy();
                    return;
                }
            }
        }
        // sprawdzenie prawego pola
        if (polePraweSkrypt != null)
        {
            if (polePraweSkrypt.KartaPolozonaNaPolu != null)
            {
                if (

                    (karta_prawa.Zachód == true
                    && karta_sciezka_kladziona.Wschód == false
                    && (!(karta_prawa is KartaKopalni) || (karta_prawa is KartaKopalni && (karta_prawa as KartaKopalni).CzyKopalniaOdkryta)) //t¹ linijke doda³em, potestowaæ
                    )

                    ||

                    (karta_prawa.Zachód == false
                    && karta_sciezka_kladziona.Wschód == true
                    && (!(karta_prawa is KartaKopalni) || (karta_prawa is KartaKopalni && (karta_prawa as KartaKopalni).CzyKopalniaOdkryta))
                    )

                    ||

                    (karta_prawa.Zachód == false
                    && karta_sciezka_kladziona.Wschód == false
                    && liczbaKartDoOkola == 1
                    )

                    ||

                    (karta_prawa is KartaKopalni
                    && (karta_prawa as KartaKopalni).CzyKopalniaOdkryta == false
                    && liczbaKartDoOkola == 1
                    )

                   )
                {
                    Debug.Log("Warunek pole Prawe Skrypt");
                    ResetKliknietejKartyIKliknietegoPolaPlanszy();
                    return;
                }
            }
        }
        // sprawdzenie górnego pola
        if (poleGorneSkrypt != null)
        {
            if (poleGorneSkrypt.KartaPolozonaNaPolu != null)
            {
                if (

                    (karta_gorna.Po³udnie == true
                    && karta_sciezka_kladziona.Pó³noc == false
                    && (!(karta_gorna is KartaKopalni) || (karta_gorna is KartaKopalni && (karta_gorna as KartaKopalni).CzyKopalniaOdkryta))
                    )

                    ||

                    (karta_gorna.Po³udnie == false
                    && karta_sciezka_kladziona.Pó³noc == true
                    && (!(karta_gorna is KartaKopalni) || (karta_gorna is KartaKopalni && (karta_gorna as KartaKopalni).CzyKopalniaOdkryta))
                    )

                    ||

                    (karta_gorna.Po³udnie == false
                    && karta_sciezka_kladziona.Pó³noc == false
                    && liczbaKartDoOkola == 1)

                    ||

                    (karta_gorna is KartaKopalni
                    && (karta_gorna as KartaKopalni).CzyKopalniaOdkryta == false
                    && (liczbaKartDoOkola == 1 || (liczbaKartDoOkola == 2 && poleDolneSkrypt != null && poleDolneSkrypt.KartaPolozonaNaPolu != null && (karta_dolna as KartaKopalni).CzyKopalniaOdkryta == false))
                    )

                  )
                {
                    Debug.Log("Warunek pole Gorne Skrypt");
                    ResetKliknietejKartyIKliknietegoPolaPlanszy();
                    return;
                }
            }
        }
        // sprawdzenie dolnego pola
        if (poleDolneSkrypt != null)
        {
            if (poleDolneSkrypt.KartaPolozonaNaPolu != null)
            {
                if (

                    (karta_dolna.Pó³noc == true
                    && karta_sciezka_kladziona.Po³udnie == false
                    && (!(karta_dolna is KartaKopalni) || (karta_dolna is KartaKopalni && (karta_dolna as KartaKopalni).CzyKopalniaOdkryta))
                    )

                    ||

                    (karta_dolna.Pó³noc == false
                    && karta_sciezka_kladziona.Po³udnie == true
                    && (!(karta_dolna is KartaKopalni) || (karta_dolna is KartaKopalni && (karta_dolna as KartaKopalni).CzyKopalniaOdkryta))
                    )

                    ||

                    (karta_dolna.Pó³noc == false
                    && karta_sciezka_kladziona.Po³udnie == false
                    && liczbaKartDoOkola == 1
                    )

                    ||

                    (karta_dolna is KartaKopalni
                    && (karta_dolna as KartaKopalni).CzyKopalniaOdkryta == false
                    && liczbaKartDoOkola == 1
                    )

                   )
                {
                    Debug.Log("Warunek pole Dolne Skrypt");
                    ResetKliknietejKartyIKliknietegoPolaPlanszy();
                    return;
                }
            }
        }




        // po³o¿enie karty na planszy je¿eli jest kart¹ typu œcie¿ki
        if (cardData.ScriptableKarta._TypKarty == Karta.TypKarty.Œcie¿ka)
        {
            // zabezpieczenie przed po³o¿eniem karty przez innego gracza na pole, na które karta p³ynie leci jeszcze
            if (polePlanszyKlikniete.GetComponent<PoleMapy>().KartaPolozonaNaPolu != null)
            {
                return;
            }

            if (karta_prawa != null)
            {
                Debug.Log("Nast¹pi wykonanie warunku ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");
                Debug.Log($"Czy karta jest kart¹ kopalni: {(karta_prawa is KartaKopalni)}");
                Debug.Log($"Czy karta jest kart¹ œcie¿ki: {karta_prawa is KartaSciezki}");
                Debug.Log($"Czy karta jest kart¹ startu: {karta_prawa is KartaStartu}");
                Debug.Log($"Nazwa karty to: {karta_prawa.name}");
                if (karta_prawa is KartaKopalni)
                {
                    Debug.Log($"Czy karta jako karta Kopalni odkryta: {(karta_prawa as KartaKopalni).CzyKopalniaOdkryta}");
                }

                Debug.Log($"Liczba kart do oko³a: {liczbaKartDoOkola}");
            }
            else
            {
                Debug.Log("karta_prawa jest nullem");
            }

            if (karta_prawa != null && karta_prawa is KartaKopalni && (karta_prawa as KartaKopalni).CzyKopalniaOdkryta == false && liczbaKartDoOkola > 1 && (karta_sciezka_kladziona.Wschód == true))
            {
                Debug.Log("Jestem w warunku ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");
                CmdWypiszKomunikat();
                CmdOdkryjKopalnie(karta_prawa_GameObject);
            }
            if (karta_lewa != null && karta_lewa is KartaKopalni && (karta_lewa as KartaKopalni).CzyKopalniaOdkryta == false && liczbaKartDoOkola > 1 && (karta_sciezka_kladziona.Zachód == true))
            {
                Debug.Log("Jestem w warunku ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");
                CmdWypiszKomunikat();
                CmdOdkryjKopalnie(karta_lewa_GameObject);
            }
            if (karta_gorna != null && karta_gorna is KartaKopalni && (karta_gorna as KartaKopalni).CzyKopalniaOdkryta == false && liczbaKartDoOkola > 1 && (karta_sciezka_kladziona.Pó³noc == true))
            {
                Debug.Log("Jestem w warunku ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");
                CmdWypiszKomunikat();
                CmdOdkryjKopalnie(karta_gorna_GameObject);
            }
            if (karta_dolna != null && karta_dolna is KartaKopalni && (karta_dolna as KartaKopalni).CzyKopalniaOdkryta == false && liczbaKartDoOkola > 1 && (karta_sciezka_kladziona.Po³udnie == true))
            {
                Debug.Log("Jestem w warunku ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");
                CmdWypiszKomunikat();
                CmdOdkryjKopalnie(karta_dolna_GameObject);
            }

            //poleMapy.SetKartaPo³ozonaNaPolu(kartaKliknieta);   komentuje 28.04.2024 03:06

            Debug.Log("Nast¹pi CmdWlaczKarteKlientom(kartaKliknieta)-----------------------------------------------------------------------------------");
            CmdWlaczKarteKlientom(kartaKliknieta);

            //kartaKliknieta.transform.position = polePlanszyKlikniete.transform.position + new Vector3(0f, 0.1f, 0f);
            // wywolaj command, który zdejmie authority klientowi z karty i w³¹czy t¹ karte dla wszystkich
            Debug.Log("Nast¹pi CmdPrzypiszKarteDoPolaWszystkimGraczom(kartaKliknieta.GetComponent<CardData>().Id, poleMapy.X, poleMapy.Y)--------------------------------------------");
            CmdPrzypiszKarteDoPolaWszystkimGraczom(kartaKliknieta.GetComponent<CardData>().Id, poleMapy.X, poleMapy.Y);

            gracz.KartyNaRece.Remove(kartaKliknieta);

            CmdUsunKarteGraczowiZListyKartNaReceNaSerwerze(kartaKliknieta);

            //CmdUstawKrokZagraniaKartyNaWykonany();

            //kartaKliknieta.transform.position = polePlanszyKlikniete.transform.position + new Vector3(0f, 0.1f, 0f);


        }


        ResetKliknietejKartyIKliknietegoPolaPlanszy();
    }

    [Command]
    public void CmdWypiszKomunikat()
    {
        Debug.Log("Jestem w warunku!");
    }

    [Command]
    private void CmdOdkryjKopalnie(GameObject karta_kopalni)
    {
        Debug.Log("Nast¹pi odkrycie kopalnii...");
        CardData cardData = karta_kopalni.GetComponent<CardData>();
        cardData.OdkryjKarte();
        (cardData.ScriptableKarta as KartaKopalni).CzyKopalniaOdkryta = true;
        RpcUstawCzyKopalniaOdkrytaNaTrue(karta_kopalni);

        SystemTur systemTur = GameObject.Find("SystemTur(Clone)").GetComponent<SystemTur>();

        if ((karta_kopalni.GetComponent<CardData>().ScriptableKarta as KartaKopalni)._TypKopalni == KartaKopalni.TypKopalni.Smok)
        {
            foreach(NetworkGamePlayerLobby gracz in systemTur.ListaKolejnoscGraczy)
            {
                if(gracz.NrKolejnosci == systemTur.NrKolejnoscGracza)
                {
                    gracz.SetIloscPunktow(-2f);
                }
            }
        }
    }

    [ClientRpc]
    public void RpcUstawCzyKopalniaOdkrytaNaTrue(GameObject karta_kopalni)
    {
        Debug.Log("karta kopalni powinna zostac ustawiona na true na klientach");
        (karta_kopalni.GetComponent<CardData>().ScriptableKarta as KartaKopalni).CzyKopalniaOdkryta = true;
        Debug.Log($"Czy kopalnia odkryta = {(karta_kopalni.GetComponent<CardData>().ScriptableKarta as KartaKopalni).CzyKopalniaOdkryta}");
    }

    [Command]
    private void CmdUsunKarteGraczowiZListyKartNaReceNaSerwerze(GameObject kartaDoUsuniecia)
    {
        // usuñ karty graczowi z listy ale na serwerze
        foreach (NetworkGamePlayerLobby graczSkrypt in gracz.Room.GamePlayers)
        {
            foreach (GameObject gameObject in graczSkrypt.KartyNaRece)
            {
                if (kartaDoUsuniecia == gameObject)
                {
                    graczSkrypt.KartyNaRece.Remove(gameObject);
                    break;
                }
            }
        }
    }

    public void PrzesunPionekKrasnoluda()
    {
        RaycastKartaIPolePlanszy();
        if (kartaKliknieta == null) { ResetKliknietejKartyIKliknietegoPolaPlanszy(); return; }
        if (!(kartaKliknieta.GetComponent<CardData>().ScriptableKarta is KartaSciezki)) { ResetKliknietejKartyIKliknietegoPolaPlanszy(); return; }

        // zabronienie wejœcia na przeszkode na której nie ma znacznika
        if(kartaKliknieta.GetComponent<CardData>().ScriptableKarta is KartaPrzeszkody)
        {
            KartaPrzeszkody kartaPrzeszkody = kartaKliknieta.GetComponent<CardData>().ScriptableKarta as KartaPrzeszkody;

            if(!kartaPrzeszkody.GetCzyZnacznikUmieszczony())
            {
                return;
            }
            else if(kartaPrzeszkody.GetCzyZnacznikUmieszczony() && (kartaKliknieta.GetComponent<CardData>().ScriptableKarta as KartaSciezki).ListaPionkowStojacychNaTejKarcie.Count > 0)
            {
                return;
            }
        }

        // zabronienie wejscia na kamienie gdy juz ktos tam stoi
        if(kartaKliknieta.GetComponent<CardData>().ScriptableKarta is KartaSciezki)
        {
            KartaSciezki kartaSciezki = (kartaKliknieta.GetComponent<CardData>().ScriptableKarta as KartaSciezki);
            if (kartaSciezki._TypSciezki == KartaSciezki.TypSciezki.Kamienie && kartaSciezki.ListaPionkowStojacychNaTejKarcie.Count > 0)
            {
                return;
            }
        }

        Debug.Log("Zadeklarowanie zmiennych poleNaKtorymPionekStoi i poleNaKtoreChcePrzesunacPionek");
        PoleMapy poleNaKtorymPionekStoi = null;
        PoleMapy poleNaKtoreChcePrzesunacPionek = null;

        Debug.Log("Zadeklarowanie zmiennych polePrawe poleLewe poleGorne poleDolne");
        PoleMapy polePrawe = null;
        PoleMapy poleLewe = null;
        PoleMapy poleGorne = null;
        PoleMapy poleDolne = null;

        Debug.Log("Przed pêtl¹ foreach");
        foreach (GameObject gameObjectPole in polaPlanszyList)
        {
            Debug.Log("Weszliœmy do pêtli foreach");
            PoleMapy poleMapy = gameObjectPole.GetComponent<PoleMapy>();
            if (poleMapy.KartaPolozonaNaPolu == kartaKliknieta)
            {
                poleNaKtoreChcePrzesunacPionek = poleMapy;
                Debug.Log("poleNaKtoreChcePrzesunacPionek = poleMapy");
                //if (poleMapy.X - gracz.PionekKrasnolud.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje.)
            }
            if (poleMapy.KartaPolozonaNaPolu == gracz.PionekKrasnolud.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje)
            {
                poleNaKtorymPionekStoi = poleMapy;
                Debug.Log("poleNaKtorymPionekStoi = poleMapy");
            }
        }
        Debug.Log("Za pêtl¹ ju¿");

        // sprawdzenie czy jest tylko o 1 pole oddalone w osi X i w osi Y
        if (Math.Abs(poleNaKtorymPionekStoi.X - poleNaKtoreChcePrzesunacPionek.X) > 1) { ResetKliknietejKartyIKliknietegoPolaPlanszy(); return; }
        if (Math.Abs(poleNaKtorymPionekStoi.Y - poleNaKtoreChcePrzesunacPionek.Y) > 1) { ResetKliknietejKartyIKliknietegoPolaPlanszy(); return; }

        Debug.Log("Za warunkami z Math.Abs()");

        // jeœli X =1 to poleLewe, jeœli =-1 to polePrawe
        // jeœli Y =1 to poleDolne, jeœli =-1 to poleGorne
        if (((poleNaKtorymPionekStoi.X - poleNaKtoreChcePrzesunacPionek.X) == 1) && ((poleNaKtorymPionekStoi.Y - poleNaKtoreChcePrzesunacPionek.Y)) == 0)
        {
            poleLewe = poleNaKtoreChcePrzesunacPionek;
            Debug.Log("poleLewe = poleNaKtoreChcePrzesunacPionek");
        }
        if (((poleNaKtorymPionekStoi.X - poleNaKtoreChcePrzesunacPionek.X) == -1) && ((poleNaKtorymPionekStoi.Y - poleNaKtoreChcePrzesunacPionek.Y)) == 0)
        {
            polePrawe = poleNaKtoreChcePrzesunacPionek;
            Debug.Log("polePrawe = poleNaKtoreChcePrzesunacPionek");
        }
        if (((poleNaKtorymPionekStoi.X - poleNaKtoreChcePrzesunacPionek.X) == 0) && ((poleNaKtorymPionekStoi.Y - poleNaKtoreChcePrzesunacPionek.Y)) == 1)
        {
            poleDolne = poleNaKtoreChcePrzesunacPionek;
            Debug.Log("poleDolne = poleNaKtoreChcePrzesunacPionek");
        }
        if (((poleNaKtorymPionekStoi.X - poleNaKtoreChcePrzesunacPionek.X) == 0) && ((poleNaKtorymPionekStoi.Y - poleNaKtoreChcePrzesunacPionek.Y)) == -1)
        {
            poleGorne = poleNaKtoreChcePrzesunacPionek;
            Debug.Log("poleGorne = poleNaKtoreChcePrzesunacPionek");
        }

        // sprawdzenie czy œcie¿ka pozwala przejœæ (np. tam gdzie stoimy pó³noc true, a tam gdzie chcemy przejœæ po³udnie true)

        if (poleLewe == null && polePrawe == null && poleDolne == null && poleGorne == null) { ResetKliknietejKartyIKliknietegoPolaPlanszy(); return; }

        KartaSciezki kartaNaKtorejStoiPionek = poleNaKtorymPionekStoi.KartaPolozonaNaPolu.GetComponent<CardData>().ScriptableKarta as KartaSciezki;
        KartaSciezki kartaNaKtoraPionekChceSiePrzesunac = poleNaKtoreChcePrzesunacPionek.KartaPolozonaNaPolu.GetComponent<CardData>().ScriptableKarta as KartaSciezki;

        if (poleNaKtoreChcePrzesunacPionek == poleLewe)
        {
            if (kartaNaKtorejStoiPionek.Zachód != true || kartaNaKtoraPionekChceSiePrzesunac.Wschód != true)
            {
                ResetKliknietejKartyIKliknietegoPolaPlanszy();
                return;
            }
        }
        else if (poleNaKtoreChcePrzesunacPionek == polePrawe)
        {
            if (kartaNaKtorejStoiPionek.Wschód != true || kartaNaKtoraPionekChceSiePrzesunac.Zachód != true)
            {
                ResetKliknietejKartyIKliknietegoPolaPlanszy();
                return;
            }
        }
        else if (poleNaKtoreChcePrzesunacPionek == poleDolne)
        {
            if (kartaNaKtorejStoiPionek.Po³udnie != true || kartaNaKtoraPionekChceSiePrzesunac.Pó³noc != true)
            {
                ResetKliknietejKartyIKliknietegoPolaPlanszy();
                return;
            }

        }
        else if (poleNaKtoreChcePrzesunacPionek == poleGorne)
        {
            if (kartaNaKtorejStoiPionek.Pó³noc != true || kartaNaKtoraPionekChceSiePrzesunac.Po³udnie != true)
            {
                ResetKliknietejKartyIKliknietegoPolaPlanszy();
                return;
            }
        }

        Debug.Log("Uda³o siê przejœæ przez wszystkie ify warunki. Gratulacjê, powinno siê udaæ przesun¹æ pionek");

        // pozwól siê przesunaæ pionkowi
        CmdWykonajPrzesunieciePionka(gracz.PionekKrasnolud.name, kartaKliknieta.name);

        ResetKliknietejKartyIKliknietegoPolaPlanszy();
    }

    public void DobierzKarty()
    {
        GameObject systemTurGameObject = GameObject.Find("SystemTur(Clone)");
        SystemTur systemTurSkrypt = systemTurGameObject.GetComponent<SystemTur>();

        if (systemTurSkrypt.NrKolejnoscGracza != gracz.NrKolejnosci) { return; }
        if (systemTurSkrypt.ZagranieLubOdczucenie != 1 || systemTurSkrypt.RuchKrasnoludem != 1) { return; }

        CmdDobierzKartyGraczowi(this.gracz.GetDisplayName());

        //UstawKrokDobieraniaKartNaWykonany();
    }

    [Command]
    public void CmdDobierzKartyGraczowi(string nazwaGracz)
    {
        Debug.Log("CmdDobierzKartyGraczowi()");

        GameObject tasowanieKartGameObject = GameObject.Find("TasowanieKart");
        TasowanieKart tasowanieKartSkrypt = tasowanieKartGameObject.GetComponent<TasowanieKart>();

        GameObject cardManagerGameObject = GameObject.Find("CardManager");
        CardManager cardManagerSkrypt = cardManagerGameObject.GetComponent<CardManager>();

        GameObject networkManagerGameObject = GameObject.Find("NetworkManager");
        NetworkManagerLobby networkManagerSkrypt = networkManagerGameObject.GetComponent<NetworkManagerLobby>();

        //NetworkGamePlayerLobby graczDoPrzekazania = null;
        GameObject graczDoPrzekazania = null;

        // Zabroniæ jeœli nie ma ju¿ kart na stosie do dobierania
        if (cardManagerSkrypt.getStosKartDoDobierania().Count < 1)
        {
            Debug.Log("Skoñczy³y siê karty na stosie do dobierania");
            UstawKrokDobraniaKartNaWykonany();
            return;
        }

        foreach (NetworkGamePlayerLobby gracz in networkManagerSkrypt.GamePlayers)
        {
            if (gracz.GetDisplayName() == nazwaGracz)
            {
                //graczDoPrzekazania = gracz;
                graczDoPrzekazania = gracz.gameObject;
            }
        }

        NetworkGamePlayerLobby graczDoPrzekazaniaSkrypt = graczDoPrzekazania.GetComponent<NetworkGamePlayerLobby>();

        // ZABRONIÆ IF GRACZ MA KART NA RECE > 4 ( WIECEJ NI¯ 4) TO RETURN
        if (graczDoPrzekazaniaSkrypt.KartyNaRece.Count > 4) { return; }

        //GameObject graczGameObject = GameObject.Find($"{nazwaGracz}");
        //NetworkGamePlayerLobby graczSkrypt = graczGameObject.GetComponent<NetworkGamePlayerLobby>();

        Debug.Log($"Przekazana nazwa Gracza w parametrze to: {nazwaGracz}");

        // int liczba kart do rozdania (ile kart brakuje do 5)
        int liczbaKartDoRozdaniaGraczowi = 5 - graczDoPrzekazaniaSkrypt.KartyNaRece.Count;

        GameObject kartaDanaGraczowi = null;

        for (int i = 0; i < liczbaKartDoRozdaniaGraczowi; i++)
        {
            // tutaj zmieniæ iloœæ zrobiæ to w pêtli for() czy dobieramy 1 czy 2 razy wtedy pod t¹ linijk¹ w pêtli bêdzie druga linijka wywo³anie Rpc
            kartaDanaGraczowi = tasowanieKartSkrypt.DajGraczowiKarteZeStosuDoDobierania(graczDoPrzekazaniaSkrypt, cardManagerSkrypt.getStosKartDoDobierania());
            RpcGraczMaWczytacSobieKarteDoSwojejListyKartyNaRece(graczDoPrzekazania, kartaDanaGraczowi);
        }

        // teraz musze kazac graczowi:   (poprzez RPC) 
        // 1.  gracz ma dodac sobie kartê do listy
        // 2. odwrocic j¹ awersem do góry
        // 3. ulozyc karty na spawnpointach (odœwie¿yæ ) pierwsza karta z listy trafi na 1 spawnPoint, itd. 5 karta trafi na 5 spawnPoint

        Debug.Log($"Karta dana graczowi: {kartaDanaGraczowi.name}");

        UstawKrokDobraniaKartNaWykonany();
    }

    [ClientRpc]
    public void RpcGraczMaWczytacSobieKarteDoSwojejListyKartyNaRece(GameObject graczGameObject, GameObject kartaGameObject) // przetestowaæ czy w ogole te parametry przechodz¹ i da sie z nich wyci¹gnaæ informacje
    {
        Debug.Log("RpcGraczMaWczytacSobieKarteDoSwojejListyKartyNaRece");

        Debug.Log($"Gracz ze skryptu: {gracz.GetDisplayName()}");
        Debug.Log($"Gracz z parametru: {graczGameObject.GetComponent<NetworkGamePlayerLobby>().GetDisplayName()}");

        if (gracz.GetDisplayName() != graczGameObject.GetComponent<NetworkGamePlayerLobby>().GetDisplayName()) { Debug.Log("gracz != graczGameObject return"); return; }

        if (isClientOnly)
        {
            gracz.KartyNaRece.Add(kartaGameObject);
            Debug.Log($"{kartaGameObject.name} zosta³a dodana do listy KartyNaRece");
        }

        SystemTur systemTur = GameObject.Find("SystemTur(Clone)").GetComponent<SystemTur>();

        kartaGameObject.GetComponent<CardData>().OdkryjKarte();

        Debug.Log("Jestem po Odkryciu karty WAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

        Debug.Log($"W³¹czê teraz karcie {kartaGameObject.name} BoxCollider, MeshRenderer, Awers i Rewers");

        if (kartaGameObject.GetComponent<NetworkIdentity>().hasAuthority)
        {
            Debug.Log($"Mam authority nad kart¹ {kartaGameObject.name}");
            kartaGameObject.GetComponent<BoxCollider>().enabled = true;
            kartaGameObject.GetComponent<MeshRenderer>().enabled = true;
            // w³¹cz rewers i w³¹cz awers
            kartaGameObject.transform.GetChild(0).gameObject.SetActive(true);
            kartaGameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            Debug.Log($"Nie mam auhority nad kart¹ {kartaGameObject.name}");
        }

        // JAK ZROBIÆ ¯EBY SERWER W£¥CZA£ TYLKO SWOJE KARTY ?????????????????

        //if(gracz.NrKolejnosci == systemTur.NrKolejnoscGracza)
        //if (isServer && gracz.KartyNaRece.Contains(kartaGameObject))
        //{
        //    kartaGameObject.GetComponent<BoxCollider>().enabled = true;
        //    kartaGameObject.GetComponent<MeshRenderer>().enabled = true;
        //    // w³¹cz rewers i w³¹cz awers
        //    kartaGameObject.transform.GetChild(0).gameObject.SetActive(true);
        //    kartaGameObject.transform.GetChild(1).gameObject.SetActive(true);
        //}


        int i = 0;
        foreach (GameObject gameObject in gracz.KartyNaRece)
        {
            if (!gameObject.GetComponent<NetworkIdentity>().hasAuthority) { continue; }
            gameObject.transform.position = GameManager.Instance.SpawnPointsKartyNaRece[i].position;
            Debug.Log($"{gameObject.name} zostal przesuniety na pozycje x={GameManager.Instance.SpawnPointsKartyNaRece[i].position.x} y={GameManager.Instance.SpawnPointsKartyNaRece[i].position.y} z={GameManager.Instance.SpawnPointsKartyNaRece[i].position.z}");
            i++;
            Debug.Log($"i = {i}");
        }
    }

    //[Command]
    public void UstawKrokDobraniaKartNaWykonany()
    {
        Debug.Log("UstawKrokDobieraniaKartNaWykonany");
        gracz.SystemTur.DobranieKart = 1;
        gracz.SystemTur.Stan++;
        RpcResetujWszystkimKarteKliknietaIPoleKlikniete();
    }

    [Command]
    private void CmdPrzypiszKarteDoPolaWszystkimGraczom(int idKarty, int xPola, int yPola)
    {
        RpcPrzypiszKarteDoPolaGraczowi(idKarty, xPola, yPola);
    }

    [ClientRpc]
    private void RpcPrzypiszKarteDoPolaGraczowi(int idKarty, int xPola, int yPola)
    {
        List<GameObject> listaKart = new List<GameObject>();
        for (int i = 0; i < 63; i++)
        {
            GameObject karta = GameObject.Find($"Karta ({i})(Clone)");
            if (karta != null)
            {
                Debug.Log($"karta != null, wiêc karta {karta.name} dodana do listyKart");
                listaKart.Add(karta);
            }
        }

        Debug.Log($"Ilosc elementow na listaKart: {listaKart.Count}");
        Debug.Log($"Ilosc elementow na liœcie PolaPlanszyList: {polaPlanszyList.Count}");

        GameObject gameObjectPoleMapy = null;
        PoleMapy pole = null;

        List<GameObject> listaPolPlanszy = new List<GameObject>();

        GameObject mapa = GameObject.Find("Mapa");
        if (mapa != null && listaPolPlanszy.Count < 1)
        {
            for (int i = 0; i < mapa.transform.childCount; i++)
            {
                GameObject pojedynczePole = mapa.transform.GetChild(i).gameObject;
                listaPolPlanszy.Add(pojedynczePole);
                Debug.Log($"Pole zosta³o dodane do listy {pojedynczePole.gameObject.name}");
            }
        }
        else
        {
            Debug.Log("Nie uda³o siê znaleŸæ MAPY !!!");
        }

        Debug.Log($"Lista pol planszy na liœcie listaPolPlanszy = {listaPolPlanszy.Count}");

        foreach (GameObject gameObject in listaPolPlanszy)
        {
            PoleMapy poleMapy = gameObject.GetComponent<PoleMapy>();
            if (poleMapy.X == xPola && poleMapy.Y == yPola)
            {
                Debug.Log($"Pola mapy X i Y: {poleMapy.X} = {xPola} | {poleMapy.Y} = {yPola}");
                pole = poleMapy;
                gameObjectPoleMapy = gameObject;
            }
        }

        GameObject gameObjectKarta = null;

        foreach (GameObject karta in listaKart)
        {
            CardData cardData = karta.GetComponent<CardData>();
            if (cardData != null)
            {
                if (cardData.Id == idKarty)
                {
                    gameObjectKarta = karta;
                }
            }
        }

        if (pole != null) { Debug.Log($"pole != null"); } else { Debug.Log("pole = null"); }
        if (gameObjectKarta != null) { Debug.Log($"gameObjectKarta != null"); } else { Debug.Log("gameObjectKarta = null"); }
        if (gameObjectPoleMapy != null) { Debug.Log($"gameObjectPoleMapy != null"); } else { Debug.Log("gameObjectPoleMapy = null"); }


        if (pole != null && gameObjectKarta != null)
        {
            Debug.Log($"Karta {gameObjectKarta.name} zostanie przypisana do pola {gameObjectPoleMapy.name}.....................");
            // ustawienie karty na polu (logicznie)
            gameObjectPoleMapy.GetComponent<PoleMapy>().SetKartaPo³ozonaNaPolu(gameObjectKarta);
        }

        if (gameObjectPoleMapy != null && gameObjectKarta != null)
        {
            Debug.Log($"{gameObjectKarta.name} zostanie przypisana do {gameObjectPoleMapy.name}");
            // ustawienie karty na polu (fizycznie graficznie)

            CmdUstawPozycjeKarty(gameObjectKarta, gameObjectPoleMapy.transform.position + new Vector3(0f, 0.1f, 0f));
            //CmdUsunAuthorityKarty(gameObjectKarta, gameObjectPoleMapy.transform.position + new Vector3(0f, 0.1f, 0f));

            //gameObjectKarta.transform.position = gameObjectPoleMapy.transform.position + new Vector3(0f, 0.1f, 0f); //Komentuje 11.05.2024 godz 01:45 wy¿ej podajê to do serwera w parametrach
        }
    }

    [Command]
    public void CmdUsunAuthorityKarty(GameObject karta)
    {
        Debug.Log("Usuwam authority karty");

        karta.GetComponent<NetworkIdentity>().RemoveClientAuthority();


    }

    [Command]
    public void CmdUstawPozycjeKarty(GameObject karta, Vector3 pozycja)
    {
        RpcUstawTrasnformPositionKarty(karta, pozycja);
    }

    [ClientRpc]
    public void RpcUstawTrasnformPositionKarty(GameObject karta, Vector3 pozycja)
    {
        if (!karta.GetComponent<NetworkIdentity>().hasAuthority) { return; }
        karta.transform.position = pozycja;
        //CmdUsunAuthorityKarty(karta);
        CmdUstawKrokZagraniaKartyNaWykonany();
    }

    [Command]
    private void CmdWlaczKarteKlientom(GameObject karta)
    {
        RpcKomunikat("LOL----------------------------------------------------------------");
        RpcKomunikat($"Przekazana karta w parametrze to: {karta.name}");
        //karta.GetComponent<NetworkIdentity>().RemoveClientAuthority();   // z tym niestety karty by³y ustawiane w róg mapy dziwnie
        karta.SetActive(true);

        karta.GetComponent<BoxCollider>().enabled = true;
        karta.GetComponent<MeshRenderer>().enabled = true;
        // znajdz awers i rewers i wylacz
        karta.transform.GetChild(0).gameObject.SetActive(true);
        karta.transform.GetChild(1).gameObject.SetActive(true);

        RpcWlaczKarteWszystkim(karta);

        //gracz.SystemTur.ZagranieLubOdczucenie = 1;
    }

    [Command]
    private void CmdUstawKrokZagraniaKartyNaWykonany()
    {
        gracz.SystemTur.ZagranieLubOdczucenie = 1;
        gracz.SystemTur.Stan++;
        RpcResetujWszystkimKarteKliknietaIPoleKlikniete();
    }

    [ClientRpc]
    private void RpcResetujWszystkimKarteKliknietaIPoleKlikniete()
    {
        ResetKliknietejKartyIKliknietegoPolaPlanszy();
        gracz.CmdOdswiezTabeleGraczyUI();
    }

    private void UstawKrokZagraniaKartyNaWykonany()
    {
        gracz.SystemTur.ZagranieLubOdczucenie = 1;
        gracz.SystemTur.Stan++;
        ResetKliknietejKartyIKliknietegoPolaPlanszy();
        //ResetKartyNarzedziaIKartyPrzeszkody();
    }

    //[Command]
    private void /*Cmd*/UstawKrokPrzesunieciaKrasnoludaNaWykonany()
    {
        gracz.SystemTur.RuchKrasnoludem = 1;
        gracz.SystemTur.Stan++;
    }

    [Command]
    public void CmdPrzyciskPominRuchPionka()
    {
        //GameObject systemTurGameobject = GameObject.Find("SystemTur(Clone)");
        //SystemTur systemTurSkrypt = systemTurGameobject.GetComponent<SystemTur>();

        //if (gracz.NrKolejnosci != systemTurSkrypt.NrKolejnoscGracza) { return; }
        if (gracz.NrKolejnosci != gracz.SystemTur.NrKolejnoscGracza) { return; }

        //if (systemTurSkrypt.ZagranieLubOdczucenie != 1) { return; }
        if (gracz.SystemTur.ZagranieLubOdczucenie != 1) { return; }

        //systemTurSkrypt.RuchKrasnoludem = 1;
        //systemTurSkrypt.Stan++;
        UstawKrokPrzesunieciaKrasnoludaNaWykonany(); //mo¿liwe, ¿e to mo¿e jednak zostaæ, a te 2 linijki wy¿ej usun¹æ i 2 od wyszukuna systemuTur te¿ usun¹æ
        RpcResetujWszystkimKarteKliknietaIPoleKlikniete();
    }

    //[Command]
    private void /*Cmd*/ZmniejszIloscPozostalychRuchowOjeden()
    {
        RpcKomunikat("CmdZmniejszIloscPozostalychRuchowOjeden");
        GameObject systemTur = GameObject.Find("SystemTur(Clone)");
        SystemTur systemTurSkrypt = systemTur.GetComponent<SystemTur>();

        systemTurSkrypt.SetIleRuchowPionkiemJeszczeZostalo(systemTurSkrypt.IleRuchowPionkiemJeszczeZostalo - 1);
        RpcKomunikat($"Ruchy zostaly pomniejszone o 1 , teraz jest ich = {systemTurSkrypt.IleRuchowPionkiemJeszczeZostalo}");
    }

    [ClientRpc]
    public void RpcKomunikat(string wiadomosc)
    {
        Debug.Log(wiadomosc);
    }

    [ClientRpc]
    private void RpcWlaczKarteWszystkim(GameObject karta)
    {
        karta.SetActive(true);
    }

    private void ResetKliknietejKartyIKliknietegoPolaPlanszy()
    {
        Debug.Log("ResetKliknietejKartyIKliknietegoPolaPlanszy()");
        kartaKliknieta = null;
        polePlanszyKlikniete = null;
    }

    public void RaycastKartaIPolePlanszy()
    {
        // Sprawdzanie klikniêcia myszk¹
        if (Input.GetMouseButtonDown(0))
        {
            // Wykrywanie klikniêtego obiektu
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                niezydentifikowanyObiekt = clickedObject;
                if (clickedObject.GetComponent<PoleMapy>() != null)
                {
                    polePlanszyKlikniete = clickedObject;
                }
                else if (clickedObject.GetComponent<CardData>() != null)
                {
                    if (kartaKliknieta == clickedObject) { return; }
                    kartaKliknieta = clickedObject;
                    polePlanszyKlikniete = null;
                }
            }
        }
    }

    public void RaycastKartaSkarbu()
    {
        // Sprawdzanie klikniêcia myszk¹
        if (Input.GetMouseButtonDown(0))
        {
            // Wykrywanie klikniêtego obiektu
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject == null) { return; }

                CardData cardData = null;
                if(clickedObject.TryGetComponent<CardData>(out cardData))
                {
                    if (!(cardData.ScriptableKarta is KartaSkarbu)) { return; }
                }

                if (cardData == null) { return; }

                

                if (kartaSkarbuKliknieta == clickedObject) { return; }
                kartaSkarbuKliknieta = clickedObject;               
            }
        }
    }

    public void ZagrajKarteNarzedzia()
    {
        if (gracz.SystemTur.NrKolejnoscGracza != gracz.NrKolejnosci) { return; }

        RaycastKartaNarzedziaIKartaPrzeszkodyNaPlanszy();       

        if (kartaNarzedziaKliknieta == null || kartaPrzeszkodyNaKtoraChcemyUzycKartyNarzedzia == null) { return; }

        bool czyMoznaPrzejscDalej = false;

        foreach (GameObject polePlanszyGameObject in polaPlanszyList)
        {
            PoleMapy poleMapySkrypt = polePlanszyGameObject.GetComponent<PoleMapy>();

            if(poleMapySkrypt.KartaPolozonaNaPolu == kartaPrzeszkodyNaKtoraChcemyUzycKartyNarzedzia)
            {
                czyMoznaPrzejscDalej = true;
            }
        }

        if (!czyMoznaPrzejscDalej) { ResetKartyNarzedziaIKartyPrzeszkody(); return; }

        KartaNarzedzia.TypNarzedzia typNarzedzia = (kartaNarzedziaKliknieta.GetComponent<CardData>().ScriptableKarta as KartaNarzedzia).GetTypNarzedzia();
        KartaPrzeszkody.TypPrzeszkody typPrzeszkody = (kartaPrzeszkodyNaKtoraChcemyUzycKartyNarzedzia.GetComponent<CardData>().ScriptableKarta as KartaPrzeszkody).GetTypPrzeszkody();

        if ((int)typNarzedzia != (int)typPrzeszkody) { return; }

        Debug.Log("CmdZagrajKarteNarzedzia() teraz powinno siê rozpocz¹æ");

        if (czyKartaNarzedziZagrana) { return; }
        CmdZagrajKarteNarzedzia(kartaNarzedziaKliknieta, kartaPrzeszkodyNaKtoraChcemyUzycKartyNarzedzia, gracz.GetDisplayName());

        gracz.KartyNaRece.Remove(kartaNarzedziaKliknieta);

        czyKartaNarzedziZagrana = true;

        //ResetKartyNarzedziaIKartyPrzeszkody();
    }

    [Command]
    public void CmdZagrajKarteNarzedzia(GameObject kartaNarzedzia, GameObject kartaPrzeszkody, string nazwaGracza)
    {
        NetworkManagerLobby networkManagerLobby = GameObject.Find("NetworkManager").GetComponent<NetworkManagerLobby>();

        foreach(NetworkGamePlayerLobby gracz in networkManagerLobby.GamePlayers)
        {
            if(gracz.GetDisplayName() == nazwaGracza)
            {
                gracz.KartyNaRece.Remove(kartaNarzedzia);
            }
        }

        KartaNarzedzia.TypNarzedzia typNarzedzia = (kartaNarzedzia.GetComponent<CardData>().ScriptableKarta as KartaNarzedzia).GetTypNarzedzia();

        CardManager cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();

        GameObject znacznik = null;

        if(typNarzedzia == KartaNarzedzia.TypNarzedzia.Topór)
        {
            znacznik = cardManager.GetListaZnacznikiTopory()[0];
            cardManager.GetListaZnacznikiTopory().Remove(znacznik);
        }
        else if(typNarzedzia == KartaNarzedzia.TypNarzedzia.£ódŸ)
        {
            znacznik = cardManager.GetListaZnacznikiLodki()[0];
            cardManager.GetListaZnacznikiLodki().Remove(znacznik);
        }
        else if( typNarzedzia == KartaNarzedzia.TypNarzedzia.Lina)
        {
            znacznik = cardManager.GetListaZnacznikiLiny()[0];
            cardManager.GetListaZnacznikiLiny().Remove(znacznik);
        }

        if (znacznik == null) { return; }

        kartaNarzedzia.SetActive(true);
        kartaNarzedzia.GetComponent<BoxCollider>().enabled = true;
        kartaNarzedzia.GetComponent<MeshRenderer>().enabled = true;
        kartaNarzedzia.transform.GetChild(0).gameObject.SetActive(true);
        kartaNarzedzia.transform.GetChild(1).gameObject.SetActive(true);

        RpcZagrajKarteNarzedzia(kartaNarzedzia, kartaPrzeszkody, znacznik.name);

        UstawKrokZagraniaKartyNaWykonany();
    }

    [ClientRpc]
    public void RpcZagrajKarteNarzedzia(GameObject kartaNarzedzia, GameObject kartaPrzeszkody, string znacznik)
    {
        kartaNarzedzia.transform.position = GameManager.Instance.SpawnPointDlaKartOdrzuconych.position;
        kartaNarzedzia.SetActive(true);
        kartaNarzedzia.transform.GetChild(0).gameObject.SetActive(true);
        kartaNarzedzia.transform.GetChild(1).gameObject.SetActive(true);



        GameObject znacznikGameObject = GameObject.Find($"{znacznik}");

        // sprawdziæ znacznik  lub karte przeszkody, ale najpierw znacznik
        znacznikGameObject.transform.position = kartaPrzeszkody.transform.position;

        (kartaPrzeszkody.GetComponent<CardData>().ScriptableKarta as KartaPrzeszkody).SetCzyZnacznikUmieszczony(true);

        ResetKartyNarzedziaIKartyPrzeszkody();
    }

    public void ResetKartyNarzedziaIKartyPrzeszkody()
    {
        kartaNarzedziaKliknieta = null;
        kartaPrzeszkodyNaKtoraChcemyUzycKartyNarzedzia = null;
        czyKartaNarzedziZagrana = false;
    }

    public void RaycastKartaNarzedziaIKartaPrzeszkodyNaPlanszy()
    {
        // Sprawdzanie klikniêcia myszk¹
        if (Input.GetMouseButtonDown(0))
        {
            // Wykrywanie klikniêtego obiektu
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                if (clickedObject == null) { return; }
                
                CardData cardData = clickedObject.GetComponent<CardData>();
                if (cardData == null) { return; }

                if(cardData.ScriptableKarta is KartaNarzedzia)
                {
                    kartaNarzedziaKliknieta = clickedObject;
                }

                if (kartaNarzedziaKliknieta == null) { return; }

                if (cardData.ScriptableKarta is KartaPrzeszkody)
                {
                    kartaPrzeszkodyNaKtoraChcemyUzycKartyNarzedzia = clickedObject;
                }
                         
            }
        }
    }

    public void ZbierzKarteSkarbu()
    {
        Debug.Log("ZbierzKarteSkarbu()");
        if (gracz.SystemTur.NrKolejnoscGracza != gracz.NrKolejnosci) { return; }

        RaycastKartaSkarbu();

        GameObject kartaNaKtorejStoiPionek = gracz.PionekKrasnolud.GetComponent<PionekInfo>().KartaSciezkiNaKtorejStoje;
        CardData cardData = kartaNaKtorejStoiPionek.GetComponent<CardData>();
        Karta kartaScriptable = cardData.ScriptableKarta;

        if (!(kartaScriptable is KartaKopalni)) { return; }

        if (kartaSkarbuKliknieta == null) { return; }

        KartaSkarbu kartaSkarbu = (kartaSkarbuKliknieta.GetComponent<CardData>().ScriptableKarta as KartaSkarbu);

        KartaSkarbu.TypSkarbu typSkarbu = kartaSkarbu._TypSkarbu;
        KartaKopalni.TypKopalni typKopalni = (kartaScriptable as KartaKopalni)._TypKopalni;

        Debug.Log("Przed Ifem");
        if (((int)typKopalni) != (int)typSkarbu) { return; }
        Debug.Log("Za Ifem");

        CmdPrzekazKarteSkarbuGraczowi(kartaSkarbuKliknieta, gracz.GetDisplayName());

        ResetKliknietejKartySkarbu();
    }

    public void ResetKliknietejKartySkarbu()
    {
        kartaSkarbuKliknieta = null;
    }

    [Command]
    public void CmdPrzekazKarteSkarbuGraczowi(GameObject kartaSkarbu, string nazwaGracz)
    {
        GameObject systemTurGameObject = GameObject.Find("SystemTur(Clone)");
        SystemTur systemTur = systemTurGameObject.GetComponent<SystemTur>();

        CardManager cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();

        if (systemTur.WzieteKartySkarbu > 0) { return; }

        systemTur.WzieteKartySkarbu = 1;
       
        kartaSkarbu.transform.position = GameManager.Instance.SpawnPointDlaKartOdrzuconych.transform.position;

        GameObject networkManager = GameObject.Find("NetworkManager");
        NetworkManagerLobby networkManagerLobby = networkManager.GetComponent<NetworkManagerLobby>();

        foreach(NetworkGamePlayerLobby gracz in networkManagerLobby.GamePlayers)
        {
            if(gracz.GetDisplayName() == nazwaGracz)
            {
                gracz.KartySkarbu.Add(kartaSkarbu);
                cardManager.getListaKartSkarbuDoBrania().Remove(kartaSkarbu);
            }
        }
    }
}
