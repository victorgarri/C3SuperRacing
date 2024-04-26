using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] private ReadyStartController readyStartController;
    [SerializeField] private CamaraSeleccionCoche camaraSeleccionCoche;
    private MyNRM NRM;
    [SerializeField] private GameObject roomPlayerPanelPrefab;
    [SerializeField] private GameObject myPlayerPanel;
    
    [SerializeField] [SyncVar] public string playerName;
    
    [SerializeField] private Color[] colorByIndex;
    [SerializeField] [SyncVar] public Color playerColor;


    [SerializeField] public int selectedCar;
    
    public override void Start()
    {
        base.Start();
        NRM = NetworkManager.singleton as MyNRM;

        selectedCar = 0;
        
        playerColor = colorByIndex[index];
        gameObject.name = "RoomPlayer-" + index+1;
        
        readyStartController = GameObject.Find("BotoneraReadyStart").GetComponent<ReadyStartController>();
        camaraSeleccionCoche = GameObject.Find("CamaraVisualizacionCoches").GetComponent<CamaraSeleccionCoche>();

        if (isLocalPlayer)
        {
            readyStartController.btnReady.onClick.AddListener(delegate { PlayerReadyToggle();});
            readyStartController.playerNameInput.onValueChanged.AddListener(delegate(string newStringInput) { CmdPlayerSetName(newStringInput); });
            camaraSeleccionCoche.btnIzquierda.onClick.AddListener(delegate { UpdateSelectedCar(-1); });
            camaraSeleccionCoche.btnDerecha.onClick.AddListener(delegate { UpdateSelectedCar(+1); });
            
        }
        
        if (isServer){
            readyStartController.btnStart.gameObject.SetActive(true);
            readyStartController.btnStart.onClick.AddListener(delegate { NRM.StartGame(); });
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
