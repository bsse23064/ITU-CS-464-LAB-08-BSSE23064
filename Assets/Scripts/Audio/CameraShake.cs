using UnityEngine;

// Put it on an empty parent of the camera, so the follow script and the shake move different Transforms.
public class CameraShake : MonoBehaviour
{
    [SerializeField] float decay = 1.6f;
    [SerializeField] float maxOffset = 0.35f;
    [SerializeField] float maxAngle = 4f;
    [SerializeField] float frequency = 18f;
    float trauma;

    public void AddTrauma(float amount) => trauma = Mathf.Min(1f, trauma + amount);

    void LateUpdate()
    {
        trauma = Mathf.Max(0f, trauma - decay * Time.unscaledDeltaTime);
        float k = trauma * trauma, t = Time.unscaledTime * frequency;
        transform.localPosition = new Vector3(Noise(0f, t), Noise(1f, t), 0f) * maxOffset * k;
        transform.localRotation = Quaternion.Euler(0f, 0f, Noise(2f, t) * maxAngle * k);
    }

    static float Noise(float seed, float t) => Mathf.PerlinNoise(seed * 10f, t) * 2f - 1f;
}