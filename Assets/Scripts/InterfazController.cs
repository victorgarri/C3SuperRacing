using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class InterfazController : MonoBehaviour
{
    [Header("InteriorCoche")]
    public GameObject interiorCoche;
    public List<Sprite> coloresInteriorCoche = new List<Sprite>();

    [Header("Velocímetro")] 
    public GameObject velocimetro;
    public List<Sprite> coloresVelocimetro = new List<Sprite>();
    
    [Header("Agujas del velocímetro")]
    private const float LIMITEANGULOIZQUIERDO = 190f;
    private const float LIMITEANGULODERECHO = -100f;
    public Transform agujaVelocimetro;
    public float velocidadNormal;
    
    [Header("Texto para indicar la posición en la que vas")]
    public TextMeshProUGUI textoPosicion;
    
    [Header("Texto para indicar las vueltas")]
    public TextMeshProUGUI textoVueltas;

    [Header("Gestión cuando el usuario vaya en sentido contrario")]
    public GameObject imagenProhibido;

    [Header("Minimapa")] 
    public Camera camaraMinimapa;
    public List<GameObject> listaCircuitosMinimapa;
    
    public Coroutine stopCor;
    public bool corBool;
    
    // Start is called before the first frame update
    void Start()
    {
        if (coloresInteriorCoche.Count == coloresVelocimetro.Count)
        {
            int numeroRandom = Random.Range(0, coloresInteriorCoche.Count);
            interiorCoche.GetComponent<Image>().sprite = coloresInteriorCoche[numeroRandom];
            velocimetro.GetComponent<Image>().sprite = coloresVelocimetro[numeroRandom];
        }
        
        imagenProhibido.SetActive(false);
           
        listaCircuitosMinimapa[0].SetActive(true);  //Circuito 1
        listaCircuitosMinimapa[1].SetActive(false); //Circuito 2
        listaCircuitosMinimapa[2].SetActive(false); //Circuito 3
    }

    
    public void AgujaVelocimetro(float velocidad, float VELOCIDADMAXIMA)
    {
        velocidadNormal = velocidad / VELOCIDADMAXIMA;

        agujaVelocimetro.localEulerAngles = new Vector3(0, 0, 
            Mathf.Lerp(LIMITEANGULOIZQUIERDO, LIMITEANGULODERECHO, velocidadNormal));   
    }
    
    //Método para mostra las posicion por pantalla a cada jugador 
    public void ActualizaPosicion(int posicion)
    {
        switch (posicion)
        {
            case 1:
                textoPosicion.color = HexToColor("#FFD700"); //Color dorado
                break;
            case 2:
                textoPosicion.color = HexToColor("#BEBEBE"); //Color plateado
                break;
            case 3:
                textoPosicion.color = HexToColor("#CD7F32"); //Color bronce
                break;
            case 4:
                textoPosicion.color = HexToColor("#FFFFFF"); //Color blanco
                break;
            case 5:
                textoPosicion.color = HexToColor("#ffdfd4"); //Color tono rojo suave
                break;
            case 6:
                textoPosicion.color = HexToColor("#ff7b5a"); //Color tono rojo medio
                break;
            case 7:
                textoPosicion.color = HexToColor("#ff5232"); //Color tono rojo duro
                break;
            case 8:
                textoPosicion.color = HexToColor("#ff0000"); //Color rojo puro
                break;
        }

        textoPosicion.text = posicion + ".";
    }
    Color HexToColor(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    public void ActualizaNumVueltas(int vueltaActual, int vueltaTotales)
    {
        textoVueltas.text = vueltaActual + "/" + vueltaTotales;
    }
    
    public IEnumerator activarProhibicion()
    {
        corBool = true;
        yield return new WaitForSeconds(1.5f);
        imagenProhibido.SetActive(true);
    }

    public void desactivarProhibicion()
    {
        imagenProhibido.SetActive(false);
        corBool = false;
        if (stopCor!=null)
            StopCoroutine(stopCor);        
    }

    public void cambiosMinimapa(int indice)
    {
        for (int i = 0; i < listaCircuitosMinimapa.Count; i++)
        {
            if(i==indice)
                listaCircuitosMinimapa[i].SetActive(true);
            else
                listaCircuitosMinimapa[i].SetActive(false);
            if(indice==2)
                camaraMinimapa.transform.position = new Vector3(-87f, 100, 40f);
        }
    }
}
