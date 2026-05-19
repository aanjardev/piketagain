using UnityEngine;

public class HintMarkerFloat : MonoBehaviour
{
    [Header("Floating")]
    public float floatSpeed = 2f;
    public float floatAmount = 0.15f;

    [Header("Look At Camera")]
    public bool faceCamera = true;

    private Vector3 _startPos;
    private Camera _cam;

    void Start()
    {
        _startPos = transform.position;
        _cam = Camera.main;
    }

    void Update()
    {
        // Animasi naik turun
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = _startPos + new Vector3(0f, yOffset, 0f);

        // Selalu menghadap kamera
        if (faceCamera && _cam != null)
        {
            transform.LookAt(transform.position + _cam.transform.forward);
        }
    }
}