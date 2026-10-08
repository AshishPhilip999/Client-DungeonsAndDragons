using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float swayIntensity = 0.1f;   // How strong the sway is
    public float swaySpeed = 1.5f;       // Speed of sway

    private Vector3 originalLocalPos;

    void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    void Update()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayIntensity;
        float swayY = Mathf.Cos(Time.time * swaySpeed) * swayIntensity;

        transform.localPosition = originalLocalPos + new Vector3(swayX, swayY, 0);
    }
}
