using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;

public class PythonRunner : MonoBehaviour
{
    public float Speed;
    

    private Vector3 MoveBall;
    private bool moveTowardsTarget = false;

    void Start()
    {
        // specify the path to the Anaconda environment activation script
        string activateScript = @"C:\Users\RaPIDadmin\anaconda3\Scripts\activate.bat";

        // specify the name of the Anaconda environment
        string environmentName = "py38";

        // specify the path to the Python script that you want to run
        string scriptPath = @"C:\Users\RaPIDadmin\Headpose_est.py";

        // specify the command to activate the Anaconda environment and run the Python script
        string command = $@"call {activateScript} {environmentName} && python -u {scriptPath}";

        // start a new process to run the command
        Process process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = $@"/c ""{command}""";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            // parse the output of the Python script
            if (e.Data.Contains("Entry"))
            {
                moveTowardsTarget = false;
                string[] data = e.Data.Split(',');
                float x = float.Parse(data[1]);
                float z = float.Parse(data[2]);
                UnityEngine.Debug.Log(x+","+z);

                // update the value of the MoveBall vector in real-time
                MoveBall = new Vector3(x, 0, z);
            }

            else if (e.Data.Contains("Recalibrating"))
                {
                    moveTowardsTarget = true;
                    MoveBall = Vector3.zero;
                }
        });

        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UnityEngine.Debug.LogError("Python error: " + e.Data);
            }
        });

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();
    }

    void FixedUpdate()
    {   

        if (moveTowardsTarget)
        {
            // Get a reference to the game object's transform
            Transform myTransform = gameObject.transform;

            // Set the position of the transform to (0,0,0)
            myTransform.position = Vector3.zero;
        }

        else
        {
            // move the game object based on the parsed string value
            gameObject.transform.GetComponent<Rigidbody>().AddForce(MoveBall * Speed);
            UnityEngine.Debug.Log("MoveBall: " + MoveBall + ", Speed: " + Speed);
        }
    }

}
