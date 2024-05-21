using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfflineSceneUI : MonoBehaviour
{
    [SerializeField] private MyNRM myNrm;
    // Start is called before the first frame update
    public void StartHost()
    {
        myNrm.StartHost();
    }

    public void StartClient()
    {
        myNrm.StartClient();
    }

    public void SetNetworkAddress(string networkAddress)
    {
        myNrm.networkAddress = networkAddress;
    }

    public void StartServer()
    {
        myNrm.StartServer();
    }
}
