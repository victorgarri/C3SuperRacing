using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    private bool collected = false;
    private Color originalColor;

    void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
    }
    
    public bool IsCollected()
    {
        return collected;
    }

    public void Collect()
    {
        collected = true;
        
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