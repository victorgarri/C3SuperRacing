using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ColorInterfaz : NetworkBehaviour
{
    [Header("InteriorCoche")]
    public GameObject interiorCoche;
    public List<Sprite> coloresInteriorCoche = new List<Sprite>();

    [Header("Velocimetro")] 
    public GameObject velocimetro;
    public List<Sprite> coloresVelocimetro = new List<Sprite>();
    
    
    // Start is called before the first frame update
    void Start()
    {
            if (coloresInteriorCoche.Count == coloresVelocimetro.Count)
            {
                int numeroRandom = Random.Range(0, coloresInteriorCoche.Count);
                interiorCoche.GetComponent<Image>().sprite = coloresInteriorCoche[numeroRandom];
                velocimetro.GetComponent<Image>().sprite = coloresVelocimetro[numeroRandom];
            }   
    }
}
