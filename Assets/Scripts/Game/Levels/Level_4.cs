using UnityEngine;
using UnityEngine.UI;
using System;

public class Level_4 : MonoBehaviour
{
    [Header("Pendulum Settings (Swing 1 Only)")]
    public Transform pendulumObject;
    public float minZ = -15f;
    public float maxZ = 15f;
    public float speed = 1.2f;

    [Header("Level 2 Rotating Object (Swing 2) - Z Axis")]
    public Transform level2RotateObject;
    public float level2MinZ = -15f;
    public float level2MaxZ = 15f;
    public float level2Speed = 1.5f;

    [Header("Level 3 Rotating Object (Swing 3) - X Axis")]
    public Transform level3RotateObject;
    public float level3MinX = -30f;
    public float level3MaxX = 30f;
    public float level3Speed = 1.3f;

    [Header("Level 4 Rotating Object (Swing 4) - X Axis")]
    public Transform level4RotateObject;
    public float level4MinX = -8f;
    public float level4MaxX = 8f;
    public float level4Speed = 1.1f;

    [Header("Hold Duration Per Swing (Seconds)")]
    public float runDuration = 60f;

    [Header("Total Swings")]
    public int totalSwings;

    [Header("Return To Zero")]
    public float returnSpeed = 6f;
    public float nextSwingDelay = 0.25f;

    [Header("UI")]
    public Image holdFillImage;
    public Text swingText;

    [Header("Level Start Objects")]
    public GameObject[] enableOnStart;
    public GameObject[] disableOnStart;

    //[Header("Level Complete Objects")]
    //public GameObject[] enableOnComplete;
    //public GameObject[] disableOnComplete;

    [Header("Swing Objects (Index Based)")]
    public GameObject[] swings;

    [Header("🔥 Level Complete Event")]
    public static Action LevelComplete;

    bool _isHolding;
    float _timer;
    float _phase;
    int _currentSwing = 0;
    bool _eventFired = false;

    Quaternion _pendulumBaseRot;
    Quaternion _level2BaseRot;
    Quaternion _level3BaseRot;
    Quaternion _level4BaseRot;

    enum State { Idle, Swinging, Returning, Completed }
    State _state = State.Idle;

    float _delayTimer;

    void Awake()
    {
        if (!pendulumObject) pendulumObject = transform;
        _pendulumBaseRot = pendulumObject.localRotation;

        if (level2RotateObject) _level2BaseRot = level2RotateObject.localRotation;
        if (level3RotateObject) _level3BaseRot = level3RotateObject.localRotation;
        if (level4RotateObject) _level4BaseRot = level4RotateObject.localRotation;
    }

    void Start()
    {
        totalSwings = swings.Length;
        foreach (GameObject obj in enableOnStart)
            if (obj) obj.SetActive(true);

        foreach (GameObject obj in disableOnStart)
            if (obj) obj.SetActive(false);

        for (int i = 0; i < swings.Length; i++)
            if (swings[i]) swings[i].SetActive(i == 0);

        if (holdFillImage) holdFillImage.fillAmount = 0f;
        UpdateSwingText();
    }

    void LateUpdate()
    {
        if (_state == State.Completed)
            return;

        // ---------- ROTATIONS (LEVEL-WISE) ----------
        if (_isHolding && _state == State.Swinging)
        {
            if (_currentSwing == 1 && level2RotateObject)
            {
                float z = Mathf.Lerp(level2MinZ, level2MaxZ,
                    (Mathf.Sin(Time.time * level2Speed) + 1f) * 0.5f);
                level2RotateObject.localRotation =
                    _level2BaseRot * Quaternion.Euler(0f, 0f, z);
            }

            if (_currentSwing == 2 && level3RotateObject)
            {
                float x = Mathf.Lerp(level3MinX, level3MaxX,
                    (Mathf.Sin(Time.time * level3Speed) + 1f) * 0.5f);
                level3RotateObject.localRotation =
                    _level3BaseRot * Quaternion.Euler(x, 0f, 0f);
            }

            if (_currentSwing == 3 && level4RotateObject)
            {
                float x = Mathf.Lerp(level4MinX, level4MaxX,
                    (Mathf.Sin(Time.time * level4Speed) + 1f) * 0.5f);
                level4RotateObject.localRotation =
                    _level4BaseRot * Quaternion.Euler(x, 0f, 0f);
            }
        }
        else
        {
            // Release OR not swinging -> return extras to base
            if (level2RotateObject)
                level2RotateObject.localRotation = Quaternion.Slerp(
                    level2RotateObject.localRotation, _level2BaseRot, Time.deltaTime * returnSpeed);

            if (level3RotateObject)
                level3RotateObject.localRotation = Quaternion.Slerp(
                    level3RotateObject.localRotation, _level3BaseRot, Time.deltaTime * returnSpeed);

            if (level4RotateObject)
                level4RotateObject.localRotation = Quaternion.Slerp(
                    level4RotateObject.localRotation, _level4BaseRot, Time.deltaTime * returnSpeed);
        }

        // ---------- MAIN SWING LOGIC ----------
        if (_state == State.Swinging)
        {
            if (!_isHolding)
            {
                ForceReturn();
                return;
            }

            _timer += Time.deltaTime;

            if (holdFillImage)
                holdFillImage.fillAmount = Mathf.Clamp01(_timer / runDuration);

            if (_timer >= runDuration)
            {
                CompleteCurrentSwing();
                return;
            }

            // Pendulum only on Swing 1
            if (_currentSwing == 0)
            {
                _phase += Time.deltaTime * speed;
                float t = (Mathf.Sin(_phase) + 1f) * 0.5f;
                float z = Mathf.Lerp(minZ, maxZ, t);

                pendulumObject.localRotation =
                    _pendulumBaseRot * Quaternion.Euler(0f, 0f, z);
            }
        }
        else if (_state == State.Returning)
        {
            // pendulum return to base
            pendulumObject.localRotation = Quaternion.Slerp(
                pendulumObject.localRotation,
                _pendulumBaseRot,
                Time.deltaTime * returnSpeed);

            // ✅ THIS IS THE FIX: start next swing after delay
            _delayTimer -= Time.deltaTime;
            if (_delayTimer <= 0f)
            {
                if (_isHolding)
                    StartNextSwing();
                else
                    _state = State.Idle;
            }
        }
        else // Idle
        {
            // keep pendulum at base smoothly
            pendulumObject.localRotation = Quaternion.Slerp(
                pendulumObject.localRotation,
                _pendulumBaseRot,
                Time.deltaTime * returnSpeed);
        }
    }

    void CompleteCurrentSwing()
    {
        _currentSwing++;
        UpdateSwingText();
        ToggleSwings();

        if (_currentSwing >= totalSwings)
        {
            Debug.Log("🎉 LEVEL 4 COMPLETED");
            _state = State.Completed;
            _isHolding = false;

            foreach (GameObject obj in enableOnStart)
                if (obj) obj.SetActive(false);

            foreach (GameObject obj in disableOnStart)
                if (obj) obj.SetActive(true);

            if (!_eventFired)
            {
                _eventFired = true;
                LevelComplete?.Invoke();
            }

            if (holdFillImage) holdFillImage.fillAmount = 1f;
            return;
        }

        _state = State.Returning;
        _timer = 0f;
        _phase = 0f;
        _delayTimer = nextSwingDelay;

        if (holdFillImage) holdFillImage.fillAmount = 0f;
    }

    void ToggleSwings()
    {
        for (int i = 0; i < swings.Length; i++)
            if (swings[i]) swings[i].SetActive(i == _currentSwing);
    }

    void StartNextSwing()
    {
        _state = State.Swinging;
        _timer = 0f;
        _phase = 0f;

        if (holdFillImage) holdFillImage.fillAmount = 0f;
        Debug.Log($"▶️ Swing {_currentSwing + 1}/{totalSwings}");
    }

    void ForceReturn()
    {
        _state = State.Returning;
        _timer = 0f;
        _phase = 0f;
        _delayTimer = 0f;

        if (holdFillImage) holdFillImage.fillAmount = 0f;
    }

    void UpdateSwingText()
    {
        if (swingText)
            swingText.text = $"Swing {_currentSwing}/{totalSwings}";
    }

    public void StartButtonHold()
    {
        _isHolding = true;

        if (_state == State.Idle)
            StartNextSwing();
    }

    public void StopButtonHold()
    {
        _isHolding = false;

        if (_state != State.Completed)
            ForceReturn();

        if (holdFillImage) holdFillImage.fillAmount = 0f;
    }
}
