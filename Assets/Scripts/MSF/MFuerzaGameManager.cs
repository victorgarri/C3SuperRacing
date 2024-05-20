using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MFuerzaGameManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI finalMessage;
    public GameObject messagePanel;
    public int enemiesDestroyed;
    private float lastDestroyedTime;
    public float startTime;
    public float maxTime = 50f;
    private FriendsController friendsController;
    private PlayerControllerSF playerController;
    private EnemySpawner enemySpawner;
    private int score;
    private bool end = false;
    public AudioClip musicaFondo, finJuego;
    private AudioSource audioSource;
    private GameManager _globalGameManager;
    [SerializeField] private Image panelInicio;

    
    void Start()
    {
        _globalGameManager = GameObject.FindObjectOfType<GameManager>();
        startTime = Time.time;
        friendsController = FindObjectOfType<FriendsController>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        playerController = FindObjectOfType<PlayerControllerSF>();
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = musicaFondo;
        audioSource.loop = true;
        audioSource.pitch = 0.95f;
        audioSource.Play();
        StartCoroutine(TutorialPanel());
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float remainingTime = Mathf.Max(maxTime - elapsedTime, 0f);

        if (countdownText != null)
        {
            countdownText.text = "" + Mathf.Ceil(remainingTime);
        }

        if (elapsedTime >= maxTime)
        {
            if(!end)
                EndGame();
        }

        if (elapsedTime >= 5)
        {
            panelInicio.gameObject.SetActive(false);
        }
    }
    
    private void EndGame()
    {
        audioSource.clip = finJuego;
        audioSource.loop = false;
        audioSource.pitch = 1f;
        audioSource.Play();
        
        end = true;
        //CalculateScore();
        
        playerController.disableControls = true;
        messagePanel.SetActive(true);
        enemySpawner.gameObject.SetActive(false);
        
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.DestroyEnemiesAlTerminarPartida();
        }
        
        finalMessage.text = $"¡Bien hecho!\nHas derrotado {enemiesDestroyed} enemigos\nLos ciudadanos están a salvo";
        var infomacionJugador = LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<InformacionJugador>();
        infomacionJugador.SetMinigameScore(enemiesDestroyed);
        infomacionJugador.CmdSetFinMinijuego(true);
        _globalGameManager.CheckAllPlayersWaiting(infomacionJugador);
    }

    private string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time - seconds) * 10000000);
        return $"{seconds}.{milliseconds:D7}";
    }
    
    public void IncrementEnemiesDestroyed()
    {
        enemiesDestroyed++;
    }

    public void UpdateLastDestroyedTime()
    {
        lastDestroyedTime = Time.time;
    }
    
    void CalculateScore()
    {
        int baseScore = 1000000000;
        int damagePenalty = friendsController.damageReceived * 1000000;
        int enemyBonus = enemiesDestroyed * 1000;

        score = Mathf.Max(baseScore - damagePenalty + enemyBonus, 0);
    }
    
    private IEnumerator TutorialPanel()
    {        
        playerController.disableControls = true;
        yield return new WaitForSeconds(5);
        panelInicio.gameObject.SetActive(false);
        playerController.disableControls = false;
    }
}
