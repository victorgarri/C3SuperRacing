using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = GameObject.Find("Starts/C1").transform.position;
        transform.rotation = GameObject.Find("Starts/C1").transform.rotation;
    }

    public void MoveSpectatorToTrack(int index)
    {
        Transform nextTransform = GameObject.Find("SpectatorLocations/Starts/C" + index).transform;
        transform.position = nextTransform.position;
        transform.rotation = nextTransform.rotation;
    }
}
