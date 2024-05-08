using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosicionCirculo : MonoBehaviour
{
    private Transform playerTransform;
    
    private float alturaCirculo = 200;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        float alturaActualizada = playerTransform.position.y + alturaCirculo;
        
        this.transform.position = new Vector3(playerTransform.position.x, alturaActualizada, playerTransform.position.z);
    }
}
