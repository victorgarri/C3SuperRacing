using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SerializeField] private ReadyStartController readyStartController;
    
    
    public override void OnGUI()
    {
        OnGUIBase();
    }

    // public override void Start()
    // {
    //     base.Start();
    //     readyStartController = GameObject.Find("BotoneraReadyStart").GetComponent<ReadyStartController>();
    //     readyStartController.btnReady.onClick.AddListener(delegate { PlayerReadyToggle();});
    //
    // }

    public void OnGUIBase()
    {
        NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
        if (room)
        {
            if (Utils.IsSceneActive(room.RoomScene))
            {
                readyStartController = GameObject.Find("BotoneraReadyStart").GetComponent<ReadyStartController>();
                Debug.Log("AddListener");
                readyStartController.btnReady.onClick.AddListener(delegate { PlayerReadyToggle();});
                
            }
        }
    }

    void PlayerReadyToggle()
    {
        Debug.Log("PlayerReadyToggle");
        if (readyToBegin)
        {
            CmdChangeReadyState(false);
            readyStartController.isReady = false;

        }
        else
        {
            CmdChangeReadyState(true);
            readyStartController.isReady = true;
        }
    }

    
}
