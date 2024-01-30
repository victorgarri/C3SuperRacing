using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransicionManagerMin0 : MonoBehaviour
{
    private GameManager _gameManager;
    private bool cambioEscenarioActivado = false;
    
    [Header("TRANSICIÓN INICIAL")]
    public CanvasGroup canvasInstrucciones;
    
    [Header("TRANSICIÓN FINAL")]
    public CanvasGroup canvasEspera;
    public TextMeshProUGUI textoPunto;
    
    [Header("OTROS AJUSTES PARA AMBOS CANVAS")]
    public float fadeDuration = 1f;
    private float timer;
    private float duracionTransicion = 5f;

    [Header("AJUSTES PERSONAJES")] 
    public bool puedeMover;

    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    //Gestión de la transición inicial
    public void transicionInicio()
    {
        StartCoroutine(MuestraInstrucciones());
    }

    private IEnumerator MuestraInstrucciones()
    {
        canvasInstrucciones.gameObject.SetActive(true);
        puedeMover = false;
        
        yield return new WaitForSeconds(duracionTransicion);
        canvasInstrucciones.gameObject.SetActive(false);
        puedeMover = true;
    }
    
    //Gestión de la transición final
    public void transicionFinal()
    {
        puedeMover = false;
        if (timer == 0)
        {
            canvasEspera.gameObject.SetActive(true);
            StartCoroutine(PuntosTexto());
        }
        timer = timer + Time.deltaTime;
        canvasEspera.alpha = timer / fadeDuration;

        if (timer > fadeDuration + 1 && !cambioEscenarioActivado)
        {
            cambioEscenarioActivado = true;
            InvokeRepeating("cambioEscenario", 5f, 0);
        }
    }
    
    IEnumerator PuntosTexto()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            textoPunto.text = ".";
            yield return new WaitForSeconds(1f);
            textoPunto.text = "..";
            yield return new WaitForSeconds(1f);
            textoPunto.text = "...";
        }
    }

    private void cambioEscenario()
    {
        _gameManager.siguienteEscenario();
    }
}
