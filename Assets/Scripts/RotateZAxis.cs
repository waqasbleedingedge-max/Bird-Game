using UnityEngine;

public class RotateAxis : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [Header("Rotation Settings")]
    [SerializeField] private RotationAxis axis = RotationAxis.Z;
    [SerializeField] private float rotationSpeed = 90f; // degrees per second

    void Update()
    {
        Vector3 rotation = Vector3.zero;

        switch (axis)
        {
            case RotationAxis.X:
                rotation.x = rotationSpeed * Time.deltaTime;
                break;

            case RotationAxis.Y:
                rotation.y = rotationSpeed * Time.deltaTime;
                break;

            case RotationAxis.Z:
                rotation.z = rotationSpeed * Time.deltaTime;
                break;
        }

        transform.Rotate(rotation);
    }
}
