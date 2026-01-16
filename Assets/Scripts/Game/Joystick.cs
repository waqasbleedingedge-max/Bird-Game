using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public Transform joystickBackgroundTransform;
    public Transform joystickControllerTransform;

    [Header("Settings")]
    public float maxMovingDistance = 77.5f;

    [HideInInspector] public bool isMoving = false;

    [HideInInspector] public float x, y;
    [HideInInspector] public float inputAxisX, inputAxisY;

    private bool reset = false;

    public static Action OnClickJoyStick;

    void Update()
    {
        if (reset)
        {
            // Smoothly move joystick back to center
            joystickControllerTransform.position = Vector3.Lerp(
                joystickControllerTransform.position,
                joystickBackgroundTransform.position,
                10f * Time.deltaTime
            );

            // Update axes
            UpdateAxes();

            if (Vector3.Distance(joystickControllerTransform.position, joystickBackgroundTransform.position) < 0.1f)
            {
                joystickControllerTransform.position = joystickBackgroundTransform.position;
                inputAxisX = 0f;
                inputAxisY = 0f;
                x = 0f;
                y = 0f;
                reset = false;
            }

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        isMoving = true;

        OnClickJoyStick?.Invoke();

        Vector2 direction = eventData.position - (Vector2)joystickBackgroundTransform.position;

        // Clamp the joystick to a circle
        if (direction.magnitude > maxMovingDistance)
            direction = direction.normalized * maxMovingDistance;

        // Move joystick
        joystickControllerTransform.position = new Vector3(direction.x + joystickBackgroundTransform.position.x,
                                                           direction.y + joystickBackgroundTransform.position.y,
                                                           0f);

        // Update axes
        UpdateAxes();

        reset = false;


      
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isMoving = false;
        reset = true;
    }

    private void UpdateAxes()
    {
        x = joystickControllerTransform.position.x - joystickBackgroundTransform.position.x;
        y = joystickControllerTransform.position.y - joystickBackgroundTransform.position.y;

        inputAxisX = x / maxMovingDistance;
        inputAxisY = y / maxMovingDistance;
    }
}
