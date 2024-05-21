using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MJEGameManager : MonoBehaviour
{
    [Header("Gestión cuenta atrás")] 
    [SerializeField] private float tiempoMaximo = 65f;
    private float tiempoRestante;
    [SerializeField] private TextMeshProUGUI textoCuentaAtras;
    [SerializeField] public bool juegoEmpezado;
    [SerializeField] public bool juegoCompletado = false;
    
    [Header("Gestión puntuación")] 
    [SerializeField] private TextMeshProUGUI textoEnemigosRestantes;
    [SerializeField] public int puntuacionFinal;
    [SerializeField] public float ultimoRegistro;

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
        tiempoRestante = Time.time;
        juegoEmpezado = false;
        
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
        if (juegoEmpezado)
        {
            float elapsedTime = Time.time - tiempoRestante;
            float remainingTime = Mathf.Max(tiempoMaximo - elapsedTime, 0f);

            if (textoCuentaAtras != null && !juegoCompletado)
            {
                textoCuentaAtras.text = "" + Mathf.Ceil(remainingTime);
            }
            if (elapsedTime >= tiempoMaximo)
            {
                if(!juegoCompletado)
                    FinJuego();
            }   
        }
    }
    
    private string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time - seconds) * 1000f);
        return $"{seconds}.{milliseconds}";
    }

    public void ActualizarPuntuacion(int sumaPuntos)
    {
        enemigosDerrotados++;
        
        //Actualizamos el número de enemigos restantes
        textoEnemigosRestantes.text = (numeroEnemigosTotales - enemigosDerrotados).ToString();

        ultimoRegistro = Time.time-tiempoRestante;

        if (enemigosDerrotados == numeroEnemigosTotales)
        {
            FinJuego();
        }
    }
    
    public void ActivarJugabilidad()
    {
        juegoEmpezado = true;
        
        //Inicio música
        if (audioSourceSonidoFondo != null)
        {
            audioSourceSonidoFondo.Play();
        }
        
        CanvasInicio.gameObject.SetActive(false);
        CanvasJuego.gameObject.SetActive(true);
        
        textoEnemigosRestantes.text = numeroEnemigosTotales.ToString();
    }
    
    public void FinJuego()
    {
        juegoEmpezado = false;
        juegoCompletado = true;
        _jugadorController.controlBloqueado = true;
        
        CalculoPuntosFinales();

        CanvasFinal.gameObject.SetActive(true);
        if (enemigosDerrotados == numeroEnemigosTotales)
        {
            textoFinal.text = "Derrotaste a todos los enemigos. ¡ENHORABUENA! \n"+
                "Puntos conseguidos: "+puntuacionFinal.ToString()+" \n"+
                "Completado en: "+ FormatTime(ultimoRegistro) + " segundos";
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
        
        var infomacionJugador = LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<InformacionJugador>();
        infomacionJugador.SetMinigameScore(puntuacionFinal);
        infomacionJugador.CmdSetFinMinijuego(true);
        _globalGameManager.CheckAllPlayersWaiting();
    }

    public void CalculoPuntosFinales()
    {
        float auxPuntos = 60 - ultimoRegistro;
        auxPuntos += enemigosDerrotados * 60;
        auxPuntos *= 1000;
        puntuacionFinal = (int)auxPuntos;
        
    }
}
