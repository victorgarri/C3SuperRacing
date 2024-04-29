using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MyNRM : NetworkRoomManager
{
    private Button btnStart;
    [SerializeField] private GameObject spectatorPrefab;
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
        return null;
    }
}
