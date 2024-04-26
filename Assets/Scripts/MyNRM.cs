using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MyNRM : NetworkRoomManager
{
    private Button btnStart;
    [SerializeField] public GameObject[] playerPrefabs;
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
        return Instantiate(playerPrefabs[roomPlayerComponent.selectedCar]);
    }
}
