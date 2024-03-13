using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class M0GameManager : MonoBehaviour
{
    private float startTime;
    private Rigidbody2D rb;
    private Collider2D wrenchCollider;
    private GameObject wrench;
    private Transform wrenchTransform;
    private PlayerInput _playerInput;
    
    public int probabilidadCajaReforzada;
    public int probabilidadTnt;
    
    public GameObject cajaPrefab;
    public GameObject cajaReforzadaPrefab;
    public GameObject tntPrefab;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        
        GenerateRandomBoxes();
    }

    private void GenerateRandomBoxes()
    {
        GameObject[] spawnpoints = GameObject.FindGameObjectsWithTag("SP");

        foreach (GameObject spawnpoint in spawnpoints)
        {
            float randomValue = Random.Range(1, 101);
            string boxType;

            if (randomValue <= probabilidadTnt)
            {
                boxType = "Tnt";
            } 
            else if (randomValue <= probabilidadCajaReforzada)
            {
                boxType = "CajaReforzada";
            }
            else
            {
                boxType = "Caja";
            }

            InstantiateBox(boxType, spawnpoint.transform.position);
        }
    }
    
    private void InstantiateBox(string boxType, Vector3 position)
    {
        GameObject boxPrefab = GetBoxPrefab(boxType);
        BoxController boxController = Instantiate(boxPrefab, position, Quaternion.identity).GetComponent<BoxController>();

        if (boxController != null)
        {
            boxController.SetBoxType(boxType);
        }
    } 
    
    private GameObject GetBoxPrefab(string boxType)
    {
        switch (boxType)
        {
            case "Caja":
                return cajaPrefab;
            case "CajaReforzada":
                return cajaReforzadaPrefab;
            case "Tnt":
                return tntPrefab;
            default:
                return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
