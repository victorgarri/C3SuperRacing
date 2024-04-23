using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    private bool collected = false;
    public AudioClip checkpointRecogido;

    public bool IsCollected()
    {
        return collected;
    }

    public void Collect()
    {
        collected = true;
        
        AudioSource.PlayClipAtPoint(checkpointRecogido, transform.position, 50);
        
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