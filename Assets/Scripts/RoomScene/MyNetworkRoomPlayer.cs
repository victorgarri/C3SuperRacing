using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] private ReadyStartController readyStartController;
    [SerializeField] private CamaraSeleccionCoche camaraSeleccionCoche;
    private MyNRM NRM;
    [SerializeField] private GameObject roomPlayerPanelPrefab;
    [SerializeField] private GameObject myPlayerPanel;
    
    [SerializeField] [SyncVar] public string playerName;

    [SerializeField] [SyncVar] public bool isSpectator = false;

    [SyncVar] public int playerIndex;
    [SerializeField] private Color[] colorByIndex;
    [SerializeField] private Material[] colorMaterialByIndex;
    [SerializeField] [SyncVar] public Color playerColor;


    [SerializeField] public int selectedCar;
    public Material selectedColorMaterial;

    public override void Start()
    {
        base.Start();
        NRM = NetworkManager.singleton as MyNRM;

        selectedCar = 0;

        UpdatePlayerIndex();
        
        readyStartController = GameObject.Find("BotoneraReadyStart").GetComponent<ReadyStartController>();
        camaraSeleccionCoche = GameObject.Find("CamaraVisualizacionCoches").GetComponent<CamaraSeleccionCoche>();

        if (isLocalPlayer)
        {
            LocalPlayerPointer.Instance.roomPlayer = this;
            readyStartController.btnReady.onClick.AddListener(delegate { PlayerReadyToggle();});
            readyStartController.playerNameInput.onValueChanged.AddListener(delegate(string newStringInput) { CmdPlayerSetName(newStringInput); });
            camaraSeleccionCoche.btnIzquierda.onClick.AddListener(delegate { UpdateSelectedCar(-1); });
            camaraSeleccionCoche.btnDerecha.onClick.AddListener(delegate { UpdateSelectedCar(+1); });
            
            readyStartController.spectatorMode.onValueChanged.AddListener(delegate(bool newSpectatorValue) { CmdSetSpectator(newSpectatorValue); });
        }
        
        if (isServer)
            CmdCreatePlayerPanel();
    }

    private void UpdateSelectedCar(int change)
    {
        selectedCar += change;
        if (selectedCar < 0)
            selectedCar = NRM.playerPrefabs.Length - 1;

        selectedCar %= NRM.playerPrefabs.Length;
        
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
        UpdatePlayerIndex();
    }

    [Command(requiresAuthority = false)]
    private void CmdCreatePlayerPanel()
    {
        myPlayerPanel = Instantiate(roomPlayerPanelPrefab, GameObject.Find("ZonaConjuntoJugadores").transform);
        NetworkServer.Spawn(myPlayerPanel);
    }

    
    private void UpdatePlayerIndex()
    {
        int counter = 0;
        foreach (var networkRoomPlayer in NRM.roomSlots)
        {
            var roomPlayer = (MyNetworkRoomPlayer)networkRoomPlayer;
            if (!roomPlayer.isSpectator)
            {
                roomPlayer.playerIndex = counter;
                roomPlayer.playerColor = colorByIndex[counter];
                roomPlayer.selectedColorMaterial = colorMaterialByIndex[counter];
                counter++;
            }
        }
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
