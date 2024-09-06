using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PomocniczaKlasa : MonoBehaviour
{
    private static PomocniczaKlasa instance;
    [SerializeField] private GameObject panelNameInput;
    [SerializeField] private NetworkManagerLobby networkManagerLobby;
    [Scene][SerializeField] private string sceneLobby = string.Empty;
    [SerializeField] private bool isValidationErrorHost = false;
    [SerializeField] private bool isValidationErrorClient = false;
    [SerializeField] private string errorMessage = string.Empty;

    public static PomocniczaKlasa Instance
    {
        get
        {
            return instance;
        }
    }

    public GameObject PanelNameInput
    {
        get { return panelNameInput; }
    }

    public NetworkManagerLobby NetworkManagerLobby
    {
        get { return networkManagerLobby; }
    }

    public string SceneLobby
    {
        get { return sceneLobby; }
    }

    public bool IsValidationErrorHost
    {
        get { return isValidationErrorHost; }
        set { isValidationErrorHost = value; }
    }

    public bool IsValidationErrorClient
    {
        get { return isValidationErrorClient; }
        set { isValidationErrorClient = value; }
    }

    public string ErrorMessage
    {
        get { return errorMessage; }
        set { errorMessage = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        GameObject obj = GameObject.Find("NetworkManager");
        if (obj != null)
        {
            networkManagerLobby = obj.GetComponent<NetworkManagerLobby>();
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    
}
