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

            // Stop resetting when close enough to center
            if (Vector3.Distance(joystickControllerTransform.position, joystickBackgroundTransform.position) < 0.1f)
            {
                joystickControllerTransform.position = joystickBackgroundTransform.position;
                reset = false;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        isMoving = true;

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
