using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidoFondo : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField] private AudioClip listaMusicaCircuito1;
    [SerializeField] private AudioClip listaMusicaCircuito2;
    [SerializeField] private AudioClip listaMusicaCircuito3;
    
    public List<AudioClip> listaDeReproduccion;
    
    [SerializeField] private AudioClip musicaVictoria;
    [SerializeField] private AudioClip musicaDerrota;

    public int indiceMusica = 0;  //Esta variable la llamará desde el GameManager
    private bool reproduccionVelocidadNormal = true;

    [SerializeField] private float volumenMusica = 0.5f;

    private CarController _carController;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = this.GetComponent<AudioSource>();
        
        listaDeReproduccion = new List<AudioClip>();
        listaDeReproduccion.Add(listaMusicaCircuito1);
        //listaDeReproduccion.Add(listaMusicaCircuito2);
        //listaDeReproduccion.Add(listaMusicaCircuito3);

        _audioSource.loop = true;
    }

    public void ReproducirMusicaVelocidadNormal()
    {
        _audioSource.PlayOneShot(listaDeReproduccion[indiceMusica], volumenMusica); 
        reproduccionVelocidadNormal = true;

    }

    public void ReproducirMusicaVelocidadRapida()
    {
        if (_audioSource.isPlaying && reproduccionVelocidadNormal)
        {
            _audioSource.Stop();
            _audioSource.PlayOneShot(listaDeReproduccion[indiceMusica], volumenMusica);
            reproduccionVelocidadNormal = false;
        }
    }

    public void PararMusicaFondo()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
}
