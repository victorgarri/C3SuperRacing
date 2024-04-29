using UnityEngine;

public class LocalPlayerPointer : MonoBehaviour
{
    public static LocalPlayerPointer Instance { get; private set; }

    public MyNetworkRoomPlayer roomPlayer;
    public GameObject gamePlayerGameObject;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
