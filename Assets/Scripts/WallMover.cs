using UnityEngine;

public class WallMover : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        // Megkeresi a fő kamerát
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // A fal X pozíciója marad, de az Y (magassága) milliméter pontosan követi a kamerát!
        transform.position = new Vector3(transform.position.x, cameraTransform.position.y, transform.position.z);
    }
}