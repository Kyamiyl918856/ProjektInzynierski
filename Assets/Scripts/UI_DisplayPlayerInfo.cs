using Mirror;
using Mirror.Cloud.Example;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DisplayPlayerInfo : MonoBehaviour
{
    [Header("TMP_Text")]
    [SerializeField] private TMP_Text nazwaGracza_textMeshPro;
    [SerializeField] private TMP_Text klasaGracza_textMeshPro;
    [SerializeField] private TMP_Text klanGracza_textMeshPro;
    [SerializeField] private TMP_Text wybranaKarta_textMeshPro;
    [SerializeField] private TMP_Text komunikatCzyjaTura_textMeshPro;
    [SerializeField] private TMP_Text komunikatJakiKrokWTurze_textMeshPro;

    [Header("Buttons")]
    [SerializeField] private Button button_pominRuchPionkiem;
    [SerializeField] private Button button_odrzucKarte;
    [SerializeField] private Button button_dalej;
    [SerializeField] private Button button_powrotDoMenu;

    [Header("TMP_Texts kolejne")]
    [SerializeField] private TMP_Text liczbaKartStosDobierania_textMeshPro;
    [SerializeField] private TMP_Text liczbaKartStosOdrzuconych_textMeshPro;
    [SerializeField] private TMP_Text liczbaGraczy_textMeshPro;
    [SerializeField] private List<TMP_Text> gracze_textMeshPro;

    [Header("TMP_Texts gracze wyniki koñcowe")]
    [SerializeField] private List<TMP_Text> gracz_1 = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> gracz_2 = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> gracz_3 = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> gracz_4 = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> gracz_5 = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> gracz_6 = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> gracz_7 = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> gracz_8 = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> gracz_9 = new List<TMP_Text>();
    [SerializeField] private List<List<TMP_Text>> listaGraczyPodsumowanie = new List<List<TMP_Text>>();

    [Header("Panel Wyniki Koñcowe")]
    [SerializeField] private GameObject panelWyniki;

    [Header("Systemy")]
    [SerializeField] private SystemTur systemTur = null;
    [SerializeField] private NetworkManagerLobby networkManagerLobby = null;

    NetworkGamePlayerLobby graczKtoregoJestTerazTura = null;

    void Start()
    {
        Debug.Log("UI_DisplayName - Start()");

        listaGraczyPodsumowanie.Add(gracz_1);
        listaGraczyPodsumowanie.Add(gracz_2);
        listaGraczyPodsumowanie.Add(gracz_3);
        listaGraczyPodsumowanie.Add(gracz_4);
        listaGraczyPodsumowanie.Add(gracz_5);
        listaGraczyPodsumowanie.Add(gracz_6);
        listaGraczyPodsumowanie.Add(gracz_7);
        listaGraczyPodsumowanie.Add(gracz_8);
        listaGraczyPodsumowanie.Add(gracz_9);

        //textMeshPro.text = gamePlayer.GetDisplayName();
        //nazwaGraczaGameObject.SetActive(true);
    }

    void Update()
    {
        if (systemTur == null)
        {
            GameObject gameObject = GameObject.Find("SystemTur(Clone)");
            if (gameObject != null)
            {
                systemTur = gameObject.GetComponent<SystemTur>();
            }
        }

        if(networkManagerLobby == null)
        {
            GameObject gameObject = GameObject.Find("NetworkManager");
            if(gameObject != null)
            {
                networkManagerLobby = gameObject.GetComponent<NetworkManagerLobby>();
            }
        }

        if (systemTur != null)
        {
            foreach (NetworkGamePlayerLobby gracz in networkManagerLobby.GamePlayers)
            {
                if(gracz.NrKolejnosci == systemTur.NrKolejnoscGracza)
                {
                    komunikatCzyjaTura_textMeshPro.text = $"Tura gracza: {gracz.GetDisplayName()}";
                    graczKtoregoJestTerazTura = gracz;

                    // ustaw komunikat jaki krok w turze
                    komunikatJakiKrokWTurze_textMeshPro.text = systemTur.ZwrocKomunikatJakiKrokWTurze();
                    // w³¹cz komunikat
                    komunikatJakiKrokWTurze_textMeshPro.gameObject.SetActive(true);
                }
                else
                {
                    // wy³¹cz komunikat o tym jaki krok w turze
                    //komunikatJakiKrokWTurze_textMeshPro.gameObject.SetActive(false);
                }
            }
        }

        //if(networkManagerLobby)
    }

    public void SetUI_NazwaGracza(string nazwaGracza)
    {
        nazwaGracza_textMeshPro.text = nazwaGracza;
    }

     public void SetUI_KlasaGracza(string klasaGracza)
    {
        klasaGracza_textMeshPro.text = klasaGracza;
    }

    public void SetUI_KlanGracza(string klanGracza)
    {
        klanGracza_textMeshPro.text = klanGracza;
    }

    public void SetUI_WybranaKarta(string wybranaKarta)
    {
        wybranaKarta_textMeshPro.text = wybranaKarta;
    }

    public void SetUI_LiczbaKartNaStosieDoDobierania(int value)
    {
        liczbaKartStosDobierania_textMeshPro.text = value.ToString();
    }

    public void SetUI_LiczbaKartNaStosieKartOdrzuconych(int value)
    {
        liczbaKartStosOdrzuconych_textMeshPro.text = value.ToString();
    }

    public void SetUI_LiczbaGraczy(int value)
    {
        liczbaGraczy_textMeshPro.text = value.ToString();
    }

    public void SetUI_TabelaGraczy(string[] tablicaImionGraczy)
    {
        if (systemTur == null) { return; }

        

        int i = 0;
        foreach(TMP_Text tMP_Text in gracze_textMeshPro)
        {
            if(tMP_Text.text == "Jakas nazwa gracza")
            {
                tMP_Text.text = "";
            }
            //i++;
        }

        i = 0;
        foreach(string nazwaGracza in tablicaImionGraczy)
        {
            if (nazwaGracza == graczKtoregoJestTerazTura.GetDisplayName() )
            {
                //gracze_textMeshPro[i].text = $"<color=green>{nazwaGracza}</color>";
                gracze_textMeshPro[i].text = $"{nazwaGracza}";
                //Debug.Log("Nazwy te same, bedzie kolor zielony");
            }
            else
            {
                gracze_textMeshPro[i].text = $"{nazwaGracza}";
                //Debug.Log($"Nazwy ró¿ne niestety {nazwaGracza} vs {nazwaGracza_textMeshPro.text}");
            }
            i++;
        }
    }

    public Button GetButtonPominRuchPionkiem()
    {
        return button_pominRuchPionkiem;
    }

    public Button GetButtonOdrzucKarte()
    {
        return button_odrzucKarte;
    }

    public Button GetButtonDalej()
    {
        return button_dalej;
    }

    public Button GetButtonPowrotDoMenu()
    {
        return button_powrotDoMenu;
    }

    public GameObject GetPanelWyniki()
    {
        return panelWyniki;
    }

    public List<List<TMP_Text>> GetListaGraczyPodsumowanie()
    {
        return listaGraczyPodsumowanie;
    }

}
