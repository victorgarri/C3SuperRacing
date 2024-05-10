using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidoFondo : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField] AudioClip listaMusicaCircuito1;
    [SerializeField] private AudioClip listaMusicaCircuito2;
    [SerializeField] private AudioClip listaMusicaCircuito3;
    
    public List<AudioClip> listaDeReproduccion;
    
    [SerializeField] private AudioClip musicaVictoria;
    [SerializeField] private AudioClip musicaDerrota;
    
    private bool reproduccionVelocidadNormal = true;

    [SerializeField] private float volumenMusica = 0.5f;

    private CarController _carController;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = this.GetComponent<AudioSource>();
        
        listaDeReproduccion = new List<AudioClip>();
        listaDeReproduccion.Add(listaMusicaCircuito1);
        listaDeReproduccion.Add(listaMusicaCircuito2);
        listaDeReproduccion.Add(listaMusicaCircuito3);
    }

    public void ReproducirMusicaVelocidadNormal(int indice)
    {
        _audioSource.pitch = 1f;

        switch (indice)
        {
            case 1:
                volumenMusica = 1;
                break;
            
            case 2:
                volumenMusica = 0.5f;
                break;
            
            case 3:
                volumenMusica = 0.5f;
                break;
        }
        
        _audioSource.PlayOneShot(listaDeReproduccion[indice-1], volumenMusica);

    }

    public void ReproducirMusicaVelocidadRapida()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.pitch = 1.5f;
        
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
