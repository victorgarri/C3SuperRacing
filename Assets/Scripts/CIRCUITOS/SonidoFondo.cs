using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidoFondo : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField] public List<AudioClip> listaDeReproduccion = new List<AudioClip>();
    
    [SerializeField] private AudioClip musicaVictoria;
    [SerializeField] private AudioClip musicaDerrota;

    [SerializeField] private float volumenMusica = 0.5f;

    private CarController _carController;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = this.GetComponent<AudioSource>();
    }

    public void ReproducirMusicaVelocidadNormal(int indice)
    {
        _audioSource.pitch = 1f;

        switch (indice)
        {
            case 1:
                volumenMusica = 0.5f;
                break;
            
            case 2:
                volumenMusica = 0.3f;
                break;
            
            case 3:
                volumenMusica = 0.3f;
                break;
        }
        
        if(!_audioSource.isPlaying)
            _audioSource.PlayOneShot(listaDeReproduccion[indice-1], volumenMusica);

    }

    public void ReproducirMusicaVelocidadRapida()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.pitch = 1.3f;
        
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
