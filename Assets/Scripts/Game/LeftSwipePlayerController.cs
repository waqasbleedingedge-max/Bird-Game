using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerLeftRightMoveRotateY : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [Header("Settings")]
    public float sideMoveSpeed = 0.02f;      // sideways movement speed
    public float rotateSpeed = 50f;          // rotation speed around Y-axis

    [Header("References")]
    public Transform player;

    private Vector2 lastPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPos = eventData.position;
        Debug.Log("Swipe Start");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Swipe delta
        Vector2 delta = eventData.position - lastPos;
        lastPos = eventData.position;

        float swipeX = delta.x;

        // -------- MOVE LEFT–RIGHT --------
        player.position += player.right * swipeX * sideMoveSpeed;

        // Optional: clamp to prevent going off-screen
        player.position = new Vector3(
            Mathf.Clamp(player.position.x, -8f, 8f),
            player.position.y,
            player.position.z
        );

        // -------- ROTATE AROUND Y --------
        float rotationY = swipeX * rotateSpeed * Time.deltaTime;
        player.Rotate(0f, rotationY, 0f);

        Debug.Log($"Player moved: {swipeX}, rotated Y: {rotationY}");
    }
}
