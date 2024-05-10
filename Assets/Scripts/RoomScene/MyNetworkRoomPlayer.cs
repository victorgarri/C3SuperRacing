using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] private ReadyStartController readyStartController;
    [SerializeField] private CamaraSeleccionCoche camaraSeleccionCoche;
    [SerializeField] private GameObject panelSeleccionCoche;
    [SerializeField] private GameObject inputNombreJugador;
    
    private MyNRM NRM;
    [SerializeField] private GameObject roomPlayerPanelPrefab;
    [SerializeField] private GameObject myPlayerPanel;
    
    [SerializeField] [SyncVar] public string playerName;

    [SerializeField] [SyncVar] public bool isSpectator = false;

    [SyncVar(hook = nameof(HookPlayerIndex))] public int playerIndex;
    [SerializeField] private Color[] colorByIndex;
    [SerializeField] private Material[] colorMaterialByIndex;
    [SerializeField] [SyncVar] public Color playerColor;


    [SerializeField] public int selectedCar;
    public Material selectedColorMaterial;

    public override void Start()
    {
        base.Start();
        NRM = NetworkManager.singleton as MyNRM;

        UpdatePlayerIndex();

        CmdUpdateSelectedCar(0);
        
        
        
        readyStartController = GameObject.Find("BotoneraReadyStart").GetComponent<ReadyStartController>();
        camaraSeleccionCoche = GameObject.Find("CamaraVisualizacionCoches").GetComponent<CamaraSeleccionCoche>();
        panelSeleccionCoche = GameObject.Find("SeleccionCochePanel");
        inputNombreJugador = GameObject.Find("NombreJugador");

        ChangeCarsBaseColor();
        if (isLocalPlayer)
        {
            LocalPlayerPointer.Instance.roomPlayer = this;
            readyStartController.btnReady.onClick.AddListener(delegate { PlayerReadyToggle();});
            readyStartController.playerNameInput.onValueChanged.AddListener(delegate(string newStringInput) { CmdPlayerSetName(newStringInput); });
            camaraSeleccionCoche.btnIzquierda.onClick.AddListener(delegate { CmdUpdateSelectedCar(-1); });
            camaraSeleccionCoche.btnDerecha.onClick.AddListener(delegate { CmdUpdateSelectedCar(+1); });
            
            readyStartController.spectatorMode.onValueChanged.AddListener(delegate(bool newSpectatorValue) { CmdSetSpectator(newSpectatorValue); });
            
        }
        
        if (isServer)
            CmdCreatePlayerPanel();
    }

    [Command]
    private void CmdUpdateSelectedCar(int change)
    {
        selectedCar += change;
        if (selectedCar < 0)
            selectedCar = NRM.playerPrefabs.Length - 1;

        selectedCar %= NRM.playerPrefabs.Length;
    }
    
    void ChangeCarsBaseColor()
    {
        if (!isLocalPlayer)return;
            
        string[] materialsToChange =
        {
            "Color Base","Color Base (Instance)",
            "Default-Material","Default-Material (Instance)",
            "P1","P1 (Instance)",
            "P2","P2 (Instance)",
            "P3","P3 (Instance)",
            "P4","P4 (Instance)",
            "P5","P5 (Instance)",
            "P6","P6 (Instance)",
            "P7","P7 (Instance)",
            "P8","P8 (Instance)"
        };
        var carModels = FindObjectsOfType<SlowRotationAnimation>();
        
        foreach (var car in carModels){
                
            var carRenderers = car.GetComponentsInChildren<Renderer>();
            foreach (var carRenderer in carRenderers)
            {
                var materials = carRenderer.materials;
                
                for (int i = 0; i < materials.Length; i++)
                    if (materialsToChange.Contains(materials[i].name))
                    {
                        // Debug.Log(selectedColorMaterial.name);
                        materials[i] = selectedColorMaterial;
                    }
            
                carRenderer.SetMaterials(new List<Material>(materials));
            }
        }        
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
        panelSeleccionCoche.SetActive(!newSpectatorValue);
        inputNombreJugador.SetActive(!newSpectatorValue);
        if(newSpectatorValue) CmdPlayerSetName("Espectador");
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
            else
            {
                roomPlayer.playerIndex = 0;
            }
        }
    }

    void HookPlayerIndex(int oldIndex, int newIndex)
    {
        playerIndex = newIndex;
        // Debug.Log("Hook");
        if(!isSpectator) selectedColorMaterial = colorMaterialByIndex[newIndex];
        ChangeCarsBaseColor();
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
