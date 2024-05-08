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
        MyNetworkRoomPlayer roomPlayerComponent = roomPlayer.GetComponent<MyNetworkRoomPlayer>();
        if (roomPlayerComponent.isSpectator)
            return Instantiate(spectatorPrefab);

        Transform startPos = GetStartPosition();
        
        var playerCar = Instantiate(playerPrefabs[roomPlayerComponent.selectedCar],startPos.position,startPos.rotation);
        
        return SetMaterialJugador(playerCar,roomPlayerComponent);
    }
    
    private GameObject SetMaterialJugador(GameObject car,MyNetworkRoomPlayer roomPlayerComponent)
    {
        Debug.Log(car.GetComponent<InformacionJugador>());
        car.GetComponent<InformacionJugador>().playerIndex = roomPlayerComponent.playerIndex;
        car.GetComponent<InformacionJugador>().colorJugador = roomPlayerComponent.playerColor;
        car.GetComponent<InformacionJugador>().playerColorMaterial = roomPlayerComponent.selectedColorMaterial;
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
