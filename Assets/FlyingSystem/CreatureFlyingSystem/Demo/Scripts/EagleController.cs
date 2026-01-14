using UnityEngine;

using FlyingSystem;

public class EagleController : MonoBehaviour
{
    private Transform characterTransform;
    public Transform meshRootTransform;

    public Transform springArmTransform;
    public Camera characterCamera;
    private Transform characterCameraTransform;

    public Animator animator;

    public TrailRenderer leftWingTrailRenderer, rightWingTrailRenderer;

    public Renderer speedLineParticleRenderer;

    private CreatureFlyingSystem creatureFlyingSystem;

    private AudioSource audioSource;

    private Airflow airflow;

    public bool activated = false;

    [Header("General Attributes")]
    public bool takeOff;
    public bool boosting;

    public float cameraSpeed = 300.0f;

    [Range(0.0f, 100.0f)]
    public float springArmSmoothingFactor = 0.25f;

    public float normalCameraY = 3.0f, normalCameraZ = -12.0f;
    public float divingZoomOutY = 3.0f, divingZoomOutZ = -15.0f;

    private bool hideWingTrails = false;

    private float targetSpringArmRotationX, targetSpringArmRotationY;

    public bool isGrabbing = false;
    private Transform targetGrabObjectTransform;
    private Rigidbody targetGrabObjectRigidbody;

    [Header("Mobile")]
    public Joystick joystick;
    public bool mobileInputControl = false;
    public float mobileCameraSpeed = 300.0f;
    private float screenCenterX;



    public float maxPitchAngle = 30f; // eagle max nose up
    public float minPitchAngle = -30f; // eagle max nose down
  


    private float initialY; // starting height
    private float targetY;

    [Header("Ascend / Descend")]

    public float ascendSpeed = 5f;            // speed going up when holding
    public float descendSpeed = 3f;           // speed going down when releasing
    private float minY;                       // minimum allowed height (ground/starting Y)
                                              // minimum height (ground/initial)


    void Start()
    {
        characterTransform = this.transform;
        // characterCameraTransform = characterCamera.transform;

        initialY = transform.position.y;
        targetY = initialY;

        minY = transform.position.y; // eagle can't go below this

        speedLineParticleRenderer.enabled = false;

        creatureFlyingSystem = this.GetComponent<CreatureFlyingSystem>();

        audioSource = this.GetComponent<AudioSource>();

        screenCenterX = screenCenterX = Screen.width / 2.0f;

        if (activated)
            Activate();
    }

    void Update()
    {
       // print("creatureFlyingSystem.inAir : "+ creatureFlyingSystem.inAir);     for ground
        //if (activated)
        //{
        //    if (!mobileInputControl)
        //    {
        //        PCInputControlLogic();
        //        //CameraControlLogic();
        //    }
        //    else
        //    {
        //        MobileInputControlLogic();
        //      // MobileCameraControlLogic();
        //    }
        //}
        MobileInputControlLogic();
    }

    public void Activate()
    {
        activated = true;
        characterCamera.enabled = true;
        characterCamera.GetComponent<AudioListener>().enabled = true;
    }

    public void Deactivate()
    {
        activated = false;
        characterCamera.enabled = false;
        characterCamera.GetComponent<AudioListener>().enabled = false;
    }

    void PCInputControlLogic()
    {
        // Take off / grab
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (creatureFlyingSystem.inAir)
            {
                if (isGrabbing)
                    Drop();
            }
            else
                TakeOff();
        }

        // Fly forward / stop
        if (Input.GetKey(KeyCode.W))
            creatureFlyingSystem.FlyForward();
        else if (Input.GetKey(KeyCode.S))
            creatureFlyingSystem.SlowDown();
        else if (Input.GetKeyUp(KeyCode.S))
            creatureFlyingSystem.StopSlowingDown();

        // Turn left / right
        creatureFlyingSystem.AddYawInput(Input.GetAxis("Mouse X"));

        DivingLogic();

        // Boost on / off
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            Boost();
    }

    //void MobileInputControlLogic()
    //{
    //    if (joystick == null) return;

    //    float x = joystick.inputAxisX;
    //    float y = joystick.inputAxisY;

    //    Debug.Log($"Joy X:{x}  Y:{y}");

    //    if (y > 0.01f)
    //        creatureFlyingSystem.FlyForward();
    //    else if (y < -0.85f)
    //        creatureFlyingSystem.SlowDown();
    //    else if (creatureFlyingSystem.slowingDown && y > -0.85f)
    //        creatureFlyingSystem.StopSlowingDown();

    //    // 🔥 LEFT / RIGHT FIX
    //    if (creatureFlyingSystem.inAir && Mathf.Abs(x) > 0.15f)
    //    {
    //        float turnSpeed = 120f; // adjust
    //        transform.Rotate(0f, x * turnSpeed * Time.deltaTime, 0f);
    //    }

    //    DivingLogic();
    //}




    void MobileInputControlLogic()
    {
        if (joystick == null || !creatureFlyingSystem.inAir) return;

        float x = joystick.inputAxisX; // left/right
        float y = joystick.inputAxisY; // forward/backward

        Vector3 newPos = transform.position;

        // --- LEFT / RIGHT ROTATION ---
        RotateLeftRight(x);

        // --- FORWARD / BACKWARD ---
        float forwardSpeed = 10f;
        newPos += transform.forward * y * forwardSpeed * Time.deltaTime;

        // --- ASCEND / DESCEND ---
       
          newPos.y -= descendSpeed * Time.deltaTime;
          if (newPos.y < minY) newPos.y = minY;
     

        transform.position = newPos;

        // --- TILT (Roll/Pitch) ---
        Vector3 currentEuler = transform.eulerAngles;
        float targetPitch = Mathf.Clamp(y * 20f, -20f, 20f); // forward/back tilt
        float targetRoll = Mathf.Clamp(-x * 20f, -20f, 20f);  // sideways tilt

        Quaternion targetRotation = Quaternion.Euler(targetPitch, transform.eulerAngles.y, targetRoll);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);

        DivingLogic();
    }



    void RotateLeftRight(float horizontalInput)
    {
        if (Mathf.Abs(horizontalInput) < 0.01f) return; // deadzone

        float turnSpeed = 120f; // adjust as needed
        transform.Rotate(0f, horizontalInput * turnSpeed * Time.deltaTime, 0f);
    }





    void DivingLogic()
    {
        if (creatureFlyingSystem.inAir && creatureFlyingSystem.diving)
        {
            // Camera zoom out
          //  characterCameraTransform.localPosition = Vector3.Lerp(characterCameraTransform.localPosition, new Vector3(0.0f, divingZoomOutY, divingZoomOutZ), 0.95f * Time.deltaTime);

            animator.SetBool("FlyToGlide", true);
            animator.SetBool("GlideToFly", false);

            // Enable trails from both wings
            if (!leftWingTrailRenderer.enabled)
            {
                hideWingTrails = false;

                leftWingTrailRenderer.enabled = true;
                rightWingTrailRenderer.enabled = true;
                
                speedLineParticleRenderer.enabled = true;
            }
        }
        else
        {
            // Reset all effects
           // characterCameraTransform.localPosition = Vector3.Lerp(characterCameraTransform.localPosition, new Vector3(0.0f, normalCameraY, normalCameraZ), 0.5f * Time.deltaTime);

            animator.SetBool("GlideToFly", true);
            animator.SetBool("FlyToGlide", false);

            if (!hideWingTrails)
            {
                hideWingTrails = true;

                leftWingTrailRenderer.enabled = false;
                rightWingTrailRenderer.enabled = false;

                speedLineParticleRenderer.enabled = false;
            }
        }
    }

    void CameraControlLogic()
    {
        springArmTransform.position = Vector3.Lerp(characterTransform.position, springArmTransform.position, springArmSmoothingFactor * Time.deltaTime);
        springArmTransform.rotation = Quaternion.Euler(springArmTransform.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * cameraSpeed * Time.deltaTime, springArmTransform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * cameraSpeed * Time.deltaTime, 0.0f);
    }

    void MobileCameraControlLogic()
    {
        //// Temporarily use mouse to simulate the touch
        //if (Input.GetMouseButton(0) && Input.mousePosition.x > screenCenterX)
        //{
        //    targetSpringArmRotationX = springArmTransform.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * mobileCameraSpeed * Time.deltaTime;
        //    targetSpringArmRotationY = springArmTransform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * mobileCameraSpeed * Time.deltaTime;

        //    creatureFlyingSystem.AddYawInput(Input.GetAxis("Mouse X"));
        //}
        //else
        //{
        //    targetSpringArmRotationX = springArmTransform.rotation.eulerAngles.x;
        //    targetSpringArmRotationY = springArmTransform.rotation.eulerAngles.y;
        //}

        // Only for mobile devices(uncomment the following and test on physical mobile devices)
        if (Input.touchCount > 0)
        {
            for (var i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).position.x > screenCenterX && Input.GetTouch(i).phase == TouchPhase.Moved)
                {
                    targetSpringArmRotationX = springArmTransform.rotation.eulerAngles.x - Input.GetTouch(i).deltaPosition.y * mobileCameraSpeed * Time.deltaTime;
                    targetSpringArmRotationY = springArmTransform.rotation.eulerAngles.y + Input.GetTouch(i).deltaPosition.x * mobileCameraSpeed * Time.deltaTime;

                    creatureFlyingSystem.AddYawInput(Input.GetTouch(i).deltaPosition.x);
                }
            }
        }
        else
        {
            targetSpringArmRotationX = springArmTransform.rotation.eulerAngles.x;
            targetSpringArmRotationY = springArmTransform.rotation.eulerAngles.y;
        }

        springArmTransform.rotation = Quaternion.Euler(targetSpringArmRotationX, targetSpringArmRotationY, 0.0f);
    }

    public void TakeOff()
    {
        if (!creatureFlyingSystem.inAir)
        {
            creatureFlyingSystem.TakeOff();
            takeOff = creatureFlyingSystem.inAir;

            animator.SetBool("FlyToIdle", false);
            animator.SetBool("IdleToFly", true);

            animator.SetBool("GlideToIdle", false);

            audioSource.Play();
        }
    }

    public void Boost()
    {
        creatureFlyingSystem.boosting = !creatureFlyingSystem.boosting;
        boosting = creatureFlyingSystem.boosting;
    }

    public void Drop()
    {
        if (targetGrabObjectTransform != null)
        {
            isGrabbing = false;

            targetGrabObjectTransform.SetParent(null);

            targetGrabObjectRigidbody.useGravity = true;
            targetGrabObjectRigidbody.isKinematic = false;

            creatureFlyingSystem.currentCarryingWeight -= 3.0f;

            targetGrabObjectTransform = null;
        }
    }

    public float GetFlyingSpeed()
    {
        return creatureFlyingSystem.flyingSpeed;
    }

    public float GetStaminaPercentage()
    {
        return creatureFlyingSystem.staminaPercentage;
    }

    public float GetWeightPercentage()
    {
        return creatureFlyingSystem.weightPercentage;
    }

    void OnCollisionEnter(Collision collision)
    {
        // The target collision can be anything like ground, terrain, etc.
        if (collision.collider.name == "Road")
        {
            if (creatureFlyingSystem.inAir && !isGrabbing)
            {
                creatureFlyingSystem.Land();
                takeOff = creatureFlyingSystem.inAir;

                animator.SetBool("GlideToIdle", true);

                animator.SetBool("FlyToIdle", true);
                animator.SetBool("IdleToFly", false);

                animator.SetBool("FlyToGlide", false);

                leftWingTrailRenderer.enabled = false;
                rightWingTrailRenderer.enabled = false;

                speedLineParticleRenderer.enabled = false;
            }
        }
        else if (collision.collider.name == "Weight" && !isGrabbing)
        {
            // Grab
            isGrabbing = true;

            targetGrabObjectTransform = collision.transform;

            targetGrabObjectRigidbody = targetGrabObjectTransform.GetComponent<Rigidbody>();
            targetGrabObjectRigidbody.useGravity = false;
            targetGrabObjectRigidbody.isKinematic = true;

            targetGrabObjectTransform.SetParent(meshRootTransform);
            targetGrabObjectTransform.localPosition = new Vector3(0.0f, -2.25f, -2.172f);

            creatureFlyingSystem.currentCarryingWeight += 3.0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Entering the airflow can lift up the flyer
        if (other.name == "Airflow")
        {
            airflow = other.GetComponent<Airflow>();

            creatureFlyingSystem.AddAirflowForce(airflow.intensity, airflow.acceleration, airflow.fadeOutAcceleration);
            creatureFlyingSystem.stopFlying = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Airflow")
        {
            creatureFlyingSystem.EndAirflowForce();
            creatureFlyingSystem.stopFlying = false;
        }
    }



    
}