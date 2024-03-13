using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class M0GameManager : MonoBehaviour
{
    private float startTime;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI finalMessage;
    public GameObject messagePanel;

    private int piecesCollected = 0;
    private int lastPiecesCollected = 0;
    private float lastCollectedTime; 
    public int totalPieces = 4;
    public bool puzzleCompleted = false;

    private float maxTime = 60f;
    private float tiempoRegistrado;
    
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
    
    public void CollectPiece(GameObject piece)
    {
        piecesCollected++;
        lastPiecesCollected = piecesCollected;
        lastCollectedTime = Time.time; 
        Debug.Log("Pieza " + piecesCollected + " recogida");

        // Marcar la pieza como recolectada para evitar duplicados
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
        if (puzzleCompleted)
        {
            messagePanel.SetActive(true);
            gameObject.SetActive(false);
            finalMessage.text = "Minijuego completado en " + FormatTime(tiempoRegistrado) + " segundos\n¡Bien hecho!";
        }
        else
        {
            messagePanel.SetActive(true);
            gameObject.SetActive(false);
            finalMessage.text = $"Tiempo límite alcanzado\nÚltima pieza recogida en {FormatTime(lastCollectedTime)} segundos\nTotal de piezas: {lastPiecesCollected}";
        }
    }
    
    private string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time - seconds) * 1000f);
        return $"{seconds}.{milliseconds:D7}";
    }

    // Update is called once per frame
    void Update()
    {
        // Verificar si ha pasado el tiempo límite de tiempo
        float elapsedTime = Time.time - startTime;
        float remainingTime = Mathf.Max(maxTime - elapsedTime, 0f);

        // Actualizar el texto de la cuenta atrás
        if (countdownText != null)
        {
            countdownText.text = "Tiempo: " + Mathf.Ceil(remainingTime);
        }

        if (elapsedTime >= maxTime)
        {
            EndGame();
        }
    }
}
