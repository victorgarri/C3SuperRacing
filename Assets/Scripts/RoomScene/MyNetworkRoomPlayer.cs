using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] private ReadyStartController readyStartController;
    private MyNRM NRM;
    [SerializeField] private GameObject roomPlayerPanelPrefab;
    [SerializeField] private GameObject myPlayerPanel;
    
    [SerializeField] [SyncVar] public string playerName;

    [SerializeField] [SyncVar] public bool isSpectator = false;
    
    public override void Start()
    {
        base.Start();
        NRM = NetworkManager.singleton as MyNRM;
        
        readyStartController = GameObject.Find("BotoneraReadyStart").GetComponent<ReadyStartController>();

        if (isLocalPlayer)
        {
            LocalPlayerPointer.Instance.roomPlayer = this;
            readyStartController.btnReady.onClick.AddListener(delegate { PlayerReadyToggle();});
            readyStartController.playerNameInput.onValueChanged.AddListener(delegate(string newStringInput) { CmdPlayerSetName(newStringInput); });
            readyStartController.spectatorMode.onValueChanged.AddListener(delegate(bool newSpectatorValue) { CmdSetSpectator(newSpectatorValue); });
        }
        
        if (isServer)
            CmdCreatePlayerPanel();
    }

    [Command]
    private void CmdPlayerSetName(string newName)
    {
        if (!String.IsNullOrEmpty(newName))
        {
            playerName = newName;
            myPlayerPanel.GetComponent<RoomPanelJugador>().playerName = playerName;
            
        }
    }
    
    [Command]
    private void CmdSetSpectator(bool newSpectatorValue)
    {
        isSpectator = newSpectatorValue;        
    }

    [Command(requiresAuthority = false)]
    private void CmdCreatePlayerPanel()
    {
        myPlayerPanel = Instantiate(roomPlayerPanelPrefab, GameObject.Find("ZonaConjuntoJugadores").transform);
        NetworkServer.Spawn(myPlayerPanel);
    }


    void PlayerReadyToggle()
    {
        CmdChangeReadyState(!readyToBegin);
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (isServer)
            myPlayerPanel.GetComponent<RoomPanelJugador>().playerReady = newReadyState;
    }
}
