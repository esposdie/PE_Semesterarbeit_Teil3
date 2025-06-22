using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/**
 * Done by KPP (pern) in 2025!
 */

public class Car : MonoBehaviour
{
    // the rigid body of the car
    private Rigidbody rb;

    // the rigid body of the bumpers
    private Rigidbody leftBumper;
    private Rigidbody rightBumper;

    // the Exporter script (if attached to the game object)
    private Exporter exporter;

    // the time the car was launched
    private float launchTime;

    // flag to remember if car was launched
    private bool isLaunched = false;

    // the width of the car (could also be gotten from the collider)
    private readonly float carWidth = 0.3f;

    // the width of the bumpers (could also be gotten from the collider)
    private readonly float bumperWidth = 0.1f;

    // helper flag to automatically start the car when recording starts
    // Window > General > Recorder > Start Recording
    private readonly bool recording = false;

    // initial velocity of the car
    public float initialVelocity = 2f;

    private bool jointFixed = false;

    private readonly bool isRotation = false; // Task 2 = false, 3 = true

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get the rigid body of the car
        rb = GetComponent<Rigidbody>();

        // get the rigid body of the bumpers
        leftBumper = GameObject.Find("Bumper left").GetComponent<Rigidbody>();
        rightBumper = GameObject.Find("Bumper right").GetComponent<Rigidbody>();

        // get the Exporter script
        exporter = GetComponent<Exporter>();
        Assert.IsNotNull(exporter, "Exporter script not found");


        // confine motion of the to 1D along z-axis
        if (!isRotation) {
            rb.constraints = RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
        } else {
            rb.constraints = RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
        }

        // set motion of the bumpers
        rightBumper.constraints = RigidbodyConstraints.FreezeAll;
        leftBumper.constraints = RigidbodyConstraints.FreezePositionY;
        leftBumper.isKinematic = false;


        // Note: switch off collider in the inspector, because depending on the spring paramters, it can happen that the car touches the bumpers
        // and then the movement looks very strange and debugging is difficult (try increasing initial velocity). Without collider one sees
        // that the car penetrates the bumpers and debugging becomes easier.


        // === solver settings ===

        // Controls how often physics updates occur (default: 0.02s or 50 Hz)
        Time.fixedDeltaTime = 0.02f;

        // Determines how many times Unity refines the constraint solving per physics step(default: 6)
        Physics.defaultSolverIterations = 6;

        // Similar to above but specifically for velocity constraints (default: 1)
        Physics.defaultSolverVelocityIterations = 1;
    }



    // Update is called once per frame
    void Update()
    {
        // launch car
        if (Keyboard.current[Key.Space].wasPressedThisFrame || (recording && !isLaunched))
        {
            // remember that car was launched
            isLaunched = true;

            // remember the current time
            launchTime = Time.time;

            // Your code here ...
            rb.linearVelocity = Vector3.forward * initialVelocity;

            // log
            Debug.Log("Launching the car");
        }


        // reload scene
        if (Keyboard.current[Key.R].wasPressedThisFrame)
        {
            // Reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    private void FixedUpdate()
    {
        // Your code here ...
        if (!isLaunched) return;

        // Get current positions
        float carPos = rb.position.z;
        float leftBumperPos = leftBumper.position.z;
        float rightBumperPos = rightBumper.position.z;

        // Calculate edges
        float carLeftEdge = carPos - carWidth/2;
        float carRightEdge = carPos + carWidth/2;
        float leftBumperRightEdge = leftBumperPos + bumperWidth/2;
        float rightBumperLeftEdge = rightBumperPos - bumperWidth/2;

        // === left collision
        float distanceLeft = carLeftEdge - leftBumperRightEdge;
        float impulseCar = rb.mass * rb.linearVelocity.z;
        float impulseLeftBumper = leftBumper.mass * leftBumper.linearVelocity.z;

        if (distanceLeft < 0 && !jointFixed) // inelastischer Stoss
        {
            FixedJoint joint = rb.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = leftBumper;
            jointFixed = true;
        }

        // === right collision
        float distanceRight = rightBumperLeftEdge - carRightEdge;
        if (distanceRight < 0)
        {
            float newVelocity = -rb.linearVelocity.z;
            rb.linearVelocity = new Vector3(0, 0, newVelocity);

            float penetrationDepth = -distanceRight;
            rb.position += Vector3.back * penetrationDepth;
        }

        // === time series data
        if (isLaunched)
        {
            TimeSeriesData timeSeriesData = new(rb, Time.time - launchTime, 0f, 0f, 0f, 0f, leftBumper.position.z, leftBumper.linearVelocity.z);
            exporter.AddData(timeSeriesData);
        }
    }

    void OnGUI()
    {
        GUIStyle textStyle = new()
        {
            fontSize = 20,
            normal = { textColor = Color.black }
        };

        // keyboard shortcuts
        GUI.Label(new Rect(10, Screen.height - 20, 400, 20),
            "R ... Reload", textStyle);
    }
}
