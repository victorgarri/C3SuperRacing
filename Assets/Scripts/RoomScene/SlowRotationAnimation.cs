using UnityEngine;

public class SlowRotationAnimation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed=1;
    private void Update()
    {
        this.transform.Rotate(Vector3.up,rotationSpeed);
    }
}
