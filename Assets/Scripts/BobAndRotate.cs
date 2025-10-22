using UnityEngine;

public class BobAndRotate : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobAmplitude = 0.5f;   // How high it bobs
    public float bobFrequency = 1f;     // How fast it bobs

    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0f, 90f, 0f); // degrees per second

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // --- Bobbing ---
        float newY = startPosition.y + Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // --- Rotating ---
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
