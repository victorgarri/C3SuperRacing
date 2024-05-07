using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    private bool collected = false;
    public AudioClip checkpointRecogido;
    public AudioSource checkpointAudioSource;

    public bool IsCollected()
    {
        return collected;
    }

    public void Collect()
    {
        collected = true;

        checkpointAudioSource.clip = checkpointRecogido;
        checkpointAudioSource.Play();
        
        ChangeColor(Color.green);
    }
    
    private void ChangeColor(Color color)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}