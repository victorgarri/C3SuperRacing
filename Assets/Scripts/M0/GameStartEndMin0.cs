using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameStartEndMin0 : MonoBehaviour
{
    public GameObject personaje;

    private GameManager _gameManager;

    public CanvasGroup canvas;
    public TextMeshProUGUI textoPunto;

    public float fadeDuration = 1f;
    private float timer;
    
    private bool acabado = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        primeraTransicion();
    }

    void primeraTransicion()
    {
        canvas.alpha = 1;
        while (timer <= 5)
        {
            timer = timer + Time.deltaTime;
        }
        canvas.gameObject.SetActive(false);
        timer = 0;
        canvas.alpha = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (acabado)
        {
            if (timer == 0)
            {
                canvas.gameObject.SetActive(true);
                StartCoroutine(PuntosTexto());
            }
            timer = timer + Time.deltaTime;
            canvas.alpha = timer / fadeDuration;

            if (timer > fadeDuration + 1)
            {
                InvokeRepeating("cambioEscenario", 5, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == personaje)
        {
            acabado = true;
            //_gameManager.siguienteEscenario();
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
