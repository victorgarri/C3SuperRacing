using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MJEGameManager : MonoBehaviour
{
    [Header ("Gestión cuenta atrás")]
    [SerializeField] private int tiempoRestante = 60;
    [SerializeField] public bool juegoCompletado;
    [SerializeField] private TextMeshProUGUI textoCuentaAtras;

    [Header("Gestión puntuación")] 
    [SerializeField] private TextMeshProUGUI textoEnemigosRestantes;
    [SerializeField] public int puntuacionFinal;
    [SerializeField] public int ultimoRegistro;

    [Header("Interfaz partida")] 
    [SerializeField] private Canvas CanvasInicio;
    [SerializeField] private Canvas CanvasJuego;
    [SerializeField] private Canvas CanvasFinal;
    [SerializeField] private TextMeshProUGUI textoFinal;

    [Header("Número enemigos")]
    [SerializeField] private int numeroEnemigosTotales;
    [SerializeField] private int enemigosDerrotados = 0;
    
    [Header("Script JugadorController")]
    [SerializeField] private JugadorController _jugadorController;

    [Header("Límite mapa")] 
    [SerializeField] public GameObject[] limiteMapa;

    [Header("Música y efectos de sonido")] 
    [SerializeField] private AudioSource audioSourceSonidoFondo;
    
    private GameManager _globalGameManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _globalGameManager = GameObject.FindObjectOfType<GameManager>();
        
        foreach (var cuadrado in limiteMapa)
        {
            cuadrado.SetActive(false);
        }
        
        numeroEnemigosTotales = GameObject.FindGameObjectsWithTag("Enemigo").Length;
        
        CanvasInicio.gameObject.SetActive(true);
        CanvasJuego.gameObject.SetActive(false);
        CanvasFinal.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActualizarPuntuacion(int sumaPuntos)
    {
        enemigosDerrotados++;
        
        //Actualizamos el número de enemigos restantes
        textoEnemigosRestantes.text = (numeroEnemigosTotales - enemigosDerrotados).ToString();

        ultimoRegistro = tiempoRestante;

        if (enemigosDerrotados == numeroEnemigosTotales)
        {
            Debug.Log("ActualizarPuntuacion");
            FinJuego();
        }
    }
    
    public IEnumerator CuentaAtras()
    {
        //Inicio música
        if (audioSourceSonidoFondo != null)
        {
            audioSourceSonidoFondo.Play();
        }
        
        CanvasInicio.gameObject.SetActive(false);
        CanvasJuego.gameObject.SetActive(true);
        
        textoEnemigosRestantes.text = numeroEnemigosTotales.ToString();
        
        while (tiempoRestante > 0)
        {
            if(!juegoCompletado) StopCoroutine(CuentaAtras());
            textoCuentaAtras.text = tiempoRestante.ToString();
            yield return new WaitForSeconds(1);
            tiempoRestante--;
        }
        
        textoCuentaAtras.text = tiempoRestante.ToString();
        Debug.Log("CuentaAtras");
        FinJuego();
    }
    
    public void FinJuego()
    {
        juegoCompletado = true;
        _jugadorController.controlBloqueado = true;
        
        CalculoPuntosFinales();

        CanvasFinal.gameObject.SetActive(true);
        if (enemigosDerrotados == numeroEnemigosTotales)
        {
            textoFinal.text = "Derrotastes a todos los enemigos. ¡ENHORABUENA! \n"+
                "Puntos conseguidos: "+puntuacionFinal.ToString()+" \n"+
                "Completado en: "+ (60 - ultimoRegistro).ToString()+ " segundos";
        }
        else
        {
            textoFinal.text = "¡¡¡GAME OVER!!! \n" +
                              "No derrotaste a todos los enemigos a tiempo. \n" +
                              "Puntos conseguidos: " + puntuacionFinal.ToString();
        }
        
        //Paro la música
        if (audioSourceSonidoFondo != null)
        {
            if(audioSourceSonidoFondo.isPlaying) 
                audioSourceSonidoFondo.Stop();
        }
        
        LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<InformacionJugador>().SetMinigameScore(puntuacionFinal);
        LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<InformacionJugador>().CmdSetFinMinijuego(true);
        _globalGameManager.CheckAllPlayersWaiting();
    }

    public void CalculoPuntosFinales()
    {
        int auxPuntos = 60 - ultimoRegistro;
        auxPuntos += enemigosDerrotados * 60;
        auxPuntos *= 1000;
        puntuacionFinal = auxPuntos;
        
    }
}
