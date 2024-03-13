using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class InterfazController : NetworkBehaviour
{
    [Header("InteriorCoche")]
    public GameObject interiorCoche;
    public List<Sprite> coloresInteriorCoche = new List<Sprite>();

    [Header("Velocimetro")] 
    public GameObject velocimetro;
    public List<Sprite> coloresVelocimetro = new List<Sprite>();
    
    [Header("Agujas del velocímetro")]
    private const float LIMITEANGULOIZQUIERDO = 190f;
    private const float LIMITEANGULODERECHO = -100f;
    public Transform agujaVelocimetro;
    
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

    public void AgujaVelocimetro(float velocidad, float VELOCIDADMAXIMA)
    {
        float velocidadNormal = velocidad / VELOCIDADMAXIMA;

        agujaVelocimetro.localEulerAngles = new Vector3(0, 0, 
            Mathf.Lerp(LIMITEANGULOIZQUIERDO, LIMITEANGULODERECHO, velocidadNormal));   
    }
}
