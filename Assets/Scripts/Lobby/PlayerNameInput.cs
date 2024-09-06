using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;
    [SerializeField] private Text komunikatText = null;

    public static string DisplayName { get; private set; }

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start()
    {
        SetUpInputField();

        UstawKomunikatBledu(); // ustawia treść błędu gdy serwer nas wyrzuci po złej walidacji
    }

    private void UstawKomunikatBledu()
    {
        GameObject obj = GameObject.Find("UI_MainMenu/Canvas_MainMenu/Panel_NameInput/Text_Error");
        if (obj != null)
        {
            //Debug.Log($"Tresc bledu: {PomocniczaKlasa.Instance.ErrorMessage}");
            obj.GetComponent<Text>().text = PomocniczaKlasa.Instance.ErrorMessage;
            //obj.GetComponent<Text>().text = "Niepoprawna nazwa!!!";
        }
    }

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        name = nameInputField.text;
        continueButton.interactable = !string.IsNullOrEmpty(name);     
    }

    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }
}

