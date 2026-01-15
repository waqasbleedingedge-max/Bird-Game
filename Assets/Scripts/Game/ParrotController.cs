using System;
using UnityEngine;

public class ParrotController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float turnSpeed = 120f;

    [Header("Fly Settings")]
    [SerializeField] private float flyUpForce = 7f;
    [SerializeField] private float glideFallSpeed = 2f;     // how fast it can fall while gliding
    [SerializeField] private float gravityOnRelease = 0.4f; // smooth gravity return (0 = instant)

    [Header("Tilt Settings")]
    [SerializeField] private float tiltAngle = 20f;
    [SerializeField] private float tiltSpeed = 6f;

    [Header("References")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private Transform parrotMesh;
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    [SerializeField] private bool isFly;
    [SerializeField] private bool isGrounded;
    private float gravityLerp; // 0..1

    public static Action LevelFailed;

    private float x;
    private float y;



 

  
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        gravityLerp = 1f;
        animator.SetBool("isGrounded", true);
    }

    void Update()
    {
        if (joystick == null) return;

        x = joystick.inputAxisX;
        y = joystick.inputAxisY;

        HandleRotation(x);
        HandleTilt(x);
        HandleAnimations(y);   // ⭐ NEW

        if (transform.position.y < -0.5f)
        {
            Debug.Log("Failed");
            LevelFailed?.Invoke();
        }
    }




    void FixedUpdate()
    {
        HandleMovement(y);
        HandleFlying();
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

    void HandleAnimations(float y)
    {
        if (isFly)
        {
            animator.SetBool("Fly", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", false);
            return;
        }

        if (Mathf.Abs(y) > 0.1f)
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Idle", false);
            animator.SetBool("Fly", false);
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Fly", false);
        }
    }

    // ---------------- FLY + GLIDE ----------------
    void HandleFlying()
    {
        if (isFly)
        {
            gravityLerp = 0f; // gravity off instantly while holding fly
            rb.useGravity = false;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, flyUpForce, rb.linearVelocity.z);

            animator.SetBool("Fly", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            // smooth gravity return
            gravityLerp = Mathf.MoveTowards(gravityLerp, 1f, Time.fixedDeltaTime / Mathf.Max(0.01f, gravityOnRelease));
            rb.useGravity = gravityLerp > 0.9f;

            // glide: limit fall speed
            if (rb.linearVelocity.y < -glideFallSpeed)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -glideFallSpeed, rb.linearVelocity.z);
            }

            animator.SetBool("Fly", false);
            animator.SetBool("Idle", true);
        }
    }

    // ---------------- TILT (VISUAL ONLY) ----------------
    void HandleTilt(float x)
    {
        if (parrotMesh == null) return;

        float targetRoll = -x * tiltAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetRoll);

        parrotMesh.localRotation = Quaternion.Slerp(
            parrotMesh.localRotation,
            targetRotation,
            Time.deltaTime * tiltSpeed
        );
    }

    // ---------------- UI EVENTS ----------------
    public void FlyHold() => isFly = true;
    public void FlyRelease() => isFly = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isFly = false;
            animator.SetBool("isGrounded", true);
            animator.SetBool("Fly", false);
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {

            animator.SetBool("isGrounded", false);

        }
    }

}
