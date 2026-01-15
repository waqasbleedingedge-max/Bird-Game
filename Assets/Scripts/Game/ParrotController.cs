using System;
using UnityEngine;

public class ParrotController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float turnSpeed = 120f;

    [Header("Fly Settings")]
    [SerializeField] private float flyUpForce = 7f;

    [Header("Tilt Settings")]
    [SerializeField] private float tiltAngle = 20f;
    [SerializeField] private float tiltSpeed = 6f;

    [Header("References")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private Transform parrotMesh;
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private bool isFly;

    public static Action LevelFailed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    void Update()
    {
        if (joystick == null) return;

        float x = joystick.inputAxisX; // left / right
        float y = joystick.inputAxisY; // forward / backward

        HandleRotation(x);
        HandleMovement(y);
        HandleFlying();
        HandleTilt(x);

        if (transform.position.y < -0.5f)
        {
            Debug.Log("Failed");
            LevelFailed?.Invoke();
        }
    }

    // ---------------- MOVEMENT ----------------
    void HandleMovement(float y)
    {
        Vector3 forward = transform.forward * y * moveSpeed;
        rb.linearVelocity = new Vector3(forward.x, rb.linearVelocity.y, forward.z);
    }

    // ---------------- ROTATION ----------------
    void HandleRotation(float x)
    {
        transform.Rotate(0f, x * turnSpeed * Time.deltaTime, 0f);
    }

    // ---------------- FLY ----------------
    //void HandleFlying()
    //{
    //    if (isFly)
    //    {
    //        rb.useGravity = false;
    //        rb.linearVelocity = new Vector3(rb.linearVelocity.x, flyUpForce, rb.linearVelocity.z);

    //        animator.SetBool("Fly", true);
    //        animator.SetBool("Idle", false);
    //    }
    //    else
    //    {
    //        rb.useGravity = true;
    //    }
    //}


    void HandleFlying()
    {
        if (isFly)
        {
            rb.useGravity = false;
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                flyUpForce,
                rb.linearVelocity.z
            );

            animator.SetBool("Fly", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            // ⭐ SLOW FALL (glide)
            rb.useGravity = true;

            if (rb.linearVelocity.y < -2f)
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    -2f,   // fall limit
                    rb.linearVelocity.z
                );
            }
        }
    }


    // ---------------- TILT (VISUAL ONLY) ----------------
    void HandleTilt(float x)
    {
        if (parrotMesh == null) return;

        float targetRoll = -x * tiltAngle;

        Quaternion targetRotation =
            Quaternion.Euler(0f, 0f, targetRoll);

        parrotMesh.localRotation = Quaternion.Slerp(
            parrotMesh.localRotation,
            targetRotation,
            Time.deltaTime * tiltSpeed
        );
    }

    // ---------------- UI EVENTS ----------------
    public void FlyHold()
    {
        isFly = true;
    }

    public void FlyRelease()
    {
        isFly = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        isFly = false;
        animator.SetBool("Fly", false);
        animator.SetBool("Idle", true);
    }
}
