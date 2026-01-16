using UnityEngine;

public class Level_4 : MonoBehaviour
{
    [Header("Pendulum Settings")]
    public Transform pendulumObject;
    public float minZ = -15f;
    public float maxZ = 15f;
    public float speed = 1.2f;

    [Header("Hold to Run (1 Minute)")]
    public float runDuration = 60f;

    [Header("Return To Zero")]
    public float returnSpeed = 6f;

    bool _isHolding;
    float _timer;
    float _phase;

    Quaternion _baseLocalRot;

    [Header("Objects To Enable On Level Start")]
    public GameObject[] enableOnStart;

    [Header("Objects To Disable On Level Start")]
    public GameObject[] disableOnStart;

    void Awake()
    {
        if (!pendulumObject) pendulumObject = transform;
        _baseLocalRot = pendulumObject.localRotation;
    }

    void Start()
    {
        foreach (GameObject obj in enableOnStart)
            if (obj) obj.SetActive(true);

        foreach (GameObject obj in disableOnStart)
            if (obj) obj.SetActive(false);
    }

    void LateUpdate()
    {
        if (_isHolding)
        {
            _timer += Time.deltaTime;
            if (_timer >= runDuration)
            {
                StopButtonHold();
                return;
            }

            _phase += Time.deltaTime * speed;

            float t = (Mathf.Sin(_phase) + 1f) * 0.5f;
            float angle = Mathf.Lerp(minZ, maxZ, t);

            pendulumObject.localRotation =
                _baseLocalRot * Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            // ✅ RELEASE → smoothly back to 0
            pendulumObject.localRotation = Quaternion.Slerp(
                pendulumObject.localRotation,
                _baseLocalRot,
                Time.deltaTime * returnSpeed
            );
        }
    }

    public void StartButtonHold()
    {
        Debug.Log("Press");
        _isHolding = true;
        _timer = 0f;
        _phase = 0f;
    }

    public void StopButtonHold()
    {
        Debug.Log("Release");
        _isHolding = false;
        _timer = 0f;
    }
}
