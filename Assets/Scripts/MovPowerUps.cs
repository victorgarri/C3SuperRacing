using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MovPowerUps : NetworkBehaviour
{
    private Rigidbody body;
    private float horizontalInput;
    private float rotationSpeed = 0.6f;

    private BoxCollider boxCollider;
    [SerializeField] private GameObject[] meshes;
    [SerializeField] private float reactivationTime=5;
    
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        this.transform.Rotate(new Vector3 (0, -180, 0)*rotationSpeed*Time.deltaTime);
    }


    [ClientRpc]
    public void RpcDeactivate()
    {
        boxCollider.enabled = false;
        foreach (GameObject meshGameObject in meshes)
        {
            meshGameObject.SetActive(false);
        }
        StartCoroutine(ReactivationTimer());   
    }
    
    private IEnumerator ReactivationTimer()
    {
        yield return new WaitForSeconds(reactivationTime);
        boxCollider.enabled = true;
        foreach (GameObject meshGameObject in meshes)
        {
            meshGameObject.SetActive(true);
        }
    }
}
