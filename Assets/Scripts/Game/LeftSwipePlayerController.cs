using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerRotateYOnly : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [Header("Rotation Settings")]
    public float rotateSpeed = 0.2f;   // touch sensitivity (mobile friendly)

    [Header("Reference")]
    public Transform player;

    private Vector2 lastPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - lastPos;
        lastPos = eventData.position;

        float swipeX = delta.x;

        // 🔄 ONLY ROTATE AROUND Y AXIS
        player.Rotate(0f, swipeX * rotateSpeed, 0f, Space.World);
    }
}
