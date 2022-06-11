using UnityEngine;
using clCollision;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class GJKtest : MonoBehaviour
{
    [Tooltip("0 = infinte")]
    public float TestMaxRunTime; // Change to time?

    [Tooltip("Enables extra debug lines. Use only a few colliders in scene to avoid clutter." +
        "Red lines point to the simplex points at termination, from origin. Blue lines show the support search directions, " +
        "starting from the collider's transform's position.")]
    public bool DrawDebugLines = false;

    public Text uiText;

    private GJK.Output GJKoutput;

    private CL_Collider[] cl_CollidersInScene;

    // eval
    private float startGJKExecutionTime = 0.0f;
    private float totalGJKExecutionTime = 0.0f;
    private List<GJK.Output> outputList;

    private float TestStartTime;
    private float TestRunTime = 0.0f;
    private int frames = 0;

    private void Start()
    {
        cl_CollidersInScene = FindObjectsOfType<CL_Collider>();
        outputList = new List<GJK.Output>(cl_CollidersInScene.Length * 50000);

        TestStartTime = Time.realtimeSinceStartup;
    }

    private void LateUpdate()
    {
        GJK.DRAWLINES = DrawDebugLines;

        if ((TestRunTime > TestMaxRunTime) && (TestMaxRunTime != 0.0f))
            return;

        TestAllCollidersInSceneOnce();

        frames++;
        TestRunTime += Time.realtimeSinceStartup - TestStartTime;

        uiText.text = $"Test Running... {Math.Round(TestRunTime, 1)} / {TestMaxRunTime}";

        if (TestRunTime > TestMaxRunTime)
            ComputeMeasurementsAndDisplay();
    }

    private void ComputeMeasurementsAndDisplay()
    {
        if (outputList.Count > 0)
        {
            int totalGJKIterations = 0;
            for (int i = 0; i < outputList.Count; i++)
            {
                totalGJKIterations += outputList[i].Iterations;
            }

            uiText.text =
                $"Total No. of GJK calls:\n <b>{outputList.Count}</b>.\n\n" +
                $"Total No. of Frames:\n <b>{frames}</b>.\n\n" +
                $"No. of GJK calls / frame:\n <b>{Math.Round((double)outputList.Count/frames)}</b>.\n\n" +
                $"Avg. FPS:\n <b>{Math.Ceiling(frames/TestRunTime)}</b>.\n\n" +
                $"Avg. No. of GJK-iterations per call:\n <b>{Math.Round((double)totalGJKIterations/outputList.Count, 2)}</b>.";
        }
    }

    /// <summary>
    /// test all colliders in cl_CollidersInScene with one another only once
    /// </summary>
    private void TestAllCollidersInSceneOnce()
    {
        for (int i = 0; i < cl_CollidersInScene.Length; i++) // forwards
            for (int j = cl_CollidersInScene.Length-1; j > 0; j--) // backwards
            {
                if (j <= i) continue;

                startGJKExecutionTime = Time.realtimeSinceStartup;
                GJKoutput = GJK.GJK_intersect(cl_CollidersInScene[i], cl_CollidersInScene[j]);
                totalGJKExecutionTime += Time.realtimeSinceStartup - startGJKExecutionTime;

                outputList.Add(GJKoutput);
            }
    }

}
