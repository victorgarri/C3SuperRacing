using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MyNRM : NetworkRoomManager
{
    private Button btnStart;
    [SerializeField] public GameObject[] playerPrefabs;
    [SerializeField] private GameObject spectatorPrefab;
    
    
    // public readonly SyncList<MyNetworkRoomPlayer> roomPlayersList = new SyncList<MyNetworkRoomPlayer>();
    
    public override void OnRoomServerPlayersReady()
    {
        if (!btnStart)
            btnStart = GameObject.Find("BtnStart").GetComponent<Button>();
        
        btnStart.interactable = true;
    }

    public override void OnRoomServerPlayersNotReady()
    {
        if (!btnStart)
            btnStart = GameObject.Find("BtnStart").GetComponent<Button>();
        
        btnStart.interactable = false;
    }

    public void StartGame()
    {
        ServerChangeScene(GameplayScene);
        
    }
    
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        if (roomPlayer.GetComponent<MyNetworkRoomPlayer>().isSpectator)
            return Instantiate(spectatorPrefab);

        Transform startPos = GetStartPosition();
        var playerCar = Instantiate(playerPrefab, startPos.position, startPos.rotation); 
        
        return SetMaterialJugador(playerCar,roomPlayer);
    }
    
    private GameObject SetMaterialJugador(GameObject car,GameObject roomPlayer)
    {
        MyNetworkRoomPlayer networkRoomPlayer = roomPlayer.GetComponent<MyNetworkRoomPlayer>();
        car.GetComponent<InformacionJugador>().playerIndex = networkRoomPlayer.playerIndex;
        car.GetComponent<InformacionJugador>().colorJugador = networkRoomPlayer.playerColor;
        car.GetComponent<InformacionJugador>().playerColorMaterial = networkRoomPlayer.selectedColorMaterial;
        return car;
    }

    //
    // public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    // {
    //     var roomPlayer = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
    //
    //     roomPlayersList.Add(roomPlayer.GetComponent<MyNetworkRoomPlayer>());
    //     
    //     return roomPlayer;
    // }


    // public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    // {
    //     MyNetworkRoomPlayer roomPlayerComponent = roomPlayer.GetComponent<MyNetworkRoomPlayer>();
    //     return Instantiate(playerPrefabs[roomPlayerComponent.selectedCar]);
    // }
}
