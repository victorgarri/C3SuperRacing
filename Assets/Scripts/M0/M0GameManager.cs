using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class M0GameManager : MonoBehaviour
{
    private float startTime;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI finalMessage;
    public GameObject messagePanel;
    private int lastPiecesCollected = 0;
    public int totalPieces = 4;
    public bool puzzleCompleted = false;
    private int piecesCollected = 0;
    private float tiempoRegistrado;
    private float lastCollectedTime; 
    private bool end = false;
    public int gamePoints = 0;
    public float maxTime = 65f;
    public int probabilidadCajaReforzada;
    public int probabilidadTnt;
    public GameObject cajaPrefab;
    public GameObject cajaReforzadaPrefab;
    public GameObject tntPrefab;
    private PlayerController playerController;
    [SerializeField] private GameManager _globalGameManager;
    public AudioClip musicaFondo, finJuego;
    private AudioSource audioSource;
    [SerializeField] private Image panelInicio;
    
    void Start()
    {
        startTime = Time.time;
        playerController = FindObjectOfType<PlayerController>();
        // _globalGameManager = GameObject.FindObjectOfType<GameManager>();
        GenerateRandomBoxes();
        
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = musicaFondo;
        audioSource.loop = true;
        audioSource.Play();
        StartCoroutine(TutorialPanel());
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

            InstantiateBox(boxType, spawnpoint);
        }
    }
    
    private void InstantiateBox(string boxType, GameObject spawnpoint)
    {
        GameObject boxPrefab = GetBoxPrefab(boxType);
        BoxController boxController = Instantiate(boxPrefab, spawnpoint.transform.position, Quaternion.identity).GetComponent<BoxController>();
        boxController.transform.parent = this.transform.parent;
        boxController.spawnAudioSource = spawnpoint.GetComponent<AudioSource>();
            
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
    
    public void CollectPiece(GameObject piece)
    {
        piecesCollected++;
        lastCollectedTime = Time.time-startTime; 
        // Debug.Log("Pieza " + piecesCollected + " recogida");

        piece.GetComponent<CheckpointController>().Collect();

        if (piecesCollected == totalPieces)
        {
            puzzleCompleted = true;
            tiempoRegistrado = Time.time - startTime;
            EndGame();
        }
    }

    private void EndGame()
    {
        audioSource.clip = finJuego;
        audioSource.Play();
        
        end = true;
        CalculatePoints();
        
        if (puzzleCompleted)
        {
            messagePanel.SetActive(true);
            playerController.disableControls = true;
            finalMessage.text = "Minijuego completado en " + FormatTime(tiempoRegistrado) + " segundos\nHas conseguido "+gamePoints+" puntos!\n¡Bien hecho!";
        }
        else
        {
            messagePanel.SetActive(true);
            playerController.disableControls = true;
            finalMessage.text = $"Tiempo límite alcanzado\nÚltima pieza recogida en {FormatTime(lastCollectedTime)} segundos\nTotal de piezas: {piecesCollected}\nHas conseguido "+gamePoints+" puntos!";
        }
        
        LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<InformacionJugador>().SetMinigameScore(gamePoints);
        LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<InformacionJugador>().CmdSetFinMinijuego(true);
        _globalGameManager.CheckAllPlayersWaiting();
    }

    private void CalculatePoints()
    {
        float auxPoints= 60 - lastCollectedTime;        
        auxPoints += piecesCollected * 60;
        auxPoints *= 1000;
        gamePoints = (int)auxPoints;
    }
    
    private string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time - seconds) * 1000f);
        return $"{seconds}.{milliseconds}";
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float remainingTime = Mathf.Max(maxTime - elapsedTime, 0f);

        if (countdownText != null && !end)
        {
            countdownText.text = "" + Mathf.Ceil(remainingTime);
        }
        if (elapsedTime >= maxTime)
        {
            if(!end)
                EndGame();
        }
        
    }

    private IEnumerator TutorialPanel()
    {        
        playerController.disableControls = true;
        yield return new WaitForSeconds(5);
        panelInicio.gameObject.SetActive(false);
        playerController.disableControls = false;
    }
}
