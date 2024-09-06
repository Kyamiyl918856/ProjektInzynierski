using UnityEngine;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby()
    {
        networkManager.StartHost();

        landingPagePanel.SetActive(false);
    }

    private void Start()
    {
        networkManager = PomocniczaKlasa.Instance.NetworkManagerLobby;
    }
}

