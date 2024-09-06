using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;

    [Header("SyncVar")]
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar][SerializeField] private bool isValidationErrorClient = false;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    [SerializeField]private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    private void Update()
    {
        if(isValidationErrorClient)
        {
            if(isClientOnly)
            {
                CmdRozlaczKLienta();
            }
        }
    }

    public override void OnStartAuthority()
    {
        //Debug.Log($"OnStartAuthority() - w NetworkRoomPlayerLobby");

        CmdSetDisplayName(PlayerNameInput.DisplayName);

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        //Debug.Log($"OnStartClient() - w NetworkRoomPlayerLobby");
        Room.RoomPlayers.Add(this);
        //Debug.Log($"NetworkRoomPlayer zostal dodany do listy RoomPlayers");
        //Debug.Log($"NetworkRoomPlayers na liœcie: {Room.RoomPlayers.Count}");

        UpdateDisplay();
        //Debug.Log($"Interfejs lobby zostal updatowany");

        //Debug.Log($"IsValidationErrorHost: {PomocniczaKlasa.Instance.IsValidationErrorHost}");
        if(PomocniczaKlasa.Instance.IsValidationErrorHost)
        {         
            RozlaczHosta();
            //Debug.Log($"Host zosta³ roz³¹czony");
        }
        //Debug.Log($"IsValidationErrorClient: {isValidationErrorClient}");
        if (isValidationErrorClient)
        {
            TargetRozlaczKienta();
            //Debug.Log($"Klient zosta³ roz³¹czony");
        }
    }

    public override void OnStopClient()
    {
        Debug.Log($"OnStopClient() wywolanie");
        Room.RoomPlayers.Remove(this);
        //Debug.Log($"RommPlayer zostal usuniety z listy: {this.connectionToClient.identity.assetId}");
        //Debug.Log($"RommPlayers na liœcie po usuniêciu: {Room.RoomPlayers.Count}");

        UpdateDisplay();
        //Debug.Log($"Interfejs lobby zostal zaaktualizowany po usunieciu RoomPlayer z listy");
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        //Debug.Log($"UpdateDisplay()");
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {      
        Debug.Log($"CmdSetDisplayName()");
        //RpcDebugLog("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        if (!ValidationDisplayName(displayName)) 
        {
            //RpcDebugLog("Tu powinno sie wykonac UstawIsValidationClientNaTrue()");
            UstawIsValidationClientNaTrue();
            //TargetPrzywrocPanelNameInput();
            if (isServer) 
            {
                //RpcDebugLog("Tu powinno sie wykonac UstawIsValidationHosttNaTrue() ClientRpc");
                //RpcDebugLog("Tu powinno sie wykonac UstawIsValidationHosttNaTrue() TargetRpc");
                UstawIsValidationHostNaTrue();
                //PrzywrocPanelNameInput();
                //RozlaczHosta();               
                //PomocniczaKlasa.Instance.NetworkManagerLobby.StopHost();
                //connectionToClient.Disconnect(); 
                Debug.Log("Po³¹czenie z serwerem zosta³o zamkniête");
                return;
            }
                       
            //TargetRozlaczKienta();
            //PomocniczaKlasa.Instance.NetworkManagerLobby.StopServer();
            //NetworkServer.Destroy(gameObject);
            //Debug.Log("aleeee jak tooo");
        }

        DisplayName = displayName;
    }

    [ClientRpc]
    private void RpcDebugLog(string tekst)
    {
        Debug.Log(tekst);
    }

    [TargetRpc]
    private void DebugLog2(string tekst)
    {
        Debug.Log(tekst);
    }

    [Command]
    private void RozlaczHosta()
    {
        PomocniczaKlasa.Instance.NetworkManagerLobby.StopHost();
        PomocniczaKlasa.Instance.ChangeScene(PomocniczaKlasa.Instance.SceneLobby);
        PomocniczaKlasa.Instance.IsValidationErrorHost = false;
    }

    [TargetRpc]
    private void TargetRozlaczKienta()
    {
        Debug.Log("TargetRozlaczKlienta() ");
        PomocniczaKlasa.Instance.NetworkManagerLobby.StopClient();
        Debug.Log("PomocniczaKlasa.Instance.NetworkManagerLobby.StopClient()");
        PomocniczaKlasa.Instance.ChangeScene(PomocniczaKlasa.Instance.SceneLobby);
        Debug.Log("PomocniczaKlasa.Instance.ChangeScene(PomocniczaKlasa.Instance.SceneLobby)");
        isValidationErrorClient = false;
        Debug.Log("Nast¹pi³o roz³¹czenie klienta");
        //Room.RoomPlayers.Remove(this);
        Room.RoomPlayers.Clear();
        Debug.Log("Gracz zostal usuniety z listy Room.RoomPlayers (this)");       
    }


    [Command]
    private void CmdRozlaczKLienta()
    {
        TargetRozlaczKienta();
    }

    [Command]
    private void UstawIsValidationHostNaTrue()
    {
        PomocniczaKlasa.Instance.IsValidationErrorHost = true;
        Debug.Log($"IsValidationErrorHost ustawiony na true");
    }

    [TargetRpc]
    private void UstawIsValidationClientNaTrue()
    {
        isValidationErrorClient = true;
        Debug.Log($"IsValidationErrorClient ustawiony na true");
    }

    [TargetRpc]
    private void TargetPrzywrocPanelNameInput()
    {
        PomocniczaKlasa.Instance.PanelNameInput.SetActive(true);
        Debug.Log("PanelNameInput zosta³ w³¹czony");
    }

    [Command]
    private void PrzywrocPanelNameInput()
    {
        PomocniczaKlasa.Instance.PanelNameInput.SetActive(true);
    }

    [TargetRpc]
    private void TargetUstawError(string trescErrora)
    {
        PomocniczaKlasa.Instance.ErrorMessage = trescErrora;
    }
    
    private bool ValidationDisplayName(string displayName)
    {
        Debug.Log($"ValidationDisplayName() wywolanie");
        if (displayName.Length < 3 || displayName.Length > 15)
        {
            TargetUstawError("Nazwa jest za krótka lub za d³uga!!!");
            return false;
        }
        foreach(NetworkRoomPlayerLobby networkRoomPlayerLobby in Room.RoomPlayers)
        {
            if(networkRoomPlayerLobby.DisplayName == displayName)
            {
                TargetUstawError("Nazwa jest ju¿ zajêta przez innego gracza w lobby!!!");
                return false;
            }
        }
        return true;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        Debug.Log($"CmdStartGame() wywolanie");
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }
}

