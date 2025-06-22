using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Done by KPP (pern) in 2025!
 */

public class Exporter : MonoBehaviour
{
    // references to the game objcet
    private Rigidbody rb;

    // the time series with the data to export
    private readonly List<TimeSeriesData> timeSeries = new();

    // Input actions
    private InputAction saveAction;

    // Start is called before the first frame update
    void Start()
    {
        // store references rigid body
        rb = GetComponent<Rigidbody>();

        // Initialize input actions
        var playerInput = new InputActionMap("Player");
        saveAction = playerInput.AddAction("Save", binding: "<Keyboard>/o");
        saveAction.performed += ctx => WriteTimeSeriesToCSV();
        playerInput.Enable();
    }


    // adds a data point to the time series
    public void AddData(TimeSeriesData data)
    {
        timeSeries.Add(data);
    }


    // Write the time series to a file
    void WriteTimeSeriesToCSV()
    {
        using var streamWriter = new StreamWriter("time_series.csv");
        streamWriter.WriteLine(TimeSeriesData.Header());

        foreach (TimeSeriesData timeStep in timeSeries)
        {
            streamWriter.WriteLine(timeStep.ToString());
            streamWriter.Flush();
        }

        // log
        Debug.Log("Time series data has been saved to time_series.csv");
    }

    // OnGUI is called for rendering and handling GUI events
    void OnGUI()
    {
        GUIStyle textStyle = new()
        {
            fontSize = 20,
            normal = { textColor = Color.black }
        };

        // keyboard shortcuts
        GUI.Label(new Rect(10, Screen.height - 60, 400, 20),
            "Space ... Launch car\n"
            + "O ... output data as csv", textStyle);
    }
}


// Data structure to store the time series data
public class TimeSeriesData
{
    public float Time { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public float VelocityZ { get; set; }
    public float CompressionLeftSpring { get; set; }
    public float ForceLeftSpring { get; set; }
    public float CompressionRightSpring { get; set; }
    public float ForceRightSpring { get; set; }
    public float BumperPositionZ { get; set; }
    public float BumperVelocityZ { get; set; }

    // Empty constructor (required since a specialized constructor is defined)
    public TimeSeriesData()
    {
    }

    // Constructor that adds the current position etc. to the time series
    public TimeSeriesData(Rigidbody rb, float currentTime, 
                          float compressionLeftSpring, float forceLeftSpring, 
                          float compressionRightSpring, float forceRightSpring,
                          float bumperPositionZ, float bumperVelocityZ)
    {
        Time = currentTime;
        PositionX = rb.position.x;
        PositionY = rb.position.y;
        PositionZ = rb.position.z;
        VelocityX = rb.linearVelocity.x;
        VelocityY = rb.linearVelocity.y;
        VelocityZ = rb.linearVelocity.z;
        CompressionLeftSpring = compressionLeftSpring;
        ForceLeftSpring = forceLeftSpring;
        CompressionRightSpring = compressionRightSpring;
        ForceRightSpring = forceRightSpring;
        BumperPositionZ = bumperPositionZ;
        BumperVelocityZ = bumperVelocityZ;
    }

    public override string ToString()
    {
        return $"{Time},{PositionX},{PositionY},{PositionZ},{VelocityX},{VelocityY},{VelocityZ},{CompressionLeftSpring},{ForceLeftSpring},{CompressionRightSpring},{ForceRightSpring},{BumperPositionZ},{BumperVelocityZ}";
    }

    public static string Header()
    {
        // Note: avoid spaces as they would become part of the colum name if imported using pandas!!
        return "t,x,y,z,vx,vy,vz,dl_left,F_left,dl_right,F_right,z_bumper,vz_bumper";
    }
}
